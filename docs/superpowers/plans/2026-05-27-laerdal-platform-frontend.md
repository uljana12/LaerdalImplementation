# Laerdal Platform Frontend Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a React + TypeScript SPA at `frontend/` that wraps the existing C# API in a polished Laerdal-branded UI (Implementer section) and adds a CPR Finder mock section (Discover, Book, Certifications).

**Architecture:** Hybrid dark-sidebar / light-content shell with React Router v6. Two feature areas share one app shell: Implementer (real API via TanStack Query) and CPR Finder (static mock data). shadcn/ui components styled with Tailwind CSS and Laerdal brand tokens.

**Tech Stack:** Vite 5, React 18, TypeScript 5, Tailwind CSS 3, shadcn/ui, React Router v6, TanStack Query v5

---

## File Map

### Created
```
frontend/
├── index.html
├── vite.config.ts
├── tailwind.config.ts
├── postcss.config.js
├── tsconfig.json
├── tsconfig.node.json
├── package.json
├── components.json                         ← shadcn config
└── src/
    ├── main.tsx
    ├── App.tsx
    ├── lib/
    │   └── utils.ts                        ← shadcn cn() helper
    ├── components/
    │   ├── layout/
    │   │   ├── Topbar.tsx
    │   │   ├── Sidebar.tsx
    │   │   └── PageHeader.tsx
    │   └── ui/                             ← shadcn generated (do not hand-edit)
    ├── features/
    │   ├── organizations/
    │   │   ├── types.ts
    │   │   ├── api.ts
    │   │   ├── OrgStatsRow.tsx
    │   │   ├── OrgTree.tsx
    │   │   ├── CreateOrgPanel.tsx
    │   │   ├── EditOrgPanel.tsx
    │   │   └── DeleteOrgDialog.tsx
    │   ├── manifests/
    │   │   ├── types.ts
    │   │   ├── api.ts
    │   │   ├── OrgSelector.tsx
    │   │   ├── StatusBadge.tsx
    │   │   ├── ManifestList.tsx
    │   │   ├── CreateManifestPanel.tsx
    │   │   └── PublishDialog.tsx
    │   ├── cpr-discover/
    │   │   ├── mockData.ts
    │   │   ├── MissionBanner.tsx
    │   │   ├── StationCard.tsx
    │   │   └── DiscoverPage.tsx
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

### Modified
```
src/LaerdalImplementation.Api/Program.cs    ← CORS already configured, no change needed
```

---

## Task 1: Scaffold Vite + React + TypeScript project

**Files:**
- Create: `frontend/package.json`
- Create: `frontend/vite.config.ts`
- Create: `frontend/tsconfig.json`
- Create: `frontend/tsconfig.node.json`
- Create: `frontend/index.html`

- [ ] **Step 1: Create the frontend directory and initialise the project**

```bash
cd /Users/ulyanahassan/VScodeProjects/LaerdalImplementation
npm create vite@latest frontend -- --template react-ts
cd frontend
```

- [ ] **Step 2: Install all dependencies**

```bash
npm install
npm install react-router-dom @tanstack/react-query
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

- [ ] **Step 3: Install shadcn/ui prerequisites**

```bash
npm install class-variance-authority clsx tailwind-merge lucide-react
npm install @radix-ui/react-dialog @radix-ui/react-select @radix-ui/react-slot
npm install @radix-ui/react-toast @radix-ui/react-alert-dialog @radix-ui/react-label
npm install @radix-ui/react-separator @radix-ui/react-calendar
npm install date-fns
```

- [ ] **Step 4: Replace `frontend/vite.config.ts` with**

```ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 5173,
  },
})
```

- [ ] **Step 5: Replace `frontend/tsconfig.json` with**

```json
{
  "files": [],
  "references": [
    { "path": "./tsconfig.app.json" },
    { "path": "./tsconfig.node.json" }
  ]
}
```

- [ ] **Step 6: Create `frontend/tsconfig.app.json`**

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["src"]
}
```

- [ ] **Step 7: Verify the dev server starts**

```bash
cd frontend && npm run dev
```

Expected: `VITE v5.x  ready in Xms  ➜  Local: http://localhost:5173/`

- [ ] **Step 8: Commit**

```bash
cd .. && git init   # only if not already a git repo
git add frontend/
git commit -m "feat: scaffold Vite + React + TypeScript frontend"
```

---

## Task 2: Configure Tailwind with Laerdal brand tokens

**Files:**
- Create: `frontend/tailwind.config.ts`
- Create: `frontend/src/lib/utils.ts`
- Create: `frontend/components.json`

- [ ] **Step 1: Replace `frontend/tailwind.config.ts` with**

```ts
import type { Config } from 'tailwindcss'

const config: Config = {
  darkMode: ['class'],
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        laerdal: {
          navy:     '#0D1B2A',
          card:     '#101e2e',
          red:      '#C8102E',
          'red-dark': '#A00D25',
        },
        content: {
          bg:      '#f0f4f8',
          surface: '#ffffff',
          border:  '#e5eaef',
        },
        text: {
          primary:   '#1a2a3a',
          secondary: '#8a9faf',
          dim:       '#5a7a90',
        },
      },
      fontFamily: {
        sans: ['Inter', 'Segoe UI', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}

export default config
```

- [ ] **Step 2: Replace `frontend/src/index.css` with**

```css
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');
@tailwind base;
@tailwind components;
@tailwind utilities;

@layer base {
  * { box-sizing: border-box; }
  body { margin: 0; font-family: 'Inter', system-ui, sans-serif; }
}
```

- [ ] **Step 3: Create `frontend/src/lib/utils.ts`**

```ts
import { type ClassValue, clsx } from 'clsx'
import { twMerge } from 'tailwind-merge'

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}
```

- [ ] **Step 4: Create `frontend/components.json` (shadcn config)**

```json
{
  "$schema": "https://ui.shadcn.com/schema.json",
  "style": "default",
  "rsc": false,
  "tsx": true,
  "tailwind": {
    "config": "tailwind.config.ts",
    "css": "src/index.css",
    "baseColor": "slate",
    "cssVariables": false,
    "prefix": ""
  },
  "aliases": {
    "components": "@/components",
    "utils": "@/lib/utils"
  }
}
```

- [ ] **Step 5: Add shadcn Button component**

```bash
cd frontend && npx shadcn@latest add button --yes
```

- [ ] **Step 6: Add remaining shadcn components**

```bash
npx shadcn@latest add sheet --yes
npx shadcn@latest add dialog --yes
npx shadcn@latest add alert-dialog --yes
npx shadcn@latest add select --yes
npx shadcn@latest add input --yes
npx shadcn@latest add label --yes
npx shadcn@latest add toast --yes
npx shadcn@latest add badge --yes
npx shadcn@latest add separator --yes
npx shadcn@latest add calendar --yes
npx shadcn@latest add radio-group --yes
```

- [ ] **Step 7: Verify build compiles without errors**

```bash
npm run build
```

Expected: output shows `dist/` built with no TypeScript errors.

- [ ] **Step 8: Commit**

```bash
cd .. && git add frontend/
git commit -m "feat: configure Tailwind brand tokens and install shadcn/ui"
```

---

## Task 3: App shell — Topbar, Sidebar, Router

**Files:**
- Create: `frontend/src/components/layout/Topbar.tsx`
- Create: `frontend/src/components/layout/Sidebar.tsx`
- Create: `frontend/src/components/layout/PageHeader.tsx`
- Create: `frontend/src/App.tsx`
- Create: `frontend/src/main.tsx`

- [ ] **Step 1: Create `frontend/src/components/layout/Topbar.tsx`**

```tsx
export function Topbar() {
  return (
    <header className="h-12 bg-laerdal-navy border-b border-white/[0.07] flex items-center px-5 gap-3 flex-shrink-0">
      <div className="w-7 h-7 bg-laerdal-red rounded flex items-center justify-center font-black text-sm text-white">
        L
      </div>
      <span className="font-bold text-white text-sm tracking-tight">
        Laerdal <span className="text-laerdal-red">Platform</span>
      </span>
      <div className="flex-1" />
      <span className="text-xs bg-laerdal-red/10 border border-laerdal-red/25 text-red-300 px-3 py-1 rounded-full">
        Laerdal Admin
      </span>
      <div className="w-7 h-7 bg-[#1e3a5f] rounded-full flex items-center justify-center text-xs font-bold text-[#7ec8e3]">
        UH
      </div>
    </header>
  )
}
```

- [ ] **Step 2: Create `frontend/src/components/layout/Sidebar.tsx`**

```tsx
import { NavLink } from 'react-router-dom'
import { cn } from '@/lib/utils'

const NAV = [
  {
    section: 'Implementer',
    items: [
      { to: '/implementer/organizations', icon: '🏢', label: 'Organizations' },
      { to: '/implementer/manifests',     icon: '📋', label: 'Manifests' },
    ],
  },
  {
    section: 'CPR Finder',
    items: [
      { to: '/cpr-finder/discover',        icon: '📍', label: 'Discover' },
      { to: '/cpr-finder/book',            icon: '📅', label: 'Book a Session' },
      { to: '/cpr-finder/certifications',  icon: '🎓', label: 'Certifications' },
    ],
  },
]

export function Sidebar() {
  return (
    <aside className="w-52 bg-laerdal-card border-r border-white/[0.05] flex flex-col py-3 flex-shrink-0">
      {NAV.map((group, gi) => (
        <div key={group.section}>
          {gi > 0 && <hr className="border-white/[0.05] my-2" />}
          <p className="text-[10px] font-bold uppercase tracking-widest text-white/25 px-4 py-2">
            {group.section}
          </p>
          {group.items.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                cn(
                  'flex items-center gap-2 px-4 py-2 text-[13px] border-l-2 transition-colors',
                  isActive
                    ? 'text-white bg-laerdal-red/10 border-laerdal-red'
                    : 'text-text-dim border-transparent hover:text-white hover:bg-white/[0.04]'
                )
              }
            >
              <span className="w-4 text-center">{item.icon}</span>
              {item.label}
            </NavLink>
          ))}
        </div>
      ))}
    </aside>
  )
}
```

- [ ] **Step 3: Create `frontend/src/components/layout/PageHeader.tsx`**

```tsx
interface PageHeaderProps {
  title: string
  subtitle?: string
  action?: React.ReactNode
}

export function PageHeader({ title, subtitle, action }: PageHeaderProps) {
  return (
    <div className="bg-white border-b border-content-border px-6 py-4 flex items-start justify-between flex-shrink-0">
      <div>
        <h1 className="text-xl font-bold text-text-primary">{title}</h1>
        {subtitle && <p className="text-xs text-text-secondary mt-1">{subtitle}</p>}
        <div className="w-9 h-[3px] bg-laerdal-red rounded mt-2" />
      </div>
      {action && <div className="mt-1">{action}</div>}
    </div>
  )
}
```

- [ ] **Step 4: Replace `frontend/src/App.tsx` with**

```tsx
import { Navigate, Outlet, Route, Routes } from 'react-router-dom'
import { Topbar } from '@/components/layout/Topbar'
import { Sidebar } from '@/components/layout/Sidebar'
import { OrganizationsPage } from '@/pages/OrganizationsPage'
import { ManifestsPage }     from '@/pages/ManifestsPage'
import { DiscoverPage }       from '@/pages/DiscoverPage'
import { BookPage }           from '@/pages/BookPage'
import { CertificationsPage } from '@/pages/CertificationsPage'

function Shell() {
  return (
    <div className="h-screen flex flex-col bg-laerdal-navy">
      <Topbar />
      <div className="flex flex-1 overflow-hidden">
        <Sidebar />
        <main className="flex-1 overflow-y-auto bg-content-bg flex flex-col">
          <Outlet />
        </main>
      </div>
    </div>
  )
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Shell />}>
        <Route index element={<Navigate to="/implementer/organizations" replace />} />
        <Route path="implementer/organizations" element={<OrganizationsPage />} />
        <Route path="implementer/manifests"     element={<ManifestsPage />} />
        <Route path="cpr-finder/discover"       element={<DiscoverPage />} />
        <Route path="cpr-finder/book"           element={<BookPage />} />
        <Route path="cpr-finder/certifications" element={<CertificationsPage />} />
      </Route>
    </Routes>
  )
}
```

- [ ] **Step 5: Replace `frontend/src/main.tsx` with**

```tsx
import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import App from './App'
import './index.css'

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 30_000 } },
})

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <App />
      </QueryClientProvider>
    </BrowserRouter>
  </React.StrictMode>
)
```

- [ ] **Step 6: Create stub pages so imports don't break**

Create `frontend/src/pages/OrganizationsPage.tsx`:
```tsx
export function OrganizationsPage() {
  return <div className="p-6 text-text-primary">Organizations — coming soon</div>
}
```

Repeat for `ManifestsPage.tsx`, `DiscoverPage.tsx`, `BookPage.tsx`, `CertificationsPage.tsx` — same pattern, different label.

- [ ] **Step 7: Verify shell renders**

```bash
cd frontend && npm run dev
```

Open http://localhost:5173 — you should see the dark topbar, dark sidebar with two sections, and the stub page text in the light content area. Clicking nav items should change the active red-border highlight.

- [ ] **Step 8: Commit**

```bash
cd .. && git add frontend/
git commit -m "feat: app shell with topbar, sidebar, and React Router"
```

---

## Task 4: Organization types and API client

**Files:**
- Create: `frontend/src/features/organizations/types.ts`
- Create: `frontend/src/features/organizations/api.ts`

- [ ] **Step 1: Create `frontend/src/features/organizations/types.ts`**

```ts
export type OrganizationType = 0 | 1 | 2  // 0=Hospital, 1=Department, 2=TrainingCenter

export interface Organization {
  id: string
  name: string
  code: string
  type: OrganizationType
  parentId: string | null
  isActive: boolean
  externalId: string | null
  createdAt: string
  updatedAt: string
  children: Organization[]
}

export interface CreateOrganizationRequest {
  name: string
  code: string
  type: OrganizationType
  parentId?: string
}

export interface UpdateOrganizationRequest {
  name?: string
  type?: OrganizationType
  isActive?: boolean
}

export const ORG_TYPE_LABEL: Record<OrganizationType, string> = {
  0: 'Hospital',
  1: 'Department',
  2: 'Training Center',
}

export const ORG_TYPE_COLOUR: Record<OrganizationType, string> = {
  0: 'bg-blue-100 text-blue-700',
  1: 'bg-green-100 text-green-700',
  2: 'bg-amber-100 text-amber-700',
}
```

- [ ] **Step 2: Create `frontend/src/features/organizations/api.ts`**

```ts
import type {
  Organization,
  CreateOrganizationRequest,
  UpdateOrganizationRequest,
} from './types'

const BASE = 'http://localhost:5000/api'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json', ...init?.headers },
    ...init,
  })
  if (!res.ok) {
    const text = await res.text().catch(() => res.statusText)
    throw new Error(text || `HTTP ${res.status}`)
  }
  if (res.status === 204) return undefined as T
  return res.json()
}

export const orgApi = {
  list: ()                                     => request<Organization[]>('/organizations'),
  getById: (id: string)                        => request<Organization>(`/organizations/${id}`),
  create: (body: CreateOrganizationRequest)    => request<Organization>('/organizations', { method: 'POST', body: JSON.stringify(body) }),
  update: (id: string, body: UpdateOrganizationRequest) => request<Organization>(`/organizations/${id}`, { method: 'PATCH', body: JSON.stringify(body) }),
  delete: (id: string)                         => request<void>(`/organizations/${id}`, { method: 'DELETE' }),
}
```

- [ ] **Step 3: Verify the API client reaches the real backend**

Make sure the C# API is running (`dotnet run --project src/LaerdalImplementation.Api`), then in the browser console at http://localhost:5173 run:

```js
fetch('http://localhost:5000/api/organizations').then(r => r.json()).then(console.log)
```

Expected: array of organizations (or `[]` if DB is empty). No CORS errors.

- [ ] **Step 4: Commit**

```bash
git add frontend/src/features/organizations/
git commit -m "feat: organization types and API client"
```

---

## Task 5: OrgStatsRow component

**Files:**
- Create: `frontend/src/features/organizations/OrgStatsRow.tsx`

- [ ] **Step 1: Create `frontend/src/features/organizations/OrgStatsRow.tsx`**

```tsx
import type { Organization } from './types'

interface Props { orgs: Organization[] }

export function OrgStatsRow({ orgs }: Props) {
  const flat = flattenOrgs(orgs)
  const hospitals   = flat.filter(o => o.type === 0).length
  const departments = flat.filter(o => o.type === 1 || o.type === 2).length
  const active      = flat.filter(o => o.isActive).length

  return (
    <div className="grid grid-cols-4 gap-3 p-6 pb-3">
      {[
        { label: 'Total',       value: flat.length,  sub: 'organizations' },
        { label: 'Hospitals',   value: hospitals,    sub: 'root level' },
        { label: 'Departments', value: departments,  sub: 'children' },
        { label: 'Active',      value: active,       sub: `${flat.length - active} soft-deleted` },
      ].map(({ label, value, sub }) => (
        <div key={label} className="bg-white rounded-lg border border-content-border px-4 py-3">
          <p className="text-[10px] font-semibold uppercase tracking-widest text-text-secondary mb-1">
            {label}
          </p>
          <p className="text-2xl font-bold text-text-primary">{value}</p>
          <p className="text-[11px] text-text-secondary mt-0.5">{sub}</p>
        </div>
      ))}
    </div>
  )
}

function flattenOrgs(orgs: Organization[]): Organization[] {
  return orgs.flatMap(o => [o, ...flattenOrgs(o.children ?? [])])
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/features/organizations/OrgStatsRow.tsx
git commit -m "feat: OrgStatsRow stat cards"
```

---

## Task 6: OrgTree component

**Files:**
- Create: `frontend/src/features/organizations/OrgTree.tsx`

- [ ] **Step 1: Create `frontend/src/features/organizations/OrgTree.tsx`**

```tsx
import { useState } from 'react'
import { Pencil, Trash2, ChevronRight, ChevronDown } from 'lucide-react'
import { Button } from '@/components/ui/button'
import type { Organization } from './types'
import { ORG_TYPE_LABEL, ORG_TYPE_COLOUR } from './types'

interface Props {
  orgs: Organization[]
  onEdit:   (org: Organization) => void
  onDelete: (org: Organization) => void
  depth?: number
}

export function OrgTree({ orgs, onEdit, onDelete, depth = 0 }: Props) {
  return (
    <>
      {orgs.map(org => (
        <OrgRow
          key={org.id}
          org={org}
          onEdit={onEdit}
          onDelete={onDelete}
          depth={depth}
        />
      ))}
    </>
  )
}

function OrgRow({
  org, onEdit, onDelete, depth,
}: {
  org: Organization; onEdit: (o: Organization) => void; onDelete: (o: Organization) => void; depth: number
}) {
  const [open, setOpen] = useState(depth === 0)
  const hasChildren = (org.children ?? []).length > 0

  return (
    <>
      <div
        className="grid items-center border-b border-content-border/60 last:border-0 hover:bg-slate-50 transition-colors"
        style={{ gridTemplateColumns: '1fr 90px 130px 100px 80px' }}
      >
        <div className="flex items-center gap-2 py-2.5 px-4" style={{ paddingLeft: `${16 + depth * 20}px` }}>
          {hasChildren ? (
            <button
              onClick={() => setOpen(o => !o)}
              className="w-5 h-5 rounded bg-slate-100 flex items-center justify-center text-text-dim hover:bg-slate-200 flex-shrink-0"
            >
              {open
                ? <ChevronDown className="w-3 h-3" />
                : <ChevronRight className="w-3 h-3" />}
            </button>
          ) : (
            <span className="w-5 flex-shrink-0" />
          )}
          <span className="text-sm font-medium text-text-primary">{org.name}</span>
        </div>

        <div className="py-2.5 px-2">
          <span className="font-mono text-xs text-text-dim">{org.code}</span>
        </div>

        <div className="py-2.5 px-2">
          <span className={`text-[11px] font-semibold px-2 py-0.5 rounded ${ORG_TYPE_COLOUR[org.type]}`}>
            {ORG_TYPE_LABEL[org.type]}
          </span>
        </div>

        <div className="py-2.5 px-2 flex items-center gap-1.5">
          <span className={`w-1.5 h-1.5 rounded-full flex-shrink-0 ${org.isActive ? 'bg-green-500' : 'bg-slate-300'}`} />
          <span className="text-xs text-text-secondary">{org.isActive ? 'Active' : 'Inactive'}</span>
        </div>

        <div className="py-2.5 px-2 flex items-center gap-1">
          <Button
            variant="outline" size="icon"
            className="w-7 h-7 border-content-border"
            onClick={() => onEdit(org)}
          >
            <Pencil className="w-3 h-3" />
          </Button>
          <Button
            variant="outline" size="icon"
            className="w-7 h-7 border-content-border hover:bg-red-50 hover:text-laerdal-red hover:border-red-200"
            onClick={() => onDelete(org)}
          >
            <Trash2 className="w-3 h-3" />
          </Button>
        </div>
      </div>

      {open && hasChildren && (
        <OrgTree
          orgs={org.children}
          onEdit={onEdit}
          onDelete={onDelete}
          depth={depth + 1}
        />
      )}
    </>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/features/organizations/OrgTree.tsx
git commit -m "feat: OrgTree with expand/collapse hierarchy"
```

---

## Task 7: CreateOrgPanel and EditOrgPanel

**Files:**
- Create: `frontend/src/features/organizations/CreateOrgPanel.tsx`
- Create: `frontend/src/features/organizations/EditOrgPanel.tsx`

- [ ] **Step 1: Create `frontend/src/features/organizations/CreateOrgPanel.tsx`**

```tsx
import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { orgApi } from './api'
import type { Organization, OrganizationType } from './types'

interface Props {
  open: boolean
  onClose: () => void
  orgs: Organization[]
}

export function CreateOrgPanel({ open, onClose, orgs }: Props) {
  const qc = useQueryClient()
  const [name, setName]         = useState('')
  const [code, setCode]         = useState('')
  const [type, setType]         = useState<OrganizationType>(0)
  const [parentId, setParentId] = useState<string>('')
  const [error, setError]       = useState('')

  const mutation = useMutation({
    mutationFn: () => orgApi.create({
      name, code, type,
      ...(parentId ? { parentId } : {}),
    }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['organizations'] })
      setName(''); setCode(''); setType(0); setParentId(''); setError('')
      onClose()
    },
    onError: (e: Error) => setError(e.message),
  })

  return (
    <Sheet open={open} onOpenChange={v => !v && onClose()}>
      <SheetContent className="w-[400px] sm:max-w-[400px]">
        <SheetHeader>
          <SheetTitle>New Organization</SheetTitle>
        </SheetHeader>

        <div className="flex flex-col gap-4 mt-6">
          <div className="space-y-1.5">
            <Label>Name</Label>
            <Input value={name} onChange={e => setName(e.target.value)} placeholder="City Hospital" />
          </div>

          <div className="space-y-1.5">
            <Label>Code <span className="text-text-secondary text-xs">(short unique ID)</span></Label>
            <Input value={code} onChange={e => setCode(e.target.value.toUpperCase())} placeholder="CITY" />
          </div>

          <div className="space-y-1.5">
            <Label>Type</Label>
            <Select value={String(type)} onValueChange={v => setType(Number(v) as OrganizationType)}>
              <SelectTrigger><SelectValue /></SelectTrigger>
              <SelectContent>
                <SelectItem value="0">Hospital</SelectItem>
                <SelectItem value="1">Department</SelectItem>
                <SelectItem value="2">Training Center</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-1.5">
            <Label>Parent <span className="text-text-secondary text-xs">(optional — leave blank for root)</span></Label>
            <Select value={parentId} onValueChange={setParentId}>
              <SelectTrigger><SelectValue placeholder="No parent (root org)" /></SelectTrigger>
              <SelectContent>
                <SelectItem value="">No parent</SelectItem>
                {orgs.map(o => (
                  <SelectItem key={o.id} value={o.id}>{o.name}</SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {error && <p className="text-sm text-laerdal-red">{error}</p>}

          <Button
            className="bg-laerdal-red hover:bg-laerdal-red-dark text-white mt-2"
            disabled={!name || !code || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            {mutation.isPending ? 'Creating…' : 'Create Organization'}
          </Button>
        </div>
      </SheetContent>
    </Sheet>
  )
}
```

- [ ] **Step 2: Create `frontend/src/features/organizations/EditOrgPanel.tsx`**

```tsx
import { useEffect, useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { orgApi } from './api'
import type { Organization, OrganizationType } from './types'

interface Props {
  org: Organization | null
  onClose: () => void
}

export function EditOrgPanel({ org, onClose }: Props) {
  const qc = useQueryClient()
  const [name, setName] = useState('')
  const [type, setType] = useState<OrganizationType>(0)
  const [error, setError] = useState('')

  useEffect(() => {
    if (org) { setName(org.name); setType(org.type); setError('') }
  }, [org])

  const mutation = useMutation({
    mutationFn: () => orgApi.update(org!.id, { name, type }),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['organizations'] }); onClose() },
    onError: (e: Error) => setError(e.message),
  })

  return (
    <Sheet open={!!org} onOpenChange={v => !v && onClose()}>
      <SheetContent className="w-[400px] sm:max-w-[400px]">
        <SheetHeader><SheetTitle>Edit Organization</SheetTitle></SheetHeader>
        <div className="flex flex-col gap-4 mt-6">
          <div className="space-y-1.5">
            <Label>Name</Label>
            <Input value={name} onChange={e => setName(e.target.value)} />
          </div>
          <div className="space-y-1.5">
            <Label>Type</Label>
            <Select value={String(type)} onValueChange={v => setType(Number(v) as OrganizationType)}>
              <SelectTrigger><SelectValue /></SelectTrigger>
              <SelectContent>
                <SelectItem value="0">Hospital</SelectItem>
                <SelectItem value="1">Department</SelectItem>
                <SelectItem value="2">Training Center</SelectItem>
              </SelectContent>
            </Select>
          </div>
          {error && <p className="text-sm text-laerdal-red">{error}</p>}
          <Button
            className="bg-laerdal-red hover:bg-laerdal-red-dark text-white mt-2"
            disabled={!name || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            {mutation.isPending ? 'Saving…' : 'Save Changes'}
          </Button>
        </div>
      </SheetContent>
    </Sheet>
  )
}
```

- [ ] **Step 3: Commit**

```bash
git add frontend/src/features/organizations/
git commit -m "feat: CreateOrgPanel and EditOrgPanel slide-over sheets"
```

---

## Task 8: DeleteOrgDialog

**Files:**
- Create: `frontend/src/features/organizations/DeleteOrgDialog.tsx`

- [ ] **Step 1: Create `frontend/src/features/organizations/DeleteOrgDialog.tsx`**

```tsx
import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  AlertDialog, AlertDialogAction, AlertDialogCancel,
  AlertDialogContent, AlertDialogDescription,
  AlertDialogFooter, AlertDialogHeader, AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import { orgApi } from './api'
import type { Organization } from './types'

interface Props {
  org: Organization | null
  onClose: () => void
}

export function DeleteOrgDialog({ org, onClose }: Props) {
  const qc = useQueryClient()
  const hasActiveChildren = (org?.children ?? []).some(c => c.isActive)

  const mutation = useMutation({
    mutationFn: () => orgApi.delete(org!.id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['organizations'] }); onClose() },
  })

  return (
    <AlertDialog open={!!org} onOpenChange={v => !v && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete "{org?.name}"?</AlertDialogTitle>
          <AlertDialogDescription>
            {hasActiveChildren
              ? '⚠️ This organization has active child organizations. Remove or deactivate them first before deleting this one.'
              : 'This will soft-delete the organization (sets IsActive = false). Manifests and audit data are preserved.'}
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onClose}>Cancel</AlertDialogCancel>
          {!hasActiveChildren && (
            <AlertDialogAction
              className="bg-laerdal-red hover:bg-laerdal-red-dark text-white"
              onClick={() => mutation.mutate()}
              disabled={mutation.isPending}
            >
              {mutation.isPending ? 'Deleting…' : 'Delete'}
            </AlertDialogAction>
          )}
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/features/organizations/DeleteOrgDialog.tsx
git commit -m "feat: DeleteOrgDialog with active-children guard"
```

---

## Task 9: OrganizationsPage — assemble

**Files:**
- Modify: `frontend/src/pages/OrganizationsPage.tsx`

- [ ] **Step 1: Replace `frontend/src/pages/OrganizationsPage.tsx` with**

```tsx
import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Plus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { PageHeader } from '@/components/layout/PageHeader'
import { orgApi } from '@/features/organizations/api'
import { OrgStatsRow }      from '@/features/organizations/OrgStatsRow'
import { OrgTree }          from '@/features/organizations/OrgTree'
import { CreateOrgPanel }   from '@/features/organizations/CreateOrgPanel'
import { EditOrgPanel }     from '@/features/organizations/EditOrgPanel'
import { DeleteOrgDialog }  from '@/features/organizations/DeleteOrgDialog'
import type { Organization } from '@/features/organizations/types'

export function OrganizationsPage() {
  const { data: orgs = [], isLoading, error } = useQuery({
    queryKey: ['organizations'],
    queryFn: orgApi.list,
  })

  const [showCreate, setShowCreate]   = useState(false)
  const [editOrg, setEditOrg]         = useState<Organization | null>(null)
  const [deleteOrg, setDeleteOrg]     = useState<Organization | null>(null)

  return (
    <div className="flex flex-col flex-1">
      <PageHeader
        title="Organizations"
        subtitle="Manage hospitals, departments & training centers"
        action={
          <Button
            className="bg-laerdal-red hover:bg-laerdal-red-dark text-white gap-2"
            onClick={() => setShowCreate(true)}
          >
            <Plus className="w-4 h-4" /> New Organization
          </Button>
        }
      />

      {isLoading && <p className="p-6 text-text-secondary text-sm">Loading…</p>}
      {error   && <p className="p-6 text-laerdal-red text-sm">Could not reach the server.</p>}

      {!isLoading && !error && (
        <>
          <OrgStatsRow orgs={orgs} />

          <div className="mx-6 mb-6 bg-white rounded-lg border border-content-border overflow-hidden">
            <div
              className="grid bg-slate-50 border-b border-content-border px-4 py-2.5 text-[10px] font-bold uppercase tracking-widest text-text-secondary"
              style={{ gridTemplateColumns: '1fr 90px 130px 100px 80px' }}
            >
              <div>Name</div>
              <div>Code</div>
              <div>Type</div>
              <div>Status</div>
              <div>Actions</div>
            </div>

            {orgs.length === 0
              ? (
                <div className="p-10 text-center text-text-secondary text-sm">
                  No organizations yet.{' '}
                  <button className="text-laerdal-red underline" onClick={() => setShowCreate(true)}>
                    Create the first one.
                  </button>
                </div>
              )
              : <OrgTree orgs={orgs} onEdit={setEditOrg} onDelete={setDeleteOrg} />
            }
          </div>
        </>
      )}

      <CreateOrgPanel  open={showCreate} onClose={() => setShowCreate(false)} orgs={orgs} />
      <EditOrgPanel    org={editOrg}     onClose={() => setEditOrg(null)} />
      <DeleteOrgDialog org={deleteOrg}   onClose={() => setDeleteOrg(null)} />
    </div>
  )
}
```

- [ ] **Step 2: Start the C# API and the frontend, verify end-to-end**

```bash
# Terminal 1
dotnet run --project src/LaerdalImplementation.Api/LaerdalImplementation.Api.csproj

# Terminal 2
cd frontend && npm run dev
```

Open http://localhost:5173 — Organizations page should load. Create an org, watch it appear in the tree. Delete one with children, confirm the guard fires. Delete a leaf org, confirm it disappears.

- [ ] **Step 3: Commit**

```bash
cd .. && git add frontend/src/pages/OrganizationsPage.tsx
git commit -m "feat: OrganizationsPage fully wired to real API"
```

---

## Task 10: Manifest types, API client, and page

**Files:**
- Create: `frontend/src/features/manifests/types.ts`
- Create: `frontend/src/features/manifests/api.ts`
- Create: `frontend/src/features/manifests/StatusBadge.tsx`
- Create: `frontend/src/features/manifests/OrgSelector.tsx`
- Create: `frontend/src/features/manifests/ManifestList.tsx`
- Create: `frontend/src/features/manifests/CreateManifestPanel.tsx`
- Create: `frontend/src/features/manifests/PublishDialog.tsx`
- Modify: `frontend/src/pages/ManifestsPage.tsx`

- [ ] **Step 1: Create `frontend/src/features/manifests/types.ts`**

```ts
export type ManifestStatus = 0 | 1 | 2  // 0=Draft, 1=Published, 2=Archived

export interface Manifest {
  id: string
  organizationId: string
  name: string
  description: string | null
  version: string
  status: ManifestStatus
  content: string
  publishedAt: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateManifestRequest {
  name: string
  description?: string
}

export interface PublishManifestRequest {
  versionBump: 'major' | 'minor' | 'patch'
}

export const STATUS_LABEL: Record<ManifestStatus, string> = {
  0: 'Draft', 1: 'Published', 2: 'Archived',
}

export const STATUS_COLOUR: Record<ManifestStatus, string> = {
  0: 'bg-blue-100 text-blue-700',
  1: 'bg-green-100 text-green-700',
  2: 'bg-slate-100 text-slate-500',
}
```

- [ ] **Step 2: Create `frontend/src/features/manifests/api.ts`**

```ts
import type { Manifest, CreateManifestRequest, PublishManifestRequest } from './types'

const BASE = 'http://localhost:5000/api'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json', ...init?.headers },
    ...init,
  })
  if (!res.ok) {
    const text = await res.text().catch(() => res.statusText)
    throw new Error(text || `HTTP ${res.status}`)
  }
  if (res.status === 204) return undefined as T
  return res.json()
}

export const manifestApi = {
  list:    (orgId: string)                          => request<Manifest[]>(`/organizations/${orgId}/manifests`),
  create:  (orgId: string, body: CreateManifestRequest) => request<Manifest>(`/organizations/${orgId}/manifests`, { method: 'POST', body: JSON.stringify(body) }),
  publish: (orgId: string, manifestId: string, body: PublishManifestRequest) =>
    request<Manifest>(`/organizations/${orgId}/manifests/${manifestId}/publish`, { method: 'POST', body: JSON.stringify(body) }),
}
```

- [ ] **Step 3: Create `frontend/src/features/manifests/StatusBadge.tsx`**

```tsx
import type { ManifestStatus } from './types'
import { STATUS_LABEL, STATUS_COLOUR } from './types'

export function StatusBadge({ status }: { status: ManifestStatus }) {
  return (
    <span className={`text-[11px] font-semibold px-2 py-0.5 rounded ${STATUS_COLOUR[status]}`}>
      {STATUS_LABEL[status]}
    </span>
  )
}
```

- [ ] **Step 4: Create `frontend/src/features/manifests/OrgSelector.tsx`**

```tsx
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import type { Organization } from '@/features/organizations/types'

interface Props {
  orgs: Organization[]
  value: string
  onChange: (id: string) => void
}

export function OrgSelector({ orgs, value, onChange }: Props) {
  return (
    <Select value={value} onValueChange={onChange}>
      <SelectTrigger className="w-64">
        <SelectValue placeholder="Select organization…" />
      </SelectTrigger>
      <SelectContent>
        {orgs.map(o => (
          <SelectItem key={o.id} value={o.id}>{o.name}</SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
}
```

- [ ] **Step 5: Create `frontend/src/features/manifests/ManifestList.tsx`**

```tsx
import { BookOpen } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { StatusBadge } from './StatusBadge'
import type { Manifest } from './types'

interface Props {
  manifests: Manifest[]
  onPublish: (m: Manifest) => void
}

export function ManifestList({ manifests, onPublish }: Props) {
  if (manifests.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-16 text-text-secondary gap-3">
        <BookOpen className="w-10 h-10 opacity-30" />
        <p className="text-sm">No manifests yet for this organization.</p>
      </div>
    )
  }

  return (
    <div className="overflow-hidden">
      <div
        className="grid bg-slate-50 border-b border-content-border px-4 py-2.5 text-[10px] font-bold uppercase tracking-widest text-text-secondary"
        style={{ gridTemplateColumns: '1fr 100px 120px 140px 80px' }}
      >
        <div>Name</div><div>Version</div><div>Status</div><div>Created</div><div>Action</div>
      </div>
      {manifests.map(m => (
        <div
          key={m.id}
          className="grid items-center border-b border-content-border/60 last:border-0 hover:bg-slate-50 px-4 py-3 text-sm"
          style={{ gridTemplateColumns: '1fr 100px 120px 140px 80px' }}
        >
          <span className="font-medium text-text-primary">{m.name}</span>
          <span className="font-mono text-xs text-text-dim">v{m.version}</span>
          <StatusBadge status={m.status} />
          <span className="text-xs text-text-secondary">
            {new Date(m.createdAt).toLocaleDateString()}
          </span>
          {m.status === 0 && (
            <Button
              size="sm"
              className="bg-green-600 hover:bg-green-700 text-white text-xs h-7"
              onClick={() => onPublish(m)}
            >
              Publish
            </Button>
          )}
        </div>
      ))}
    </div>
  )
}
```

- [ ] **Step 6: Create `frontend/src/features/manifests/CreateManifestPanel.tsx`**

```tsx
import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { manifestApi } from './api'

interface Props { open: boolean; orgId: string; onClose: () => void }

export function CreateManifestPanel({ open, orgId, onClose }: Props) {
  const qc = useQueryClient()
  const [name, setName]   = useState('')
  const [desc, setDesc]   = useState('')
  const [error, setError] = useState('')

  const mutation = useMutation({
    mutationFn: () => manifestApi.create(orgId, { name, description: desc }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['manifests', orgId] })
      setName(''); setDesc(''); setError(''); onClose()
    },
    onError: (e: Error) => setError(e.message),
  })

  return (
    <Sheet open={open} onOpenChange={v => !v && onClose()}>
      <SheetContent className="w-[400px] sm:max-w-[400px]">
        <SheetHeader><SheetTitle>New Draft Manifest</SheetTitle></SheetHeader>
        <div className="flex flex-col gap-4 mt-6">
          <div className="space-y-1.5">
            <Label>Name</Label>
            <Input value={name} onChange={e => setName(e.target.value)} placeholder="Q3 Training Bundle" />
          </div>
          <div className="space-y-1.5">
            <Label>Description <span className="text-text-secondary text-xs">(optional)</span></Label>
            <Input value={desc} onChange={e => setDesc(e.target.value)} placeholder="Covers BLS modules…" />
          </div>
          {error && <p className="text-sm text-laerdal-red">{error}</p>}
          <Button
            className="bg-laerdal-red hover:bg-laerdal-red-dark text-white mt-2"
            disabled={!name || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            {mutation.isPending ? 'Creating…' : 'Create Draft'}
          </Button>
        </div>
      </SheetContent>
    </Sheet>
  )
}
```

- [ ] **Step 7: Create `frontend/src/features/manifests/PublishDialog.tsx`**

```tsx
import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog'
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { manifestApi } from './api'
import type { Manifest } from './types'

interface Props { manifest: Manifest | null; orgId: string; onClose: () => void }

function bumpVersion(version: string, bump: 'major' | 'minor' | 'patch') {
  const [maj, min, pat] = version.split('.').map(Number)
  if (bump === 'major') return `${maj + 1}.0.0`
  if (bump === 'minor') return `${maj}.${min + 1}.0`
  return `${maj}.${min}.${pat + 1}`
}

export function PublishDialog({ manifest, orgId, onClose }: Props) {
  const qc = useQueryClient()
  const [bump, setBump] = useState<'major' | 'minor' | 'patch'>('minor')

  const mutation = useMutation({
    mutationFn: () => manifestApi.publish(orgId, manifest!.id, { versionBump: bump }),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['manifests', orgId] }); onClose() },
  })

  const nextVersion = manifest ? bumpVersion(manifest.version, bump) : ''

  return (
    <Dialog open={!!manifest} onOpenChange={v => !v && onClose()}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Publish "{manifest?.name}"</DialogTitle>
        </DialogHeader>

        <div className="py-2">
          <p className="text-sm text-text-secondary mb-4">
            Current version: <span className="font-mono text-text-primary">v{manifest?.version}</span>
          </p>

          <RadioGroup value={bump} onValueChange={v => setBump(v as typeof bump)} className="gap-3">
            {(['patch', 'minor', 'major'] as const).map(b => (
              <div key={b} className="flex items-center gap-3 p-3 rounded-lg border border-content-border hover:bg-slate-50 cursor-pointer">
                <RadioGroupItem value={b} id={b} />
                <Label htmlFor={b} className="cursor-pointer flex-1">
                  <span className="font-semibold capitalize">{b}</span>
                  <span className="text-text-secondary text-xs ml-2">
                    {b === 'patch' ? '— bug fixes' : b === 'minor' ? '— new content' : '— breaking changes'}
                  </span>
                </Label>
                <span className="font-mono text-xs text-laerdal-red">→ v{bumpVersion(manifest?.version ?? '1.0.0', b)}</span>
              </div>
            ))}
          </RadioGroup>

          <p className="mt-4 text-sm text-text-secondary">
            Will publish as <span className="font-mono text-text-primary">v{nextVersion}</span> and archive the current published version.
          </p>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button
            className="bg-green-600 hover:bg-green-700 text-white"
            onClick={() => mutation.mutate()}
            disabled={mutation.isPending}
          >
            {mutation.isPending ? 'Publishing…' : `Publish v${nextVersion}`}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

- [ ] **Step 8: Replace `frontend/src/pages/ManifestsPage.tsx` with**

```tsx
import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Plus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { PageHeader } from '@/components/layout/PageHeader'
import { orgApi } from '@/features/organizations/api'
import { manifestApi } from '@/features/manifests/api'
import { OrgSelector }          from '@/features/manifests/OrgSelector'
import { ManifestList }         from '@/features/manifests/ManifestList'
import { CreateManifestPanel }  from '@/features/manifests/CreateManifestPanel'
import { PublishDialog }        from '@/features/manifests/PublishDialog'
import type { Manifest } from '@/features/manifests/types'

export function ManifestsPage() {
  const [orgId, setOrgId]           = useState('')
  const [showCreate, setShowCreate] = useState(false)
  const [toPublish, setToPublish]   = useState<Manifest | null>(null)

  const { data: orgs = [] } = useQuery({ queryKey: ['organizations'], queryFn: orgApi.list })

  const { data: manifests = [], isLoading, error } = useQuery({
    queryKey: ['manifests', orgId],
    queryFn: () => manifestApi.list(orgId),
    enabled: !!orgId,
  })

  return (
    <div className="flex flex-col flex-1">
      <PageHeader
        title="Manifests"
        subtitle="Versioned course content bundles"
        action={
          orgId
            ? <Button className="bg-laerdal-red hover:bg-laerdal-red-dark text-white gap-2" onClick={() => setShowCreate(true)}>
                <Plus className="w-4 h-4" /> New Draft
              </Button>
            : undefined
        }
      />

      <div className="p-6 pb-3">
        <OrgSelector orgs={orgs} value={orgId} onChange={setOrgId} />
      </div>

      <div className="mx-6 mb-6 bg-white rounded-lg border border-content-border overflow-hidden flex-1">
        {!orgId && (
          <p className="p-10 text-center text-text-secondary text-sm">
            Select an organization to see its manifests.
          </p>
        )}
        {orgId && isLoading && <p className="p-6 text-text-secondary text-sm">Loading…</p>}
        {orgId && error    && <p className="p-6 text-laerdal-red text-sm">Could not load manifests — endpoint may be coming soon.</p>}
        {orgId && !isLoading && !error && (
          <ManifestList manifests={manifests} onPublish={setToPublish} />
        )}
      </div>

      <CreateManifestPanel open={showCreate} orgId={orgId} onClose={() => setShowCreate(false)} />
      <PublishDialog manifest={toPublish} orgId={orgId} onClose={() => setToPublish(null)} />
    </div>
  )
}
```

- [ ] **Step 9: Commit**

```bash
git add frontend/src/features/manifests/ frontend/src/pages/ManifestsPage.tsx
git commit -m "feat: Manifests page with status badges, create draft, publish dialog"
```

---

## Task 11: CPR Finder — Discover page

**Files:**
- Create: `frontend/src/features/cpr-discover/mockData.ts`
- Create: `frontend/src/features/cpr-discover/MissionBanner.tsx`
- Create: `frontend/src/features/cpr-discover/StationCard.tsx`
- Modify: `frontend/src/pages/DiscoverPage.tsx`

- [ ] **Step 1: Create `frontend/src/features/cpr-discover/mockData.ts`**

```ts
export interface Station {
  id: string
  name: string
  address: string
  distanceKm: number
  slotsAvailable: number
  nextSlot: string
  certifications: string[]
  rating: number
  gradientFrom: string
  gradientTo: string
}

export const STATIONS: Station[] = [
  { id: '1', name: 'Copenhagen Central Training Hub', address: 'Vesterbrogade 12, 1620 Copenhagen', distanceKm: 0.4, slotsAvailable: 3, nextSlot: 'Tomorrow 09:00', certifications: ['BLS', 'AED'], rating: 4.9, gradientFrom: '#dbeafe', gradientTo: '#e0f2fe' },
  { id: '2', name: 'Rigshospitalet Training Center',  address: 'Blegdamsvej 9, 2100 Copenhagen',   distanceKm: 1.2, slotsAvailable: 8, nextSlot: 'Today 14:30',    certifications: ['AED'],        rating: 4.7, gradientFrom: '#fef3c7', gradientTo: '#fde68a' },
  { id: '3', name: 'Nørrebro CPR Station',            address: 'Nørrebrogade 43, 2200 Copenhagen', distanceKm: 2.1, slotsAvailable: 0, nextSlot: 'Jun 3rd',         certifications: ['BLS'],        rating: 4.8, gradientFrom: '#fce7f3', gradientTo: '#fbcfe8' },
  { id: '4', name: 'Østerbro Skills Lab',             address: 'Østerbrogade 102, 2100 Copenhagen',distanceKm: 3.4, slotsAvailable: 12,nextSlot: 'Today 16:00',    certifications: ['BLS'],        rating: 4.6, gradientFrom: '#e0e7ff', gradientTo: '#c7d2fe' },
  { id: '5', name: 'Frederiksberg CPR Hub',           address: 'Smallegade 2, 2000 Frederiksberg', distanceKm: 4.1, slotsAvailable: 5, nextSlot: 'Tomorrow 11:00', certifications: ['BLS', 'AED'], rating: 4.5, gradientFrom: '#dcfce7', gradientTo: '#bbf7d0' },
  { id: '6', name: 'Bispebjerg Hospital Lab',         address: 'Bispebjerg Bakke 23, 2400 Copenhagen', distanceKm: 5.0, slotsAvailable: 2, nextSlot: 'Jun 4th', certifications: ['BLS'], rating: 4.4, gradientFrom: '#f3e8ff', gradientTo: '#e9d5ff' },
]
```

- [ ] **Step 2: Create `frontend/src/features/cpr-discover/MissionBanner.tsx`**

```tsx
export function MissionBanner() {
  return (
    <div className="mx-6 mb-4 bg-laerdal-navy rounded-lg px-5 py-3.5 flex items-center gap-4 border border-laerdal-red/20">
      <span className="text-2xl flex-shrink-0">💗</span>
      <p className="text-sm text-slate-300 leading-relaxed">
        <span className="text-white font-semibold">One Million Lives by 2030</span>
        {' '}— every booking brings Laerdal closer to its mission of helping save one million more lives a year.
      </p>
    </div>
  )
}
```

- [ ] **Step 3: Create `frontend/src/features/cpr-discover/StationCard.tsx`**

```tsx
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import type { Station } from './mockData'

export function StationCard({ station }: { station: Station }) {
  const navigate = useNavigate()
  const full = station.slotsAvailable === 0

  return (
    <div className="bg-white rounded-xl border border-content-border overflow-hidden hover:shadow-md transition-shadow cursor-pointer">
      <div
        className="h-20 relative flex items-center justify-center"
        style={{ background: `linear-gradient(135deg, ${station.gradientFrom}, ${station.gradientTo})` }}
      >
        <span className="text-3xl drop-shadow">📍</span>
        <span className="absolute top-2 right-2 bg-white/95 text-text-primary text-[11px] font-bold px-2.5 py-1 rounded-full shadow-sm">
          {station.distanceKm} km
        </span>
      </div>

      <div className="p-4">
        <h3 className="text-sm font-bold text-text-primary mb-0.5">{station.name}</h3>
        <p className="text-xs text-text-secondary mb-3">{station.address}</p>

        <div className="flex items-center gap-2 mb-3 flex-wrap">
          <span className={`text-[11px] font-semibold px-2 py-0.5 rounded ${full ? 'bg-red-100 text-red-700' : 'bg-green-100 text-green-700'}`}>
            {full ? 'Fully booked' : `${station.slotsAvailable} slots left`}
          </span>
          {station.certifications.map(c => (
            <span key={c} className="text-[11px] font-semibold px-2 py-0.5 rounded bg-blue-100 text-blue-700">
              {c} Certified
            </span>
          ))}
          <span className="ml-auto text-xs text-amber-500 font-medium">★ {station.rating}</span>
        </div>

        <div className="flex items-center gap-2">
          <p className="text-xs text-text-secondary flex-1">
            Next: <span className="font-semibold text-text-primary">{station.nextSlot}</span>
          </p>
          <Button
            size="sm"
            className={`text-xs h-7 ${full ? 'bg-slate-100 text-slate-500 hover:bg-slate-200' : 'bg-laerdal-red hover:bg-laerdal-red-dark text-white'}`}
            onClick={() => !full && navigate(`/cpr-finder/book?station=${station.id}`)}
          >
            {full ? 'Waitlist' : 'Book →'}
          </Button>
        </div>
      </div>
    </div>
  )
}
```

- [ ] **Step 4: Replace `frontend/src/pages/DiscoverPage.tsx` with**

```tsx
import { useState } from 'react'
import { PageHeader } from '@/components/layout/PageHeader'
import { MissionBanner } from '@/features/cpr-discover/MissionBanner'
import { StationCard }   from '@/features/cpr-discover/StationCard'
import { STATIONS }      from '@/features/cpr-discover/mockData'

export function DiscoverPage() {
  const [search, setSearch] = useState('')

  const filtered = STATIONS.filter(s =>
    s.name.toLowerCase().includes(search.toLowerCase()) ||
    s.address.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div className="flex flex-col flex-1">
      <PageHeader title="Discover CPR Stations" subtitle="Find certified training stations near you" />

      <div className="px-6 py-4 bg-white border-b border-content-border flex items-center gap-3">
        <div className="flex items-center gap-2 bg-content-bg border border-content-border rounded-lg px-3 py-2 flex-1 max-w-md">
          <span className="text-text-dim text-sm">📍</span>
          <input
            className="bg-transparent outline-none text-sm text-text-primary flex-1"
            placeholder="Copenhagen, Denmark"
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        {(['CPR Only', 'This week', '4.5+ ★'] as const).map(f => (
          <button key={f} className="text-xs px-3 py-2 rounded-lg bg-content-bg border border-content-border text-text-dim hover:border-laerdal-red hover:text-laerdal-red transition-colors">
            {f}
          </button>
        ))}
      </div>

      <div className="px-6 py-3 flex items-center">
        <p className="text-xs text-text-secondary">
          <strong className="text-text-primary">{filtered.length} stations</strong> found near Copenhagen
        </p>
      </div>

      <MissionBanner />

      <div className="grid grid-cols-2 gap-4 px-6 pb-6">
        {filtered.map(s => <StationCard key={s.id} station={s} />)}
      </div>
    </div>
  )
}
```

- [ ] **Step 5: Commit**

```bash
git add frontend/src/features/cpr-discover/ frontend/src/pages/DiscoverPage.tsx
git commit -m "feat: CPR Finder Discover page with station cards"
```

---

## Task 12: CPR Finder — Booking wizard

**Files:**
- Create: `frontend/src/features/cpr-book/steps/StepStation.tsx`
- Create: `frontend/src/features/cpr-book/steps/StepDate.tsx`
- Create: `frontend/src/features/cpr-book/steps/StepTime.tsx`
- Create: `frontend/src/features/cpr-book/steps/StepConfirm.tsx`
- Create: `frontend/src/features/cpr-book/steps/StepSuccess.tsx`
- Create: `frontend/src/features/cpr-book/BookingWizard.tsx`
- Modify: `frontend/src/pages/BookPage.tsx`

- [ ] **Step 1: Create `frontend/src/features/cpr-book/steps/StepStation.tsx`**

```tsx
import { STATIONS } from '@/features/cpr-discover/mockData'

interface Props { stationId: string; onNext: () => void }

export function StepStation({ stationId, onNext }: Props) {
  const station = STATIONS.find(s => s.id === stationId)
  if (!station) return <p className="text-text-secondary text-sm">Station not found.</p>

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Your selected station</h2>
      <div className="border border-content-border rounded-xl overflow-hidden">
        <div
          className="h-20 flex items-center justify-center"
          style={{ background: `linear-gradient(135deg, ${station.gradientFrom}, ${station.gradientTo})` }}
        >
          <span className="text-3xl">📍</span>
        </div>
        <div className="p-4">
          <p className="font-bold text-text-primary text-sm">{station.name}</p>
          <p className="text-xs text-text-secondary mt-1">{station.address}</p>
          <p className="text-xs text-text-secondary mt-1">Next available: <strong className="text-text-primary">{station.nextSlot}</strong></p>
        </div>
      </div>
      <button
        className="w-full bg-laerdal-red hover:bg-laerdal-red-dark text-white font-semibold py-2.5 rounded-lg text-sm transition-colors"
        onClick={onNext}
      >
        Continue — Pick a Date →
      </button>
    </div>
  )
}
```

- [ ] **Step 2: Create `frontend/src/features/cpr-book/steps/StepDate.tsx`**

```tsx
import { useState } from 'react'
import { Calendar } from '@/components/ui/calendar'

interface Props { onNext: (date: Date) => void }

export function StepDate({ onNext }: Props) {
  const [date, setDate] = useState<Date | undefined>()

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Pick a date</h2>
      <div className="border border-content-border rounded-xl p-4 flex justify-center">
        <Calendar
          mode="single"
          selected={date}
          onSelect={setDate}
          disabled={d => d < new Date()}
        />
      </div>
      <button
        className="w-full bg-laerdal-red hover:bg-laerdal-red-dark text-white font-semibold py-2.5 rounded-lg text-sm transition-colors disabled:opacity-40"
        disabled={!date}
        onClick={() => date && onNext(date)}
      >
        Continue — Pick a Time →
      </button>
    </div>
  )
}
```

- [ ] **Step 3: Create `frontend/src/features/cpr-book/steps/StepTime.tsx`**

```tsx
import { useState } from 'react'

const SLOTS = ['09:00', '10:30', '12:00', '13:30', '15:00', '16:30']

interface Props { onNext: (time: string) => void }

export function StepTime({ onNext }: Props) {
  const [selected, setSelected] = useState('')

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Pick a time slot</h2>
      <div className="grid grid-cols-3 gap-3">
        {SLOTS.map(slot => (
          <button
            key={slot}
            onClick={() => setSelected(slot)}
            className={`py-3 rounded-lg text-sm font-semibold border transition-all ${
              selected === slot
                ? 'bg-laerdal-red text-white border-laerdal-red'
                : 'bg-white text-text-primary border-content-border hover:border-laerdal-red'
            }`}
          >
            {slot}
          </button>
        ))}
      </div>
      <button
        className="w-full bg-laerdal-red hover:bg-laerdal-red-dark text-white font-semibold py-2.5 rounded-lg text-sm transition-colors disabled:opacity-40"
        disabled={!selected}
        onClick={() => selected && onNext(selected)}
      >
        Continue — Confirm →
      </button>
    </div>
  )
}
```

- [ ] **Step 4: Create `frontend/src/features/cpr-book/steps/StepConfirm.tsx`**

```tsx
import { STATIONS } from '@/features/cpr-discover/mockData'

interface Props {
  stationId: string
  date: Date
  time: string
  onConfirm: () => void
}

export function StepConfirm({ stationId, date, time, onConfirm }: Props) {
  const station = STATIONS.find(s => s.id === stationId)

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Confirm your booking</h2>
      <div className="border border-content-border rounded-xl p-5 space-y-3 bg-slate-50">
        <Row label="Station" value={station?.name ?? ''} />
        <Row label="Address" value={station?.address ?? ''} />
        <Row label="Date"    value={date.toLocaleDateString('en-DK', { weekday: 'long', day: 'numeric', month: 'long' })} />
        <Row label="Time"    value={time} />
        <Row label="Type"    value="CPR / BLS Training" />
      </div>
      <button
        className="w-full bg-green-600 hover:bg-green-700 text-white font-semibold py-2.5 rounded-lg text-sm transition-colors"
        onClick={onConfirm}
      >
        Confirm Booking ✓
      </button>
    </div>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex gap-3 text-sm">
      <span className="w-20 text-text-secondary flex-shrink-0">{label}</span>
      <span className="font-medium text-text-primary">{value}</span>
    </div>
  )
}
```

- [ ] **Step 5: Create `frontend/src/features/cpr-book/steps/StepSuccess.tsx`**

```tsx
import { useNavigate } from 'react-router-dom'

export function StepSuccess() {
  const navigate = useNavigate()

  return (
    <div className="flex flex-col items-center text-center py-6 space-y-5">
      <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center text-3xl">✓</div>
      <div>
        <h2 className="text-lg font-bold text-text-primary">You're booked!</h2>
        <p className="text-sm text-text-secondary mt-1">
          Your CPR training slot is confirmed. You'll receive a reminder before the session.
        </p>
      </div>
      <div className="flex gap-3 w-full">
        <button
          className="flex-1 py-2.5 rounded-lg border border-content-border text-sm font-semibold text-text-primary hover:bg-slate-50 transition-colors"
          onClick={() => alert('Calendar integration coming soon')}
        >
          📅 Add to Calendar
        </button>
        <button
          className="flex-1 py-2.5 rounded-lg bg-laerdal-red text-white text-sm font-semibold hover:bg-laerdal-red-dark transition-colors"
          onClick={() => navigate('/cpr-finder/certifications')}
        >
          View Certifications →
        </button>
      </div>
    </div>
  )
}
```

- [ ] **Step 6: Create `frontend/src/features/cpr-book/BookingWizard.tsx`**

```tsx
import { useState } from 'react'
import { StepStation } from './steps/StepStation'
import { StepDate }    from './steps/StepDate'
import { StepTime }    from './steps/StepTime'
import { StepConfirm } from './steps/StepConfirm'
import { StepSuccess } from './steps/StepSuccess'

const STEP_LABELS = ['Station', 'Date', 'Time', 'Confirm']

interface Props { stationId: string }

export function BookingWizard({ stationId }: Props) {
  const [step, setStep]   = useState(0)
  const [date, setDate]   = useState<Date>(new Date())
  const [time, setTime]   = useState('')

  return (
    <div className="max-w-md w-full mx-auto">
      {step < 4 && (
        <div className="flex items-center gap-2 mb-6">
          {STEP_LABELS.map((label, i) => (
            <div key={label} className="flex items-center gap-2">
              <div className={`w-6 h-6 rounded-full text-xs font-bold flex items-center justify-center ${i < step ? 'bg-green-500 text-white' : i === step ? 'bg-laerdal-red text-white' : 'bg-content-border text-text-secondary'}`}>
                {i < step ? '✓' : i + 1}
              </div>
              <span className={`text-xs font-medium ${i === step ? 'text-text-primary' : 'text-text-secondary'}`}>{label}</span>
              {i < STEP_LABELS.length - 1 && <div className="h-px w-4 bg-content-border" />}
            </div>
          ))}
        </div>
      )}

      {step === 0 && <StepStation stationId={stationId} onNext={() => setStep(1)} />}
      {step === 1 && <StepDate    onNext={d => { setDate(d); setStep(2) }} />}
      {step === 2 && <StepTime    onNext={t => { setTime(t); setStep(3) }} />}
      {step === 3 && <StepConfirm stationId={stationId} date={date} time={time} onConfirm={() => setStep(4)} />}
      {step === 4 && <StepSuccess />}
    </div>
  )
}
```

- [ ] **Step 7: Replace `frontend/src/pages/BookPage.tsx` with**

```tsx
import { useSearchParams } from 'react-router-dom'
import { PageHeader }    from '@/components/layout/PageHeader'
import { BookingWizard } from '@/features/cpr-book/BookingWizard'
import { STATIONS }      from '@/features/cpr-discover/mockData'

export function BookPage() {
  const [params] = useSearchParams()
  const stationId = params.get('station') ?? STATIONS[0].id

  return (
    <div className="flex flex-col flex-1">
      <PageHeader title="Book a Session" subtitle="Reserve your CPR training slot" />
      <div className="p-6 flex justify-center">
        <BookingWizard stationId={stationId} />
      </div>
    </div>
  )
}
```

- [ ] **Step 8: Verify the booking flow works end-to-end**

Navigate to Discover, click "Book →" on a station. Confirm the wizard steps through Station → Date → Time → Confirm → Success, and "View Certifications →" navigates correctly.

- [ ] **Step 9: Commit**

```bash
git add frontend/src/features/cpr-book/ frontend/src/pages/BookPage.tsx
git commit -m "feat: CPR Finder booking wizard (5-step flow)"
```

---

## Task 13: CPR Finder — Certifications page

**Files:**
- Create: `frontend/src/features/cpr-certifications/mockData.ts`
- Modify: `frontend/src/pages/CertificationsPage.tsx`

- [ ] **Step 1: Create `frontend/src/features/cpr-certifications/mockData.ts`**

```ts
export interface Certification {
  id: string
  type: string
  station: string
  completedAt: string
  expiresAt: string
  status: 'valid' | 'expiring' | 'expired'
}

export const CERTIFICATIONS: Certification[] = [
  { id: '1', type: 'BLS — Basic Life Support',     station: 'Copenhagen Central Training Hub', completedAt: '2025-11-10', expiresAt: '2027-11-10', status: 'valid' },
  { id: '2', type: 'AED — Automated Defibrillation', station: 'Rigshospitalet Training Center', completedAt: '2024-03-22', expiresAt: '2026-03-22', status: 'expiring' },
]

export const STATUS_STYLE: Record<Certification['status'], string> = {
  valid:    'bg-green-100 text-green-700',
  expiring: 'bg-amber-100 text-amber-700',
  expired:  'bg-red-100 text-red-700',
}

export const STATUS_LABEL: Record<Certification['status'], string> = {
  valid:    'Valid',
  expiring: 'Expiring Soon',
  expired:  'Expired',
}
```

- [ ] **Step 2: Replace `frontend/src/pages/CertificationsPage.tsx` with**

```tsx
import { PageHeader } from '@/components/layout/PageHeader'
import { CERTIFICATIONS, STATUS_STYLE, STATUS_LABEL } from '@/features/cpr-certifications/mockData'

export function CertificationsPage() {
  return (
    <div className="flex flex-col flex-1">
      <PageHeader title="My Certifications" subtitle="Your completed CPR training certificates" />

      <div className="p-6">
        {CERTIFICATIONS.length === 0 ? (
          <p className="text-center text-text-secondary text-sm py-12">
            No certifications yet. <a href="/cpr-finder/discover" className="text-laerdal-red underline">Book a session</a> to get started.
          </p>
        ) : (
          <div className="bg-white rounded-xl border border-content-border overflow-hidden">
            <div
              className="grid bg-slate-50 border-b border-content-border px-5 py-3 text-[10px] font-bold uppercase tracking-widest text-text-secondary"
              style={{ gridTemplateColumns: '2fr 2fr 110px 110px 110px 90px' }}
            >
              <div>Certification</div>
              <div>Station</div>
              <div>Completed</div>
              <div>Expires</div>
              <div>Status</div>
              <div>Download</div>
            </div>
            {CERTIFICATIONS.map(cert => (
              <div
                key={cert.id}
                className="grid items-center border-b border-content-border/60 last:border-0 px-5 py-4 hover:bg-slate-50 text-sm"
                style={{ gridTemplateColumns: '2fr 2fr 110px 110px 110px 90px' }}
              >
                <span className="font-semibold text-text-primary">{cert.type}</span>
                <span className="text-text-secondary text-xs">{cert.station}</span>
                <span className="text-text-secondary text-xs">{new Date(cert.completedAt).toLocaleDateString()}</span>
                <span className="text-text-secondary text-xs">{new Date(cert.expiresAt).toLocaleDateString()}</span>
                <span className={`text-[11px] font-semibold px-2 py-0.5 rounded inline-block ${STATUS_STYLE[cert.status]}`}>
                  {STATUS_LABEL[cert.status]}
                </span>
                <button
                  className="text-xs text-laerdal-red underline hover:no-underline"
                  onClick={() => alert('PDF download coming soon')}
                >
                  Download PDF
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
```

- [ ] **Step 3: Commit**

```bash
git add frontend/src/features/cpr-certifications/ frontend/src/pages/CertificationsPage.tsx
git commit -m "feat: Certifications page with status badges and mock data"
```

---

## Task 14: Final polish

**Files:**
- Modify: `frontend/src/App.tsx` — add Toaster
- Modify: `frontend/src/components/layout/Sidebar.tsx` — manifest draft badge

- [ ] **Step 1: Add `Toaster` to `frontend/src/App.tsx`**

Add this import at the top:
```tsx
import { Toaster } from '@/components/ui/toaster'
```

Add `<Toaster />` just before the closing `</BrowserRouter>` tag in `main.tsx` (or at the bottom of the `Shell` component body in `App.tsx`).

- [ ] **Step 2: Add manifest draft badge to Sidebar**

Update `frontend/src/components/layout/Sidebar.tsx` — replace the static nav items with a version that fetches the draft count from TanStack Query cache:

```tsx
import { useQueryClient } from '@tanstack/react-query'
import type { Manifest } from '@/features/manifests/types'

// inside Sidebar(), before the return:
const qc = useQueryClient()
const allManifests = qc.getQueriesData<Manifest[]>({ queryKey: ['manifests'] })
const draftCount = allManifests.flatMap(([, data]) => data ?? []).filter(m => m.status === 0).length
```

Then in the Manifests nav item, add the badge:
```tsx
{ to: '/implementer/manifests', icon: '📋', label: 'Manifests', badge: draftCount || undefined },
```

Update the `NavLink` render to show the badge:
```tsx
{item.label}
{item.badge && (
  <span className="ml-auto text-[10px] bg-laerdal-red/20 text-red-300 px-1.5 py-0.5 rounded-full">
    {item.badge}
  </span>
)}
```

- [ ] **Step 3: Verify the full app end-to-end**

```bash
# Terminal 1 — backend
dotnet run --project src/LaerdalImplementation.Api/LaerdalImplementation.Api.csproj

# Terminal 2 — frontend
cd frontend && npm run dev
```

Walk through:
1. Organizations: create a hospital, add a child department, edit the name, try to delete the parent (blocked), delete the child, delete the parent
2. Manifests: select an org, see empty state, create a draft
3. CPR Finder Discover: see station cards, click Book on one
4. Booking wizard: step through all 5 stages to Success
5. Certifications: see the two mock certs with status badges

- [ ] **Step 4: Final commit**

```bash
cd .. && git add frontend/
git commit -m "feat: complete Laerdal Platform frontend — Implementer + CPR Finder"
```

---

## Self-Review

**Spec coverage check:**

| Spec section | Task |
|---|---|
| Tech stack (Vite, React, TS, Tailwind, shadcn, Router, TanStack Query) | Tasks 1–2 |
| Hybrid theme + brand tokens | Task 2 |
| App shell (topbar, sidebar, routing) | Task 3 |
| Org types + API client | Task 4 |
| OrgStatsRow | Task 5 |
| OrgTree with expand/collapse | Task 6 |
| CreateOrgPanel + EditOrgPanel | Task 7 |
| DeleteOrgDialog with children guard | Task 8 |
| OrganizationsPage assembled | Task 9 |
| Manifest types + API client | Task 10 |
| ManifestList + StatusBadge + OrgSelector | Task 10 |
| CreateManifestPanel + PublishDialog | Task 10 |
| ManifestsPage | Task 10 |
| CPR Discover mock data + MissionBanner + StationCard | Task 11 |
| DiscoverPage | Task 11 |
| Booking wizard all 5 steps | Task 12 |
| BookPage | Task 12 |
| Certifications mock data + page | Task 13 |
| Toaster + manifest draft badge polish | Task 14 |
| CORS (already present in Program.cs) | Pre-verified |

All spec requirements covered. ✅
