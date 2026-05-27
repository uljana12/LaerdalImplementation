# Laerdal Platform — Full-Stack Demo

A full-stack demo built to showcase proficiency with the Laerdal tech stack. Includes a C# .NET 10 backend and a React + TypeScript frontend — built as a follow-up to the Backend Developer interview and as part of an application for the Frontend Developer role.

## Live demo

🌐 **[laerdal-implementation.vercel.app](https://laerdal-implementation.vercel.app)** *(CPR Finder section fully interactive — Implementer section requires the local API)*

---

## What's inside

### Implementer section (wired to real C# API)
- **Organizations** — recursive tree (Hospital → Department → Training Center), full CRUD, active-children delete guard, stat cards
- **Manifests** — versioned course bundles, status badges (Draft / Published / Archived), create draft panel, publish wizard with major / minor / patch version bump

### CPR Finder section (mock data)
- **Discover** — station cards with distance, slot availability, ratings, search filter, and Laerdal's "One Million Lives" mission banner
- **Book a Session** — 5-step wizard: Station → Date → Time → Confirm → Success
- **Certifications** — table with valid / expiring-soon / expired status badges

---

## Tech stack

| Layer | Technology |
|---|---|
| Frontend | React 18, TypeScript, Vite 5 |
| Styling | Tailwind CSS v3 + shadcn/ui |
| Routing | React Router v6 |
| Data fetching | TanStack Query v5 |
| Backend | C# .NET 10, ASP.NET Core minimal API |
| Database | SQLite (EF Core) |

---

## Running locally

### 1. Backend (C# API)

```bash
dotnet run --project src/LaerdalImplementation.Api/LaerdalImplementation.Api.csproj
# API starts at http://localhost:5000
# API docs at http://localhost:5000/scalar/v1
```

### 2. Frontend (React)

```bash
cd frontend
npm install
npm run dev
# App starts at http://localhost:5173
```

Open **http://localhost:5173**. The Implementer section talks to the live API; the CPR Finder section runs on mock data with no backend needed.

---

## Project structure

```
LaerdalImplementation/
├── src/
│   └── LaerdalImplementation.Api/   # C# .NET 10 backend
│       ├── Endpoints/               # Organizations + Manifests
│       ├── Models/                  # EF Core entities
│       └── Program.cs
├── frontend/                        # React + TypeScript SPA
│   └── src/
│       ├── features/
│       │   ├── organizations/       # Tree, CRUD panels, API client
│       │   ├── manifests/           # List, create, publish dialog
│       │   ├── cpr-discover/        # Station cards, mock data
│       │   ├── cpr-book/            # 5-step booking wizard
│       │   └── cpr-certifications/  # Cert table, mock data
│       └── pages/
└── docs/
    └── superpowers/
        ├── specs/                   # Design document
        └── plans/                   # Implementation plan
```

---

## Backend design decisions

1. **Business rules in the domain** — `Organization.CanBeDeleted()`, `Manifest.Publish()` enforce invariants at the entity level
2. **Immutable published manifests** — once published, never modified; new edits create a new version
3. **Soft delete** — orgs set `IsActive = false` to preserve audit trails and foreign key integrity
4. **CQRS via MediatR** — commands and queries are separated; handlers are independently testable
5. **48 passing tests** — domain entities, value objects, all command/query handlers

---

*Built by Ulyana Hassan — [uljana12@gmail.com](mailto:uljana12@gmail.com)*
