import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import type { Station } from './mockData'

export function StationCard({ station }: { station: Station }) {
  const navigate = useNavigate()
  const full = station.slotsAvailable === 0

  return (
    <div className="bg-white rounded-xl border border-content-border overflow-hidden hover:shadow-md transition-shadow cursor-pointer">
      <div
        className="h-20 relative flex items-center justify-center"
        style={{ background: `linear-gradient(135deg, ${station.gradientFrom}, ${station.gradientTo})` }}
      >
        <span className="text-3xl drop-shadow">📍</span>
        <span className="absolute top-2 right-2 bg-white/95 text-text-primary text-[11px] font-bold px-2.5 py-1 rounded-full shadow-sm">
          {station.distanceKm} km
        </span>
      </div>

      <div className="p-4">
        <h3 className="text-sm font-bold text-text-primary mb-0.5">{station.name}</h3>
        <p className="text-xs text-text-secondary mb-3">{station.address}</p>

        <div className="flex items-center gap-2 mb-3 flex-wrap">
          <span className={`text-[11px] font-semibold px-2 py-0.5 rounded ${full ? 'bg-red-100 text-red-700' : 'bg-green-100 text-green-700'}`}>
            {full ? 'Fully booked' : `${station.slotsAvailable} slots left`}
          </span>
          {station.certifications.map(c => (
            <span key={c} className="text-[11px] font-semibold px-2 py-0.5 rounded bg-blue-100 text-blue-700">
              {c} Certified
            </span>
          ))}
          <span className="ml-auto text-xs text-amber-500 font-medium">★ {station.rating}</span>
        </div>

        <div className="flex items-center gap-2">
          <p className="text-xs text-text-secondary flex-1">
            Next: <span className="font-semibold text-text-primary">{station.nextSlot}</span>
          </p>
          <Button
            size="sm"
            className={`text-xs h-7 ${full ? 'bg-slate-100 text-slate-500 hover:bg-slate-200' : 'bg-laerdal-red hover:bg-laerdal-red-dark text-white'}`}
            onClick={() => !full && navigate(`/cpr-finder/book?station=${station.id}`)}
          >
            {full ? 'Waitlist' : 'Book →'}
          </Button>
        </div>
      </div>
    </div>
  )
}
