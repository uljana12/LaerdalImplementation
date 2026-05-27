# Laerdal Platform — Frontend Design Spec

**Date:** 2026-05-27  
**Target:** React + TypeScript SPA alongside the existing C# / .NET 10 backend  
**Goal:** Demonstrate full-stack capability for a Frontend Engineer application at Laerdal Copenhagen — connects the existing Implementer backend to a polished React UI and adds a CPR Finder mock section that signals product thinking aligned with the job role.

---

## 1. Summary

A single React app living at `frontend/` inside the monorepo. It has two top-level sections united by a shared Laerdal design shell:

| Section         | Data source                        | Purpose                                                                             |
| --------------- | ---------------------------------- | ----------------------------------------------------------------------------------- |
| **Implementer** | Real API (`http://localhost:5000`) | Org hierarchy management + manifest lifecycle                                       |
| **CPR Finder**  | Mock / static data                 | Discovery, booking, and certification UI that mirrors the actual CPR Finder product |

The hiring manager sees one cohesive product: the backend they just reviewed, wrapped in a professional React UI, with a second section showing you have thought about the user-facing product they are currently building.

---

## 2. Tech Stack

| Concern       | Choice                           | Reason                                                       |
| ------------- | -------------------------------- | ------------------------------------------------------------ |
| Bundler       | **Vite**                         | Fast HMR, zero-config for React + TS                         |
| Framework     | **React 18 + TypeScript**        | Matches Laerdal job spec exactly                             |
| Styling       | **Tailwind CSS**                 | Utility-first, easy to apply custom brand tokens             |
| Components    | **shadcn/ui**                    | Composable, unstyled-by-default, Tailwind-compatible         |
| Routing       | **React Router v6**              | Standard SPA routing                                         |
| Data fetching | **TanStack Query (React Query)** | Caching, loading/error states, refetch — right tool for REST |
| HTTP client   | **fetch** (native)               | No extra dependency needed                                   |

No Redux or global state store — TanStack Query handles server state; local UI state lives in component hooks.

---

## 3. Visual Design System

### Colours (Tailwind CSS variables)

```
--laerdal-navy:    #0D1B2A   (topbar, sidebar background)
--laerdal-card:    #101e2e   (sidebar hover, nested cards)
--laerdal-red:     #C8102E   (primary CTA, active nav indicator, accent bar)
--laerdal-red-dark:#A00D25   (hover state for red buttons)
--content-bg:      #f0f4f8   (main content area background)
--surface:         #ffffff   (cards, tables, panels)
--border:          #e5eaef   (dividers, card borders)
--text-primary:    #1a2a3a
--text-secondary:  #8a9faf
--text-dim:        #5a7a90
```

### Theme: Hybrid

- **Dark** topbar + sidebar (`--laerdal-navy` / `--laerdal-card`)
- **Light** main content area (`--content-bg` / `--surface`)
- Red left-border `2px solid --laerdal-red` on active nav item
- Red `3px` accent bar below every page title

### Typography

- Font: `Inter` (Google Fonts) — clean, modern, professional
- Page titles: `text-xl font-bold text-primary`
- Labels/chips: `text-xs font-semibold uppercase tracking-widest`

---

## 4. App Shell

### Layout

```
┌──────────────────────────────────────────────┐
│  TOPBAR (dark navy, full width, h-12)        │
│  [L] Laerdal Platform    [Admin pill] [UH]   │
├──────────────┬───────────────────────────────┤
│              │                               │
│   SIDEBAR    │       MAIN CONTENT            │
│   (dark)     │       (light)                 │
│   w-52       │       flex-1                  │
│              │                               │
└──────────────┴───────────────────────────────┘
```

### Sidebar navigation

```
── IMPLEMENTER ──
  🏢 Organizations        [active = red left border]
  📋 Manifests            [badge: draft count]

── CPR FINDER ──
  📍 Discover Stations
  📅 Book a Session
  🎓 Certifications
```

### Routes

```
/                               → redirect to /implementer/organizations
/implementer/organizations      → Organizations list page
/implementer/organizations/:id  → Org detail (slide-over panel, not a full page)
/implementer/manifests          → Manifests list page
/cpr-finder/discover            → Discover stations
/cpr-finder/book                → Booking flow wizard
/cpr-finder/certifications      → Certifications list
```

---

## 5. Implementer Section — Organizations Page

**Data:** `GET /api/organizations` → real API

### Components

- **StatsRow** — 4 stat cards: Total / Hospitals / Departments / Active. Values derived from the org list response.
- **OrgTree** — table with inline hierarchy. Each row shows: expand/collapse button (if has children), name (indented by depth), code chip, type badge, status dot, edit + delete icon buttons.
- **CreateOrgPanel** — shadcn `Sheet` (slide-over from the right). Form fields: Name, Code, Type (select), Parent (optional select populated from org list). Calls `POST /api/organizations`. On success: refetches org list, closes panel, shows a toast.
- **EditOrgPanel** — same Sheet, pre-populated. Calls `PATCH /api/organizations/{id}`.
- **DeleteOrgDialog** — shadcn `AlertDialog`. If org has active children, shows a blocking message: "Remove all child organizations first." Otherwise calls `DELETE /api/organizations/{id}` and shows success toast.

### Error handling

- API error → shadcn `Toast` with error message
- 400 from delete (active children) → Dialog explains which children are blocking
- Network offline → Toast "Could not reach the server"

---

## 6. Implementer Section — Manifests Page

**Data:** `GET /api/organizations/{orgId}/manifests` — endpoints currently return 501. UI renders gracefully with a "Coming soon" empty state for write operations; the list UI is fully built so it lights up immediately when the handler is wired up.

### Components

- **OrgSelector** — dropdown to pick which org's manifests to view
- **ManifestList** — table with: name, version chip, status badge (Draft/Published/Archived with colour coding), createdAt, actions
- **StatusBadge** colours: Draft = blue, Published = green, Archived = grey
- **CreateManifestPanel** — slide-over Sheet with Name + Description fields
- **PublishDialog** — shadcn Dialog asking for version bump (Major / Minor / Patch radio). Shows current version → new version preview. Calls `POST .../publish`.
- **EmptyState** — "No manifests yet. Create a draft to get started." with a CTA button

---

## 7. CPR Finder Section — Discover Page

**Data:** Mock — static TypeScript array of 8 stations

### Station data shape

```ts
interface Station {
  id: string;
  name: string;
  address: string;
  distanceKm: number;
  slotsAvailable: number; // 0 = fully booked
  nextSlot: string; // "Today 14:30" | "Tomorrow 09:00" | "Jun 3rd"
  certifications: string[]; // ["BLS", "AED"]
  rating: number;
  mapGradient: string; // Tailwind gradient classes for card header
}
```

### Components

- **SearchBar** — location input + filter pills (CPR Only, This week, Rating 4.5+)
- **MissionBanner** — "One Million Lives by 2030" — references Laerdal's actual mission
- **StationCard** — gradient map header, distance badge, station name, address, availability badge, rating, next slot, Book button (→ `/cpr-finder/book?station={id}`)
- Fully booked stations show "Waitlist" button instead

---

## 8. CPR Finder Section — Book a Session

**Data:** Mock — multi-step wizard, no real API

### Steps

1. **Station confirmed** — shows selected station card (read-only recap)
2. **Pick a date** — calendar component (shadcn `Calendar`), highlights available dates in green
3. **Pick a time slot** — button grid of available times (e.g. 09:00, 10:30, 14:00)
4. **Confirm** — summary card + "Confirm Booking" primary button
5. **Success** — green checkmark, "You're booked!", "Add to calendar" link, "View Certifications" link

### State

Wizard step stored in `useState`. Selections passed forward via local state. No API call — clicking Confirm navigates to success screen.

---

## 9. CPR Finder Section — Certifications

**Data:** Mock — static list of 2 completed certifications

Simple list page:

- Each row: cert name (BLS / AED), issuing station, date completed, expiry date, status badge (Valid / Expiring Soon / Expired), "Download PDF" button (no-op, shows toast "PDF download coming soon")

---

## 10. Backend CORS

The C# API needs one addition before the frontend can call it:

```csharp
// Program.cs — add before app.Build()
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// after app.Build()
app.UseCors();
```

`5173` is Vite's default dev port.

---

## 11. Project Structure

```
frontend/
├── index.html
├── vite.config.ts
├── tailwind.config.ts
├── tsconfig.json
├── package.json
└── src/
    ├── main.tsx                    ← React root, Router, QueryClientProvider
    ├── App.tsx                     ← Shell layout (topbar + sidebar + <Outlet>)
    ├── design-system/
    │   └── tokens.ts               ← Tailwind colour token constants
    ├── components/
    │   ├── layout/
    │   │   ├── Topbar.tsx
    │   │   ├── Sidebar.tsx
    │   │   └── PageHeader.tsx      ← title + red accent bar
    │   └── ui/                     ← shadcn/ui generated components
    ├── features/
    │   ├── organizations/
    │   │   ├── api.ts              ← fetch wrappers for org endpoints
    │   │   ├── OrgTree.tsx
    │   │   ├── OrgStatsRow.tsx
    │   │   ├── CreateOrgPanel.tsx
    │   │   ├── EditOrgPanel.tsx
    │   │   └── DeleteOrgDialog.tsx
    │   ├── manifests/
    │   │   ├── api.ts
    │   │   ├── ManifestList.tsx
    │   │   ├── CreateManifestPanel.tsx
    │   │   └── PublishDialog.tsx
    │   ├── cpr-discover/
    │   │   ├── mockData.ts
    │   │   ├── DiscoverPage.tsx
    │   │   ├── StationCard.tsx
    │   │   └── MissionBanner.tsx
    │   ├── cpr-book/
    │   │   ├── BookingWizard.tsx
    │   │   └── steps/
    │   │       ├── StepStation.tsx
    │   │       ├── StepDate.tsx
    │   │       ├── StepTime.tsx
    │   │       ├── StepConfirm.tsx
    │   │       └── StepSuccess.tsx
    │   └── cpr-certifications/
    │       ├── mockData.ts
    │       └── CertificationsPage.tsx
    └── pages/
        ├── OrganizationsPage.tsx
        ├── ManifestsPage.tsx
        ├── DiscoverPage.tsx
        ├── BookPage.tsx
        └── CertificationsPage.tsx
```

---

## 12. Scope for 1–2 Days

### Day 1

- [ ] Scaffold Vite + React + TS + Tailwind + shadcn/ui
- [ ] App shell: topbar, sidebar, routing
- [ ] Add CORS to C# API
- [ ] Organizations page: list + tree + stats (real API)
- [ ] Create org panel + delete dialog (real API)

### Day 2

- [ ] Manifests page: list + status badges + publish dialog
- [ ] CPR Finder: Discover page (mock data)
- [ ] CPR Finder: Booking wizard (mock)
- [ ] CPR Finder: Certifications (mock)
- [ ] Polish: transitions, toasts, empty states, responsive sidebar

### Out of scope

- Auth / login screen (backend is also stubbed; consistent to leave both out)
- Mobile / React Native (that's a follow-up conversation in the interview)
- Unit tests for the frontend (mention as next step, same as backend integration tests)
