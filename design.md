# Laerdal Implementer Backend - Design Document

## 1. Domain Model

### Organizations

- **Hierarchy**: Parent-child relationships (hospitals → departments/sub-units)
- **Unique identifier**: GUID (`Id`)
- **Core attributes**:
  - `Name`, `Code` (unique short identifier)
  - `Type` (enum: Hospital, Department, Training Center)
  - `ParentId` (nullable; null = root organization)
  - `IsActive` (soft delete via flag)
  - `CreatedAt`, `UpdatedAt` (audit trail)
  - `ExternalId` (link to OIDC/enterprise directory)

- **Business rules**:
  - Organization code must be unique within parent scope
  - Deleting parent organization requires all children to be deleted first (or soft-deleted)
  - Only active orgs can have active children

### Manifests

- **Immutability of published versions**: Published manifest versions are immutable snapshots
- **Core attributes**:
  - `Id`, `OrganizationId` (foreign key)
  - `Name`, `Description`
  - `Version` (semantic versioning: Major.Minor.Patch)
  - `Status` (enum: Draft, Published, Archived)
  - `PublishedAt` (timestamp when transitioned to Published)
  - `Content` (JSON blob describing courses/activities structure)
  - `CreatedAt`, `UpdatedAt`

- **Content Structure** (inside JSON):

  ```json
  {
    "courses": [
      {
        "id": "uuid",
        "title": "Course Title",
        "description": "",
        "activities": [
          {
            "id": "uuid",
            "title": "Activity Title",
            "type": "lesson|simulation|assessment",
            "duration": 30,
            "sequence": 1
          }
        ]
      }
    ]
  }
  ```

- **Business rules**:
  - Only one manifest can be "Published" per organization at a time
  - Published manifests cannot be modified (new versions only)
  - When a new manifest is published, previous published version transitions to Archived
  - Draft manifests can be deleted, published manifests are soft-deleted by archiving
  - Learners always access the currently Published manifest for their org

### Domain Events (for future audit/integration)

- `OrganizationCreated`, `OrganizationUpdated`, `OrganizationDeleted`
- `ManifestPublished`, `ManifestArchived`

---

## 2. API Surface

### Key Operations (REST endpoints)

#### Organizations

1. **POST /api/organizations**
   - Create organization
   - Body: `{ name, code, type, parentId? }`
   - Returns: Organization (201)

2. **GET /api/organizations**
   - List all organizations (with optional parent filter)
   - Query: `?parentId=uuid` (optional)
   - Returns: Organization[] (200)

3. **GET /api/organizations/{id}**
   - Get single organization with hierarchy info
   - Returns: Organization + children (200)

4. **PATCH /api/organizations/{id}**
   - Update organization (name, type, active status)
   - Returns: Updated Organization (200)

5. **DELETE /api/organizations/{id}**
   - Soft-delete organization (sets `IsActive = false`)
   - Blocked if active child organizations exist
   - Returns: 204 No Content / 400 if active children

#### Manifests

6. **POST /api/organizations/{orgId}/manifests**
   - Create/draft a new manifest
   - Body: `{ name, description, content? }`
   - Returns: Manifest in Draft status (201)

7. **POST /api/organizations/{orgId}/manifests/{manifestId}/publish**
   - Publish a manifest (transitions Draft → Published)
   - Archives previous published version
   - Body: `{ versionBump: "major|minor|patch" }`
   - Returns: Published Manifest (200)

#### Training App (Service-to-Service)

8. **GET /api/manifests/published/{orgCode}**
   - Fetch currently published manifest by organization code
   - **Auth**: Service account with read-only scope
   - Returns: Published Manifest with full content (200)
   - _No version history leaked; only active manifest visible_

---

## 3. Authentication & Authorization

### Architecture

- **OIDC Provider Integration**: Laerdal OIDC endpoint issues JWT tokens
- **Token structure**: JWT includes claims for `sub` (user ID), `org` (primary organization), `roles` (admin, editor, viewer)

### Authorization Levels

**React Frontend (User-Facing)**

- Endpoint: `POST /api/auth/login` (redirects to OIDC → callback)
- Scopes: `openid profile email org`
- **Laerdal Admin** (via claim `role=laerdal_admin`):
  - Full CRUD on any organization
  - Full manifest management across all orgs
- **Hospital Admin** (via claim `role=org_admin` + `orgId` claim):
  - CRUD limited to own org and descendants
  - Can manage manifests for their org
  - Cannot modify parent org
- **Hospital Editor**:
  - Can draft/publish manifests (not delete)
  - Read-only on organization settings

**Training App (Service-to-Service)**

- OAuth 2.0 Client Credentials flow
- Client ID/Secret issued per training deployment
- Scope: `manifest:read:org-code`
- Token returned by `POST /oauth/token`
- Caller must supply `orgCode` in query param; token validates it has access

### Implementation Strategy

- Middleware validates JWT signature via OIDC public key (cached, refreshed periodically)
- Policy-based authorization: `[Authorize(Policy = "LaerdalAdminOnly")]`
- Custom claims principal extraction in `AuthenticationService`
- Read-only scoped cache for org hierarchy (5-min TTL) to avoid recursive queries on authorization checks

---

## 4. Persistence & Versioning

### Database Schema

**Organizations**

```sql
CREATE TABLE Organizations (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  ParentId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Organizations(Id),
  Name NVARCHAR(255) NOT NULL,
  Code NVARCHAR(50) NOT NULL,
  Type INT NOT NULL, -- 0=Hospital, 1=Department, 2=TrainingCenter
  ExternalId NVARCHAR(255) NULL,
  IsActive BIT NOT NULL DEFAULT 1,
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2 NOT NULL,

  CONSTRAINT UQ_Code_ParentId UNIQUE (Code, ParentId)
);
```

**Manifests**

```sql
CREATE TABLE Manifests (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  OrganizationId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Organizations(Id),
  Name NVARCHAR(255) NOT NULL,
  Description NVARCHAR(MAX),
  Version NVARCHAR(20) NOT NULL, -- "1.0.0"
  Status INT NOT NULL, -- 0=Draft, 1=Published, 2=Archived
  Content NVARCHAR(MAX) NOT NULL, -- JSON
  PublishedAt DATETIME2 NULL,
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2 NOT NULL,

  CONSTRAINT UQ_Manifest_OrgVersion UNIQUE (OrganizationId, Version)
);

CREATE INDEX IX_Manifest_OrgStatus ON Manifests(OrganizationId, Status);
```

### Preventing Content Drift for Learners

**Problem**: Admin publishes manifest v2 → learner mid-session sees content change.

**Solution**: Learner sessions pin a specific manifest version snapshot

- When a learner session starts, it stores the `ManifestId` + `PublishedAt` timestamp
- Training app calls `GET /api/manifests/published/{orgCode}` → receives versioned snapshot
- If admin publishes a new manifest, that version is archived; learner session continues with old snapshot
- Schema: learner session has `ManifestSnapshotId` (immutable reference)

**Implementation**:

- Published manifests are never updated (INSERT only, set `PublishedAt` once)
- New manifests get new Version number and new `Id`
- Archive operation: update `Status = Archived` (not delete)
- Query for "current published": `WHERE OrganizationId = X AND Status = Published` (returns exactly 1 row)

---

## 5. Project Structure

```
LaerdalImplementation/
├── LaerdalImplementation.sln
├── src/
│   ├── LaerdalImplementation.Domain/
│   │   ├── Entities/
│   │   │   ├── Organization.cs
│   │   │   └── Manifest.cs
│   │   ├── Enums/
│   │   │   ├── OrganizationType.cs
│   │   │   └── ManifestStatus.cs
│   │   ├── ValueObjects/
│   │   │   └── ManifestVersion.cs
│   │   ├── Repositories/
│   │   │   ├── IOrganizationRepository.cs
│   │   │   └── IManifestRepository.cs
│   │   └── LaerdalImplementation.Domain.csproj
│   │
│   ├── LaerdalImplementation.Application/
│   │   ├── Commands/
│   │   │   ├── CreateOrganizationCommand.cs
│   │   │   ├── CreateOrganizationCommandHandler.cs
│   │   │   ├── PublishManifestCommand.cs
│   │   │   └── PublishManifestCommandHandler.cs
│   │   ├── Queries/
│   │   │   ├── GetOrganizationsQuery.cs
│   │   │   ├── GetOrganizationsQueryHandler.cs
│   │   │   ├── GetPublishedManifestQuery.cs
│   │   │   └── GetPublishedManifestQueryHandler.cs
│   │   ├── DTOs/
│   │   │   ├── CreateOrganizationDto.cs
│   │   │   ├── OrganizationDto.cs
│   │   │   └── ManifestDto.cs
│   │   ├── Services/
│   │   │   ├── AuthenticationService.cs
│   │   │   └── AuthorizationService.cs
│   │   ├── Mappers/
│   │   │   └── OrganizationMapper.cs
│   │   └── LaerdalImplementation.Application.csproj
│   │
│   ├── LaerdalImplementation.Infrastructure/
│   │   ├── Data/
│   │   │   ├── LaerdalDbContext.cs
│   │   │   └── Repositories/
│   │   │       ├── OrganizationRepository.cs
│   │   │       └── ManifestRepository.cs
│   │   ├── Configuration/
│   │   │   ├── OrganizationConfiguration.cs
│   │   │   └── ManifestConfiguration.cs
│   │   ├── Migrations/
│   │   │   ├── _InitialCreate.cs
│   │   │   └── _InitialCreate.Designer.cs
│   │   └── LaerdalImplementation.Infrastructure.csproj
│   │
│   └── LaerdalImplementation.Api/
│       ├── Controllers/
│       │   ├── OrganizationsController.cs
│       │   ├── ManifestsController.cs
│       │   └── AuthController.cs
│       ├── Models/
│       │   ├── CreateOrganizationRequest.cs
│       │   ├── OrganizationResponse.cs
│       │   └── ManifestResponse.cs
│       ├── Middleware/
│       │   └── AuthenticationMiddleware.cs
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── LaerdalImplementation.Api.csproj
│
├── tests/
│   └── LaerdalImplementation.Tests/   ← xUnit + Moq, 49 tests
├── README.md
└── design.md
```

---

## 6. What's Next / Future Work

- [ ] **Event sourcing**: Domain events → audit log + downstream messaging (PowerBI, training app webhooks)
- [ ] **Soft-delete strategy**: Refine cascading deletions for organizations with children
- [ ] **Manifest draft collaboration**: Concurrency control (optimistic locking via ETag)
- [ ] **Content validation**: Schema validation for manifest JSON (JSONSchema on POST)
- [ ] **Search & filter**: Full-text search on organization names, manifest content
- [ ] **Batch operations**: Bulk org creation from CSV, bulk manifest publish
- [ ] **Read models / CQRS**: Denormalized org hierarchy table for fast queries (if needed)
- [ ] **Integration tests**: `WebApplicationFactory` tests for the full HTTP → DB round trip (unit tests for domain and handlers already in place)
- [ ] **API versioning**: `/api/v1/` prefix; support `/api/v2/` endpoints alongside for non-breaking evolution
