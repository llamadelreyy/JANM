import { ArrowLeft, RefreshCw } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { cn } from '../../utils/cn'

const PageTemplate = ({ 
  title, 
  subtitle, 
  icon: Icon,
  children,
  actions,
  breadcrumbs,
  loading = false,
  onRefresh,
  showBack = false,
  className
}) => {
  const navigate = useNavigate()

  return (
    <div className={cn("min-h-full", className)}>
      {/* Page Header */}
      <div className="bg-white border-b border-slate-200 shadow-sm">
        <div className="px-6 py-6">
          {/* Breadcrumbs */}
          {breadcrumbs && (
            <nav className="mb-4">
              <ol className="flex items-center space-x-2 text-sm">
                {breadcrumbs.map((crumb, index) => (
                  <li key={index} className="flex items-center">
                    {index > 0 && (
                      <span className="mx-2 text-slate-400">/</span>
                    )}
                    {crumb.href ? (
                      <button
                        onClick={() => navigate(crumb.href)}
                        className="text-blue-600 hover:text-blue-800 hover:underline"
                      >
                        {crumb.label}
                      </button>
                    ) : (
                      <span className="text-slate-600">{crumb.label}</span>
                    )}
                  </li>
                ))}
              </ol>
            </nav>
          )}

          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
            {/* Title Section */}
            <div className="flex items-center gap-4">
              {showBack && (
                <button
                  onClick={() => navigate(-1)}
                  className="p-2 text-slate-600 hover:text-slate-900 hover:bg-slate-100 rounded-lg transition-colors"
                  title="Kembali"
                >
                  <ArrowLeft className="h-5 w-5" />
                </button>
              )}
              
              <div className="flex items-center gap-3">
                {Icon && (
                  <div className="p-3 bg-blue-50 text-blue-600 rounded-lg border border-blue-200">
                    <Icon className="h-6 w-6" />
                  </div>
                )}
                <div>
                  <h1 className="text-2xl font-bold text-slate-900">{title}</h1>
                  {subtitle && (
                    <p className="text-slate-600 mt-1">{subtitle}</p>
                  )}
                </div>
              </div>
            </div>

            {/* Actions Section */}
            <div className="flex items-center gap-3">
              {onRefresh && (
                <button
                  onClick={onRefresh}
                  disabled={loading}
                  className="btn-secondary"
                  title="Muat Semula"
                >
                  <RefreshCw className={cn(
                    "h-4 w-4",
                    loading && "animate-spin"
                  )} />
                </button>
              )}
              {actions}
            </div>
          </div>
        </div>
      </div>

      {/* Page Content */}
      <div className="px-6 py-6">
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="loading-spinner h-8 w-8 mx-auto mb-4"></div>
              <p className="text-slate-600">Memuat halaman...</p>
            </div>
          </div>
        ) : (
          children
        )}
      </div>
    </div>
  )
}

// Professional Alert Component
export const Alert = ({ type = 'info', title, children, onClose }) => {
  const typeClasses = {
    info: 'bg-blue-50 border-blue-200 text-blue-800',
    success: 'bg-green-50 border-green-200 text-green-800',
    warning: 'bg-yellow-50 border-yellow-200 text-yellow-800',
    error: 'bg-red-50 border-red-200 text-red-800'
  }

  return (
    <div className={cn(
      "rounded-lg border p-4 mb-6",
      typeClasses[type]
    )}>
      <div className="flex items-start justify-between">
        <div className="flex-1">
          {title && (
            <h4 className="font-medium mb-1">{title}</h4>
          )}
          <div className="text-sm">{children}</div>
        </div>
        {onClose && (
          <button
            onClick={onClose}
            className="ml-4 text-current hover:opacity-70"
          >
            <span className="sr-only">Tutup</span>
            ×
          </button>
        )}
      </div>
    </div>
  )
}

// Professional Empty State Component
export const EmptyState = ({ 
  icon: Icon, 
  title, 
  description, 
  action,
  actionText = "Tambah Data"
}) => {
  return (
    <div className="text-center py-12">
      {Icon && (
        <div className="mx-auto h-12 w-12 text-slate-400 mb-4">
          <Icon className="h-full w-full" />
        </div>
      )}
      <h3 className="text-lg font-medium text-slate-900 mb-2">{title}</h3>
      {description && (
        <p className="text-slate-600 mb-6 max-w-sm mx-auto">{description}</p>
      )}
      {action && (
        <button onClick={action} className="btn-primary">
          {actionText}
        </button>
      )}
    </div>
  )
}

export default PageTemplate