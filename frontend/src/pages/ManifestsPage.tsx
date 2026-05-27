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
