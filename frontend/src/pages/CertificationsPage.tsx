import { PageHeader } from '@/components/layout/PageHeader'
import { CERTIFICATIONS, STATUS_STYLE, STATUS_LABEL } from '@/features/cpr-certifications/mockData'

export function CertificationsPage() {
  return (
    <div className="flex flex-col flex-1">
      <PageHeader title="My Certifications" subtitle="Your completed CPR training certificates" />

      <div className="p-6">
        {CERTIFICATIONS.length === 0 ? (
          <p className="text-center text-text-secondary text-sm py-12">
            No certifications yet. <a href="/cpr-finder/discover" className="text-laerdal-red underline">Book a session</a> to get started.
          </p>
        ) : (
          <div className="bg-white rounded-xl border border-content-border overflow-hidden">
            <div
              className="grid bg-slate-50 border-b border-content-border px-5 py-3 text-[10px] font-bold uppercase tracking-widest text-text-secondary"
              style={{ gridTemplateColumns: '2fr 2fr 110px 110px 110px 90px' }}
            >
              <div>Certification</div>
              <div>Station</div>
              <div>Completed</div>
              <div>Expires</div>
              <div>Status</div>
              <div>Download</div>
            </div>
            {CERTIFICATIONS.map(cert => (
              <div
                key={cert.id}
                className="grid items-center border-b border-content-border/60 last:border-0 px-5 py-4 hover:bg-slate-50 text-sm"
                style={{ gridTemplateColumns: '2fr 2fr 110px 110px 110px 90px' }}
              >
                <span className="font-semibold text-text-primary">{cert.type}</span>
                <span className="text-text-secondary text-xs">{cert.station}</span>
                <span className="text-text-secondary text-xs">{new Date(cert.completedAt).toLocaleDateString()}</span>
                <span className="text-text-secondary text-xs">{new Date(cert.expiresAt).toLocaleDateString()}</span>
                <span className={`text-[11px] font-semibold px-2 py-0.5 rounded inline-block ${STATUS_STYLE[cert.status]}`}>
                  {STATUS_LABEL[cert.status]}
                </span>
                <button
                  className="text-xs text-laerdal-red underline hover:no-underline"
                  onClick={() => alert('PDF download coming soon')}
                >
                  Download PDF
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
