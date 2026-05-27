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
