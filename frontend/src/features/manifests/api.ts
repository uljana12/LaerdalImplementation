import type { Manifest, CreateManifestRequest, PublishManifestRequest } from './types'

const BASE = 'http://localhost:5000/api'

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

export const manifestApi = {
  list:    (orgId: string) =>
    request<Manifest[]>(`/organizations/${orgId}/manifests`),
  create:  (orgId: string, body: CreateManifestRequest) =>
    request<Manifest>(`/organizations/${orgId}/manifests`, { method: 'POST', body: JSON.stringify(body) }),
  publish: (orgId: string, manifestId: string, body: PublishManifestRequest) =>
    request<Manifest>(`/organizations/${orgId}/manifests/${manifestId}/publish`, { method: 'POST', body: JSON.stringify(body) }),
}
