import type {
  Organization,
  CreateOrganizationRequest,
  UpdateOrganizationRequest,
} from './types'

const BASE = (import.meta.env.VITE_API_URL as string) || 'http://localhost:5050/api'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json', ...init?.headers },
    ...init,
  })
  if (!res.ok) {
    const text = await res.text().catch(() => res.statusText)
    throw new Error(text || `HTTP ${res.status}`)
  }
  if (res.status === 204) return undefined as T
  return res.json()
}

export const orgApi = {
  list:     ()                                          => request<Organization[]>('/organizations'),
  getById:  (id: string)                               => request<Organization>(`/organizations/${id}`),
  create:   (body: CreateOrganizationRequest)          => request<Organization>('/organizations', { method: 'POST', body: JSON.stringify(body) }),
  update:   (id: string, body: UpdateOrganizationRequest) => request<Organization>(`/organizations/${id}`, { method: 'PATCH', body: JSON.stringify(body) }),
  delete:   (id: string)                               => request<void>(`/organizations/${id}`, { method: 'DELETE' }),
}
