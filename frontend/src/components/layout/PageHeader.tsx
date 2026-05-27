interface PageHeaderProps {
  title: string
  subtitle?: string
  action?: React.ReactNode
}

export function PageHeader({ title, subtitle, action }: PageHeaderProps) {
  return (
    <div className="bg-white border-b border-content-border px-6 py-4 flex items-start justify-between flex-shrink-0">
      <div>
        <h1 className="text-xl font-bold text-text-primary">{title}</h1>
        {subtitle && <p className="text-xs text-text-secondary mt-1">{subtitle}</p>}
        <div className="w-9 h-[3px] bg-laerdal-red rounded mt-2" />
      </div>
      {action && <div className="mt-1">{action}</div>}
    </div>
  )
}
