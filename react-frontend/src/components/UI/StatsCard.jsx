import { TrendingUp, TrendingDown, Minus } from 'lucide-react'
import { cn } from '../../utils/cn'

const StatsCard = ({ 
  title, 
  value, 
  subtitle,
  icon: Icon,
  trend,
  trendValue,
  color = 'blue',
  loading = false 
}) => {
  const colorClasses = {
    blue: {
      bg: 'bg-blue-50',
      icon: 'text-blue-600',
      border: 'border-blue-200'
    },
    green: {
      bg: 'bg-green-50',
      icon: 'text-green-600',
      border: 'border-green-200'
    },
    yellow: {
      bg: 'bg-yellow-50',
      icon: 'text-yellow-600',
      border: 'border-yellow-200'
    },
    red: {
      bg: 'bg-red-50',
      icon: 'text-red-600',
      border: 'border-red-200'
    },
    purple: {
      bg: 'bg-purple-50',
      icon: 'text-purple-600',
      border: 'border-purple-200'
    },
    indigo: {
      bg: 'bg-indigo-50',
      icon: 'text-indigo-600',
      border: 'border-indigo-200'
    }
  }

  const getTrendIcon = () => {
    if (trend === 'up') return <TrendingUp className="h-4 w-4" />
    if (trend === 'down') return <TrendingDown className="h-4 w-4" />
    return <Minus className="h-4 w-4" />
  }

  const getTrendColor = () => {
    if (trend === 'up') return 'text-green-600'
    if (trend === 'down') return 'text-red-600'
    return 'text-slate-500'
  }

  if (loading) {
    return (
      <div className="professional-stat-card">
        <div className="animate-pulse">
          <div className="flex items-center justify-between mb-4">
            <div className="h-4 bg-slate-200 rounded w-24"></div>
            <div className="h-8 w-8 bg-slate-200 rounded"></div>
          </div>
          <div className="h-8 bg-slate-200 rounded w-20 mb-2"></div>
          <div className="h-3 bg-slate-200 rounded w-16"></div>
        </div>
      </div>
    )
  }

  const colors = colorClasses[color] || colorClasses.blue

  return (
    <div className="professional-stat-card group">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-sm font-medium text-slate-600 group-hover:text-slate-800 transition-colors">
          {title}
        </h3>
        {Icon && (
          <div className={cn(
            "p-2 rounded-lg transition-all duration-200",
            colors.bg,
            colors.border,
            "border group-hover:scale-110"
          )}>
            <Icon className={cn("h-5 w-5", colors.icon)} />
          </div>
        )}
      </div>
      
      <div className="space-y-2">
        <div className="flex items-baseline gap-2">
          <span className="text-2xl font-bold text-slate-900 professional-number">
            {typeof value === 'number' ? value.toLocaleString('ms-MY') : value}
          </span>
          {trendValue && (
            <div className={cn(
              "flex items-center gap-1 text-sm font-medium",
              getTrendColor()
            )}>
              {getTrendIcon()}
              <span>{trendValue}</span>
            </div>
          )}
        </div>
        
        {subtitle && (
          <p className="text-sm text-slate-500">{subtitle}</p>
        )}
      </div>
    </div>
  )
}

export default StatsCard