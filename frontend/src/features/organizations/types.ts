export type OrganizationType = 0 | 1 | 2  // 0=Hospital, 1=Department, 2=TrainingCenter

export interface Organization {
  id: string
  name: string
  code: string
  type: OrganizationType
  parentId: string | null
  isActive: boolean
  externalId: string | null
  createdAt: string
  updatedAt: string
  children: Organization[]
}

export interface CreateOrganizationRequest {
  name: string
  code: string
  type: OrganizationType
  parentId?: string
}

export interface UpdateOrganizationRequest {
  name?: string
  type?: OrganizationType
  isActive?: boolean
}

export const ORG_TYPE_LABEL: Record<OrganizationType, string> = {
  0: 'Hospital',
  1: 'Department',
  2: 'Training Center',
}

export const ORG_TYPE_COLOUR: Record<OrganizationType, string> = {
  0: 'bg-blue-100 text-blue-700',
  1: 'bg-green-100 text-green-700',
  2: 'bg-amber-100 text-amber-700',
}
