import { STATIONS } from '@/features/cpr-discover/mockData'

interface Props { stationId: string; onNext: () => void }

export function StepStation({ stationId, onNext }: Props) {
  const station = STATIONS.find(s => s.id === stationId)
  if (!station) return <p className="text-text-secondary text-sm">Station not found.</p>

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Your selected station</h2>
      <div className="border border-content-border rounded-xl overflow-hidden">
        <div
          className="h-20 flex items-center justify-center"
          style={{ background: `linear-gradient(135deg, ${station.gradientFrom}, ${station.gradientTo})` }}
        >
          <span className="text-3xl">📍</span>
        </div>
        <div className="p-4">
          <p className="font-bold text-text-primary text-sm">{station.name}</p>
          <p className="text-xs text-text-secondary mt-1">{station.address}</p>
          <p className="text-xs text-text-secondary mt-1">Next available: <strong className="text-text-primary">{station.nextSlot}</strong></p>
        </div>
      </div>
      <button
        className="w-full bg-laerdal-red hover:bg-laerdal-red-dark text-white font-semibold py-2.5 rounded-lg text-sm transition-colors"
        onClick={onNext}
      >
        Continue — Pick a Date →
      </button>
    </div>
  )
}
