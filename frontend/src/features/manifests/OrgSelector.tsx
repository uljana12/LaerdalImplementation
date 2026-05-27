import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import type { Organization } from '@/features/organizations/types'

interface Props {
  orgs: Organization[]
  value: string
  onChange: (id: string) => void
}

export function OrgSelector({ orgs, value, onChange }: Props) {
  return (
    <Select value={value} onValueChange={onChange}>
      <SelectTrigger className="w-64">
        <SelectValue placeholder="Select organization…" />
      </SelectTrigger>
      <SelectContent>
        {orgs.map(o => (
          <SelectItem key={o.id} value={o.id}>{o.name}</SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
}
