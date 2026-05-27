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
