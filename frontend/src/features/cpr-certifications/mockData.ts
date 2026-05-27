export interface Certification {
  id: string
  type: string
  station: string
  completedAt: string
  expiresAt: string
  status: 'valid' | 'expiring' | 'expired'
}

export const CERTIFICATIONS: Certification[] = [
  { id: '1', type: 'BLS — Basic Life Support',     station: 'Copenhagen Central Training Hub', completedAt: '2025-11-10', expiresAt: '2027-11-10', status: 'valid' },
  { id: '2', type: 'AED — Automated Defibrillation', station: 'Rigshospitalet Training Center', completedAt: '2024-03-22', expiresAt: '2026-03-22', status: 'expiring' },
]

export const STATUS_STYLE: Record<Certification['status'], string> = {
  valid:    'bg-green-100 text-green-700',
  expiring: 'bg-amber-100 text-amber-700',
  expired:  'bg-red-100 text-red-700',
}

export const STATUS_LABEL: Record<Certification['status'], string> = {
  valid:    'Valid',
  expiring: 'Expiring Soon',
  expired:  'Expired',
}
