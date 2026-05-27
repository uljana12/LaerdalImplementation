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
