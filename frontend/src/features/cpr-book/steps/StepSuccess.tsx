import { useNavigate } from 'react-router-dom'

export function StepSuccess() {
  const navigate = useNavigate()

  return (
    <div className="flex flex-col items-center text-center py-6 space-y-5">
      <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center text-3xl">✓</div>
      <div>
        <h2 className="text-lg font-bold text-text-primary">You're booked!</h2>
        <p className="text-sm text-text-secondary mt-1">
          Your CPR training slot is confirmed. You'll receive a reminder before the session.
        </p>
      </div>
      <div className="flex gap-3 w-full">
        <button
          className="flex-1 py-2.5 rounded-lg border border-content-border text-sm font-semibold text-text-primary hover:bg-slate-50 transition-colors"
          onClick={() => alert('Calendar integration coming soon')}
        >
          📅 Add to Calendar
        </button>
        <button
          className="flex-1 py-2.5 rounded-lg bg-laerdal-red text-white text-sm font-semibold hover:bg-laerdal-red-dark transition-colors"
          onClick={() => navigate('/cpr-finder/certifications')}
        >
          View Certifications →
        </button>
      </div>
    </div>
  )
}
