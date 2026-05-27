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
