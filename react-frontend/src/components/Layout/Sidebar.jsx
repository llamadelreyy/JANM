import { useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import {
  ChevronDown,
  ChevronRight,
  Settings,
  BarChart3,
  FileText,
  Users,
  MapPin,
  Building,
  Shield,
  FileBarChart,
  X,
  Home,
  TrendingUp,
  Database,
  ClipboardList,
  FileSpreadsheet,
  MessageCircle
} from 'lucide-react'
import { cn } from '../../utils/cn'
import mdkLogo from '../../assets/jata-logo.png'

const menuItems = [
  {
    id: 'ai-chat',
    name: 'AI Chat',
    path: '/ai-chat',
    icon: MessageCircle,
  }
]

const Sidebar = ({ isOpen, onClose }) => {
  const [expandedItems, setExpandedItems] = useState([]) // No default expansion needed
  const location = useLocation()

  const toggleExpanded = (itemId) => {
    setExpandedItems(prev => 
      prev.includes(itemId) 
        ? prev.filter(id => id !== itemId)
        : [...prev, itemId]
    )
  }

  const isActive = (path) => {
    return location.pathname === path
  }

  const isParentActive = (children) => {
    if (!children) return false
    return children.some(child => {
      if (child.children) {
        return isParentActive(child.children)
      }
      return isActive(child.path)
    })
  }

  const renderMenuItem = (item, level = 0) => {
    const hasChildren = item.children && item.children.length > 0
    const isExpanded = expandedItems.includes(item.id)
    const Icon = item.icon
    const paddingLeft = level === 0 ? 'pl-4' : level === 1 ? 'pl-8' : 'pl-12'

    if (hasChildren) {
      return (
        <div key={item.id} className="mb-1">
          <button
            onClick={() => toggleExpanded(item.id)}
            className={cn(
              "w-full flex items-center justify-between py-3 text-left text-sm font-medium rounded-lg transition-all duration-200",
              paddingLeft,
              "pr-4",
              isParentActive(item.children)
                ? "bg-blue-50 text-blue-700 border-l-4 border-blue-600"
                : "text-slate-700 hover:bg-slate-50 hover:text-slate-900"
            )}
          >
            <div className="flex items-center space-x-3">
              <Icon className={cn(
                "h-5 w-5 flex-shrink-0",
                isParentActive(item.children) ? "text-blue-600" : "text-slate-500"
              )} />
              <span className="font-medium">{item.name}</span>
            </div>
            {isExpanded ? (
              <ChevronDown className="h-4 w-4 flex-shrink-0 text-slate-400" />
            ) : (
              <ChevronRight className="h-4 w-4 flex-shrink-0 text-slate-400" />
            )}
          </button>
          
          {isExpanded && (
            <div className="mt-1 space-y-1 border-l-2 border-slate-100 ml-4">
              {item.children.map(child => renderMenuItem(child, level + 1))}
            </div>
          )}
        </div>
      )
    }

    return (
      <Link
        key={item.id}
        to={item.path}
        onClick={onClose}
        className={cn(
          "flex items-center space-x-3 py-2.5 text-sm font-medium rounded-lg transition-all duration-200 mb-1",
          paddingLeft,
          "pr-4",
          isActive(item.path)
            ? "bg-blue-600 text-white shadow-sm border-l-4 border-blue-800"
            : "text-slate-700 hover:bg-slate-50 hover:text-slate-900"
        )}
      >
        <Icon className={cn(
          "h-4 w-4 flex-shrink-0",
          isActive(item.path) ? "text-white" : "text-slate-500"
        )} />
        <span className="font-medium">{item.name}</span>
      </Link>
    )
  }

  return (
    <>
      {/* Desktop sidebar */}
      <div className="hidden lg:flex lg:flex-shrink-0">
        <div className="flex flex-col w-64">
          <div className="flex flex-col flex-grow bg-white border-r border-slate-200 shadow-sm overflow-y-auto">
            <div className="flex items-center flex-shrink-0 px-4 py-6 bg-slate-50 border-b border-slate-200">
              <div className="h-10 w-10 bg-white rounded-lg flex items-center justify-center mr-3 border border-slate-200">
                <img src={mdkLogo} alt="MDK Logo" className="h-6 w-6 object-contain" />
              </div>
              <div>
                <h2 className="text-lg font-bold text-slate-900">Sistem Kecerdasan Buatan JANM</h2>
                <p className="text-xs text-slate-600">Sistem AI</p>
              </div>
            </div>
            <nav className="flex-1 px-3 py-4 space-y-1">
              {menuItems.map(item => renderMenuItem(item))}
            </nav>
          </div>
        </div>
      </div>

      {/* Mobile sidebar */}
      <div className={cn(
        "fixed inset-y-0 left-0 z-50 w-64 bg-white transform transition-transform duration-300 ease-in-out lg:hidden shadow-xl",
        isOpen ? "translate-x-0" : "-translate-x-full"
      )}>
        <div className="flex flex-col h-full">
          <div className="flex items-center justify-between flex-shrink-0 px-4 py-4 bg-slate-50 border-b border-slate-200">
            <div className="flex items-center">
              <div className="h-8 w-8 bg-white rounded-lg flex items-center justify-center mr-3 border border-slate-200">
                <img src={mdkLogo} alt="MDK Logo" className="h-5 w-5 object-contain" />
              </div>
              <div>
                <h2 className="text-lg font-bold text-slate-900">Sistem Kecerdasan Buatan JANM</h2>
                <p className="text-xs text-slate-600">Sistem AI</p>
              </div>
            </div>
            <button
              onClick={onClose}
              className="p-2 rounded-lg text-slate-600 hover:text-slate-900 hover:bg-slate-100 transition-colors"
            >
              <X className="h-6 w-6" />
            </button>
          </div>
          <nav className="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
            {menuItems.map(item => renderMenuItem(item))}
          </nav>
        </div>
      </div>
    </>
  )
}

export default Sidebar