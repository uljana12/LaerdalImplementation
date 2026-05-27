import { useState } from 'react'
import { StepStation } from './steps/StepStation'
import { StepDate }    from './steps/StepDate'
import { StepTime }    from './steps/StepTime'
import { StepConfirm } from './steps/StepConfirm'
import { StepSuccess } from './steps/StepSuccess'

const STEP_LABELS = ['Station', 'Date', 'Time', 'Confirm']

interface Props { stationId: string }

export function BookingWizard({ stationId }: Props) {
  const [step, setStep]   = useState(0)
  const [date, setDate]   = useState<Date>(new Date())
  const [time, setTime]   = useState('')

  return (
    <div className="max-w-md w-full mx-auto">
      {step < 4 && (
        <div className="flex items-center gap-2 mb-6">
          {STEP_LABELS.map((label, i) => (
            <div key={label} className="flex items-center gap-2">
              <div className={`w-6 h-6 rounded-full text-xs font-bold flex items-center justify-center ${i < step ? 'bg-green-500 text-white' : i === step ? 'bg-laerdal-red text-white' : 'bg-content-border text-text-secondary'}`}>
                {i < step ? '✓' : i + 1}
              </div>
              <span className={`text-xs font-medium ${i === step ? 'text-text-primary' : 'text-text-secondary'}`}>{label}</span>
              {i < STEP_LABELS.length - 1 && <div className="h-px w-4 bg-content-border" />}
            </div>
          ))}
        </div>
      )}

      {step === 0 && <StepStation stationId={stationId} onNext={() => setStep(1)} />}
      {step === 1 && <StepDate    onNext={d => { setDate(d); setStep(2) }} />}
      {step === 2 && <StepTime    onNext={t => { setTime(t); setStep(3) }} />}
      {step === 3 && <StepConfirm stationId={stationId} date={date} time={time} onConfirm={() => setStep(4)} />}
      {step === 4 && <StepSuccess />}
    </div>
  )
}
