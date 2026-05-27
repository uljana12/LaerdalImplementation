import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { orgApi } from './api'
import type { Organization, OrganizationType } from './types'

interface Props {
  open: boolean
  onClose: () => void
  orgs: Organization[]
}

export function CreateOrgPanel({ open, onClose, orgs }: Props) {
  const qc = useQueryClient()
  const [name, setName]         = useState('')
  const [code, setCode]         = useState('')
  const [type, setType]         = useState<OrganizationType>(0)
  const [parentId, setParentId] = useState<string>('')
  const [error, setError]       = useState('')

  const mutation = useMutation({
    mutationFn: () => orgApi.create({
      name, code, type,
      ...(parentId ? { parentId } : {}),
    }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['organizations'] })
      setName(''); setCode(''); setType(0); setParentId(''); setError('')
      onClose()
    },
    onError: (e: Error) => setError(e.message),
  })

  return (
    <Sheet open={open} onOpenChange={v => !v && onClose()}>
      <SheetContent className="w-[400px] sm:max-w-[400px]">
        <SheetHeader>
          <SheetTitle>New Organization</SheetTitle>
        </SheetHeader>
        <div className="flex flex-col gap-4 mt-6">
          <div className="space-y-1.5">
            <Label>Name</Label>
            <Input value={name} onChange={e => setName(e.target.value)} placeholder="City Hospital" />
          </div>
          <div className="space-y-1.5">
            <Label>Code <span className="text-text-secondary text-xs">(short unique ID)</span></Label>
            <Input value={code} onChange={e => setCode(e.target.value.toUpperCase())} placeholder="CITY" />
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
          <div className="space-y-1.5">
            <Label>Parent <span className="text-text-secondary text-xs">(optional)</span></Label>
            <Select value={parentId} onValueChange={setParentId}>
              <SelectTrigger><SelectValue placeholder="No parent (root org)" /></SelectTrigger>
              <SelectContent>
                <SelectItem value="">No parent</SelectItem>
                {orgs.map(o => (
                  <SelectItem key={o.id} value={o.id}>{o.name}</SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          {error && <p className="text-sm text-laerdal-red">{error}</p>}
          <Button
            className="bg-laerdal-red hover:bg-laerdal-red-dark text-white mt-2"
            disabled={!name || !code || mutation.isPending}
            onClick={() => mutation.mutate()}
          >
            {mutation.isPending ? 'Creating…' : 'Create Organization'}
          </Button>
        </div>
      </SheetContent>
    </Sheet>
  )
}
