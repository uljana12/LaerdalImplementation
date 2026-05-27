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
        <OrgRow key={org.id} org={org} onEdit={onEdit} onDelete={onDelete} depth={depth} />
      ))}
    </>
  )
}

function OrgRow({
  org, onEdit, onDelete, depth,
}: {
  org: Organization
  onEdit: (o: Organization) => void
  onDelete: (o: Organization) => void
  depth: number
}) {
  const [open, setOpen] = useState(depth === 0)
  const hasChildren = (org.children ?? []).length > 0

  return (
    <>
      <div
        className="grid items-center border-b border-content-border/60 last:border-0 hover:bg-slate-50 transition-colors"
        style={{ gridTemplateColumns: '1fr 90px 130px 100px 80px' }}
      >
        <div className="flex items-center gap-2 py-2.5" style={{ paddingLeft: `${16 + depth * 20}px` }}>
          {hasChildren ? (
            <button
              onClick={() => setOpen(o => !o)}
              className="w-5 h-5 rounded bg-slate-100 flex items-center justify-center text-text-dim hover:bg-slate-200 flex-shrink-0"
            >
              {open ? <ChevronDown className="w-3 h-3" /> : <ChevronRight className="w-3 h-3" />}
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
        <OrgTree orgs={org.children} onEdit={onEdit} onDelete={onDelete} depth={depth + 1} />
      )}
    </>
  )
}
