export type ManifestStatus = 0 | 1 | 2  // 0=Draft, 1=Published, 2=Archived

export interface Manifest {
  id: string
  organizationId: string
  name: string
  description: string | null
  version: string
  status: ManifestStatus
  content: string
  publishedAt: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateManifestRequest {
  name: string
  description?: string
}

export interface PublishManifestRequest {
  versionBump: 'major' | 'minor' | 'patch'
}

export const STATUS_LABEL: Record<ManifestStatus, string> = {
  0: 'Draft', 1: 'Published', 2: 'Archived',
}

export const STATUS_COLOUR: Record<ManifestStatus, string> = {
  0: 'bg-blue-100 text-blue-700',
  1: 'bg-green-100 text-green-700',
  2: 'bg-slate-100 text-slate-500',
}
