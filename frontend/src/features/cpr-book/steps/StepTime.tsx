import { useState } from 'react'

const SLOTS = ['09:00', '10:30', '12:00', '13:30', '15:00', '16:30']

interface Props { onNext: (time: string) => void }

export function StepTime({ onNext }: Props) {
  const [selected, setSelected] = useState('')

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Pick a time slot</h2>
      <div className="grid grid-cols-3 gap-3">
        {SLOTS.map(slot => (
          <button
            key={slot}
            onClick={() => setSelected(slot)}
            className={`py-3 rounded-lg text-sm font-semibold border transition-all ${
              selected === slot
                ? 'bg-laerdal-red text-white border-laerdal-red'
                : 'bg-white text-text-primary border-content-border hover:border-laerdal-red'
            }`}
          >
            {slot}
          </button>
        ))}
      </div>
      <button
        className="w-full bg-laerdal-red hover:bg-laerdal-red-dark text-white font-semibold py-2.5 rounded-lg text-sm transition-colors disabled:opacity-40"
        disabled={!selected}
        onClick={() => selected && onNext(selected)}
      >
        Continue — Confirm →
      </button>
    </div>
  )
}
