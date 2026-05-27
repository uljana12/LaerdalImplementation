export function Topbar() {
  return (
    <header className="h-12 bg-laerdal-navy border-b border-white/[0.07] flex items-center px-5 gap-3 flex-shrink-0">
      <div className="w-7 h-7 bg-laerdal-red rounded flex items-center justify-center font-black text-sm text-white">
        L
      </div>
      <span className="font-bold text-white text-sm tracking-tight">
        Laerdal <span className="text-laerdal-red">Platform</span>
      </span>
      <div className="flex-1" />
      <span className="text-xs bg-laerdal-red/10 border border-laerdal-red/25 text-red-300 px-3 py-1 rounded-full">
        Laerdal Admin
      </span>
      <div className="w-7 h-7 bg-[#1e3a5f] rounded-full flex items-center justify-center text-xs font-bold text-[#7ec8e3]">
        UH
      </div>
    </header>
  )
}
