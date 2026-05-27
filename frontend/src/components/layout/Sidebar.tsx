import { NavLink } from 'react-router-dom'
import { useQueryClient } from '@tanstack/react-query'
import { cn } from '@/lib/utils'
import type { Manifest } from '@/features/manifests/types'

const NAV = [
  {
    section: 'Implementer',
    items: [
      { to: '/implementer/organizations', icon: '🏢', label: 'Organizations', badgeKey: null },
      { to: '/implementer/manifests',     icon: '📋', label: 'Manifests',     badgeKey: 'manifests' },
    ],
  },
  {
    section: 'CPR Finder',
    items: [
      { to: '/cpr-finder/discover',       icon: '📍', label: 'Discover',       badgeKey: null },
      { to: '/cpr-finder/book',           icon: '📅', label: 'Book a Session', badgeKey: null },
      { to: '/cpr-finder/certifications', icon: '🎓', label: 'Certifications', badgeKey: null },
    ],
  },
]

export function Sidebar() {
  const qc = useQueryClient()
  const allManifests = qc.getQueriesData<Manifest[]>({ queryKey: ['manifests'] })
  const draftCount = allManifests.flatMap(([, data]) => data ?? []).filter(m => m.status === 0).length

  return (
    <aside className="w-52 bg-laerdal-card border-r border-white/[0.05] flex flex-col py-3 flex-shrink-0">
      {NAV.map((group, gi) => (
        <div key={group.section}>
          {gi > 0 && <hr className="border-white/[0.05] my-2" />}
          <p className="text-[10px] font-bold uppercase tracking-widest text-white/25 px-4 py-2">
            {group.section}
          </p>
          {group.items.map((item) => {
            const badge = item.badgeKey === 'manifests' ? (draftCount || undefined) : undefined
            return (
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
                {badge !== undefined && (
                  <span className="ml-auto text-[10px] bg-laerdal-red/20 text-red-300 px-1.5 py-0.5 rounded-full">
                    {badge}
                  </span>
                )}
              </NavLink>
            )
          })}
        </div>
      ))}
    </aside>
  )
}
