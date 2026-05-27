import { STATIONS } from '@/features/cpr-discover/mockData'

interface Props {
  stationId: string
  date: Date
  time: string
  onConfirm: () => void
}

export function StepConfirm({ stationId, date, time, onConfirm }: Props) {
  const station = STATIONS.find(s => s.id === stationId)

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Confirm your booking</h2>
      <div className="border border-content-border rounded-xl p-5 space-y-3 bg-slate-50">
        <Row label="Station" value={station?.name ?? ''} />
        <Row label="Address" value={station?.address ?? ''} />
        <Row label="Date"    value={date.toLocaleDateString('en-DK', { weekday: 'long', day: 'numeric', month: 'long' })} />
        <Row label="Time"    value={time} />
        <Row label="Type"    value="CPR / BLS Training" />
      </div>
      <button
        className="w-full bg-green-600 hover:bg-green-700 text-white font-semibold py-2.5 rounded-lg text-sm transition-colors"
        onClick={onConfirm}
      >
        Confirm Booking ✓
      </button>
    </div>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex gap-3 text-sm">
      <span className="w-20 text-text-secondary flex-shrink-0">{label}</span>
      <span className="font-medium text-text-primary">{value}</span>
    </div>
  )
}
