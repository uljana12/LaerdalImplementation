import { useState } from 'react'
import { Calendar } from '@/components/ui/calendar'

interface Props { onNext: (date: Date) => void }

export function StepDate({ onNext }: Props) {
  const [date, setDate] = useState<Date | undefined>()

  return (
    <div className="space-y-4">
      <h2 className="text-base font-bold text-text-primary">Pick a date</h2>
      <div className="border border-content-border rounded-xl p-4 flex justify-center">
        <Calendar
          mode="single"
          selected={date}
          onSelect={setDate}
          disabled={d => d < new Date()}
        />
      </div>
      <button
        className="w-full bg-laerdal-red hover:bg-laerdal-red-dark text-white font-semibold py-2.5 rounded-lg text-sm transition-colors disabled:opacity-40"
        disabled={!date}
        onClick={() => date && onNext(date)}
      >
        Continue — Pick a Time →
      </button>
    </div>
  )
}
