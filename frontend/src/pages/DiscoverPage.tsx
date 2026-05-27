import { useState } from 'react'
import { PageHeader } from '@/components/layout/PageHeader'
import { MissionBanner } from '@/features/cpr-discover/MissionBanner'
import { StationCard }   from '@/features/cpr-discover/StationCard'
import { STATIONS }      from '@/features/cpr-discover/mockData'

export function DiscoverPage() {
  const [search, setSearch] = useState('')

  const filtered = STATIONS.filter(s =>
    s.name.toLowerCase().includes(search.toLowerCase()) ||
    s.address.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div className="flex flex-col flex-1">
      <PageHeader title="Discover CPR Stations" subtitle="Find certified training stations near you" />

      <div className="px-6 py-4 bg-white border-b border-content-border flex items-center gap-3">
        <div className="flex items-center gap-2 bg-content-bg border border-content-border rounded-lg px-3 py-2 flex-1 max-w-md">
          <span className="text-text-dim text-sm">📍</span>
          <input
            className="bg-transparent outline-none text-sm text-text-primary flex-1"
            placeholder="Copenhagen, Denmark"
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        {(['CPR Only', 'This week', '4.5+ ★'] as const).map(f => (
          <button key={f} className="text-xs px-3 py-2 rounded-lg bg-content-bg border border-content-border text-text-dim hover:border-laerdal-red hover:text-laerdal-red transition-colors">
            {f}
          </button>
        ))}
      </div>

      <div className="px-6 py-3 flex items-center">
        <p className="text-xs text-text-secondary">
          <strong className="text-text-primary">{filtered.length} stations</strong> found near Copenhagen
        </p>
      </div>

      <MissionBanner />

      <div className="grid grid-cols-2 gap-4 px-6 pb-6">
        {filtered.map(s => <StationCard key={s.id} station={s} />)}
      </div>
    </div>
  )
}
