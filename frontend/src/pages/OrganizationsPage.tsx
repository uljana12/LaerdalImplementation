import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Plus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { PageHeader } from '@/components/layout/PageHeader'
import { orgApi }            from '@/features/organizations/api'
import { OrgStatsRow }       from '@/features/organizations/OrgStatsRow'
import { OrgTree }           from '@/features/organizations/OrgTree'
import { CreateOrgPanel }    from '@/features/organizations/CreateOrgPanel'
import { EditOrgPanel }      from '@/features/organizations/EditOrgPanel'
import { DeleteOrgDialog }   from '@/features/organizations/DeleteOrgDialog'
import type { Organization } from '@/features/organizations/types'

export function OrganizationsPage() {
  const { data: orgs = [], isLoading, error } = useQuery({
    queryKey: ['organizations'],
    queryFn: orgApi.list,
  })

  const [showCreate, setShowCreate] = useState(false)
  const [editOrg, setEditOrg]       = useState<Organization | null>(null)
  const [deleteOrg, setDeleteOrg]   = useState<Organization | null>(null)

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
      {error && <p className="p-6 text-laerdal-red text-sm">Could not reach the API. Check that it is running.</p>}

      {!isLoading && !error && (
        <>
          <OrgStatsRow orgs={orgs} />
          <div className="mx-6 mb-6 bg-white rounded-lg border border-content-border overflow-hidden">
            <div
              className="grid bg-slate-50 border-b border-content-border px-4 py-2.5 text-[10px] font-bold uppercase tracking-widest text-text-secondary"
              style={{ gridTemplateColumns: '1fr 90px 130px 100px 80px' }}
            >
              <div>Name</div><div>Code</div><div>Type</div><div>Status</div><div>Actions</div>
            </div>
            {orgs.length === 0 ? (
              <div className="p-10 text-center text-text-secondary text-sm">
                No organizations yet.{' '}
                <button className="text-laerdal-red underline" onClick={() => setShowCreate(true)}>
                  Create the first one.
                </button>
              </div>
            ) : (
              <OrgTree orgs={orgs} onEdit={setEditOrg} onDelete={setDeleteOrg} />
            )}
          </div>
        </>
      )}

      <CreateOrgPanel  open={showCreate} onClose={() => setShowCreate(false)} orgs={orgs} />
      <EditOrgPanel    org={editOrg}     onClose={() => setEditOrg(null)} />
      <DeleteOrgDialog org={deleteOrg}   onClose={() => setDeleteOrg(null)} />
    </div>
  )
}
