import type { Organization } from './types'

interface Props { orgs: Organization[] }

function flattenOrgs(orgs: Organization[]): Organization[] {
  return orgs.flatMap(o => [o, ...flattenOrgs(o.children ?? [])])
}

export function OrgStatsRow({ orgs }: Props) {
  const flat        = flattenOrgs(orgs)
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
          <p className="text-[10px] font-semibold uppercase tracking-widest text-text-secondary mb-1">{label}</p>
          <p className="text-2xl font-bold text-text-primary">{value}</p>
          <p className="text-[11px] text-text-secondary mt-0.5">{sub}</p>
        </div>
      ))}
    </div>
  )
}
