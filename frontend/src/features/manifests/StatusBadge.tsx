import type { ManifestStatus } from './types'
import { STATUS_LABEL, STATUS_COLOUR } from './types'

export function StatusBadge({ status }: { status: ManifestStatus }) {
  return (
    <span className={`text-[11px] font-semibold px-2 py-0.5 rounded ${STATUS_COLOUR[status]}`}>
      {STATUS_LABEL[status]}
    </span>
  )
}
