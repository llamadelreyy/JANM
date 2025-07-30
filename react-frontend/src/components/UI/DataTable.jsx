import { useState } from 'react'
import { ChevronUp, ChevronDown, Search, Plus, Edit, Trash2, Eye } from 'lucide-react'
import { cn } from '../../utils/cn'

const DataTable = ({ 
  data = [], 
  columns = [], 
  title,
  subtitle,
  searchable = true,
  sortable = true,
  actions = true,
  onAdd,
  onEdit,
  onDelete,
  onView,
  loading = false
}) => {
  const [searchTerm, setSearchTerm] = useState('')
  const [sortConfig, setSortConfig] = useState({ key: null, direction: 'asc' })

  // Filter data based on search term
  const filteredData = data.filter(item =>
    Object.values(item).some(value =>
      value?.toString().toLowerCase().includes(searchTerm.toLowerCase())
    )
  )

  // Sort data
  const sortedData = [...filteredData].sort((a, b) => {
    if (!sortConfig.key) return 0
    
    const aValue = a[sortConfig.key]
    const bValue = b[sortConfig.key]
    
    if (aValue < bValue) return sortConfig.direction === 'asc' ? -1 : 1
    if (aValue > bValue) return sortConfig.direction === 'asc' ? 1 : -1
    return 0
  })

  const handleSort = (key) => {
    if (!sortable) return
    
    setSortConfig(prevConfig => ({
      key,
      direction: prevConfig.key === key && prevConfig.direction === 'asc' ? 'desc' : 'asc'
    }))
  }

  const getSortIcon = (columnKey) => {
    if (sortConfig.key !== columnKey) return null
    return sortConfig.direction === 'asc' ? 
      <ChevronUp className="h-4 w-4" /> : 
      <ChevronDown className="h-4 w-4" />
  }

  if (loading) {
    return (
      <div className="professional-card">
        <div className="p-8 text-center">
          <div className="loading-spinner h-8 w-8 mx-auto mb-4"></div>
          <p className="text-slate-600">Memuat data...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="professional-card">
      {/* Header */}
      {(title || subtitle || searchable || onAdd) && (
        <div className="card-header">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
            <div>
              {title && <h2 className="card-title">{title}</h2>}
              {subtitle && <p className="card-subtitle">{subtitle}</p>}
            </div>
            
            <div className="flex flex-col sm:flex-row gap-3">
              {searchable && (
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                  <input
                    type="text"
                    placeholder="Cari..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="input pl-10 w-full sm:w-64"
                  />
                </div>
              )}
              
              {onAdd && (
                <button onClick={onAdd} className="btn-primary">
                  <Plus className="h-4 w-4 mr-2" />
                  Tambah
                </button>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Table */}
      <div className="card-content">
        <div className="overflow-x-auto">
          <table className="gov-table">
            <thead>
              <tr>
                {columns.map((column) => (
                  <th
                    key={column.key}
                    className={cn(
                      "gov-table th",
                      sortable && column.sortable !== false && "cursor-pointer hover:bg-slate-100"
                    )}
                    onClick={() => column.sortable !== false && handleSort(column.key)}
                  >
                    <div className="flex items-center justify-between">
                      <span>{column.label}</span>
                      {sortable && column.sortable !== false && (
                        <div className="ml-2">
                          {getSortIcon(column.key)}
                        </div>
                      )}
                    </div>
                  </th>
                ))}
                {actions && (
                  <th className="gov-table th text-center">Tindakan</th>
                )}
              </tr>
            </thead>
            <tbody>
              {sortedData.length === 0 ? (
                <tr>
                  <td 
                    colSpan={columns.length + (actions ? 1 : 0)} 
                    className="gov-table td text-center py-8 text-slate-500"
                  >
                    {searchTerm ? 'Tiada data yang sepadan dengan carian' : 'Tiada data tersedia'}
                  </td>
                </tr>
              ) : (
                sortedData.map((item, index) => (
                  <tr key={item.id || index} className="gov-table tr">
                    {columns.map((column) => (
                      <td key={column.key} className="gov-table td">
                        {column.render ? 
                          column.render(item[column.key], item) : 
                          item[column.key]
                        }
                      </td>
                    ))}
                    {actions && (
                      <td className="gov-table td">
                        <div className="flex items-center justify-center gap-2">
                          {onView && (
                            <button
                              onClick={() => onView(item)}
                              className="p-1 text-blue-600 hover:text-blue-800 hover:bg-blue-50 rounded"
                              title="Lihat"
                            >
                              <Eye className="h-4 w-4" />
                            </button>
                          )}
                          {onEdit && (
                            <button
                              onClick={() => onEdit(item)}
                              className="p-1 text-green-600 hover:text-green-800 hover:bg-green-50 rounded"
                              title="Edit"
                            >
                              <Edit className="h-4 w-4" />
                            </button>
                          )}
                          {onDelete && (
                            <button
                              onClick={() => onDelete(item)}
                              className="p-1 text-red-600 hover:text-red-800 hover:bg-red-50 rounded"
                              title="Padam"
                            >
                              <Trash2 className="h-4 w-4" />
                            </button>
                          )}
                        </div>
                      </td>
                    )}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
        
        {/* Footer with count */}
        {sortedData.length > 0 && (
          <div className="mt-4 pt-4 border-t border-slate-200">
            <p className="text-sm text-slate-600">
              Menunjukkan {sortedData.length} daripada {data.length} rekod
              {searchTerm && ` (ditapis)`}
            </p>
          </div>
        )}
      </div>
    </div>
  )
}

export default DataTable