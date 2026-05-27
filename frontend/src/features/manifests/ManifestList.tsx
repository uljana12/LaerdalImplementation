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
