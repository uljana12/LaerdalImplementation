import { useSearchParams } from 'react-router-dom'
import { PageHeader }    from '@/components/layout/PageHeader'
import { BookingWizard } from '@/features/cpr-book/BookingWizard'
import { STATIONS }      from '@/features/cpr-discover/mockData'

export function BookPage() {
  const [params] = useSearchParams()
  const stationId = params.get('station') ?? STATIONS[0].id

  return (
    <div className="flex flex-col flex-1">
      <PageHeader title="Book a Session" subtitle="Reserve your CPR training slot" />
      <div className="p-6 flex justify-center">
        <BookingWizard stationId={stationId} />
      </div>
    </div>
  )
}
