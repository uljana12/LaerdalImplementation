export function MissionBanner() {
  return (
    <div className="mx-6 mb-4 bg-laerdal-navy rounded-lg px-5 py-3.5 flex items-center gap-4 border border-laerdal-red/20">
      <span className="text-2xl flex-shrink-0">💗</span>
      <p className="text-sm text-slate-300 leading-relaxed">
        <span className="text-white font-semibold">One Million Lives by 2030</span>
        {' '}— every booking brings Laerdal closer to its mission of helping save one million more lives a year.
      </p>
    </div>
  )
}
