import { useState, useEffect } from 'react'
import { X, Save, AlertCircle } from 'lucide-react'
import { cn } from '../../utils/cn'

const FormModal = ({ 
  isOpen, 
  onClose, 
  onSubmit, 
  title, 
  fields = [], 
  initialData = {},
  loading = false,
  submitText = "Simpan",
  cancelText = "Batal"
}) => {
  const [formData, setFormData] = useState({})
  const [errors, setErrors] = useState({})

  useEffect(() => {
    if (isOpen) {
      setFormData(initialData)
      setErrors({})
    }
  }, [isOpen, initialData])

  const handleInputChange = (fieldName, value) => {
    setFormData(prev => ({
      ...prev,
      [fieldName]: value
    }))
    
    // Clear error when user starts typing
    if (errors[fieldName]) {
      setErrors(prev => ({
        ...prev,
        [fieldName]: null
      }))
    }
  }

  const validateForm = () => {
    const newErrors = {}
    
    fields.forEach(field => {
      if (field.required && (!formData[field.name] || formData[field.name].toString().trim() === '')) {
        newErrors[field.name] = `${field.label} adalah wajib`
      }
      
      if (field.validation && formData[field.name]) {
        const validationResult = field.validation(formData[field.name])
        if (validationResult !== true) {
          newErrors[field.name] = validationResult
        }
      }
    })
    
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    
    if (validateForm()) {
      onSubmit(formData)
    }
  }

  const renderField = (field) => {
    const hasError = errors[field.name]
    const value = formData[field.name] || ''

    switch (field.type) {
      case 'select':
        return (
          <select
            value={value}
            onChange={(e) => handleInputChange(field.name, e.target.value)}
            className={cn("select", hasError && "border-red-500 focus:ring-red-500")}
            disabled={loading}
          >
            <option value="">Pilih {field.label}</option>
            {field.options?.map(option => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        )
      
      case 'textarea':
        return (
          <textarea
            value={value}
            onChange={(e) => handleInputChange(field.name, e.target.value)}
            placeholder={field.placeholder}
            rows={field.rows || 3}
            className={cn("textarea", hasError && "border-red-500 focus:ring-red-500")}
            disabled={loading}
          />
        )
      
      case 'number':
        return (
          <input
            type="number"
            value={value}
            onChange={(e) => handleInputChange(field.name, e.target.value)}
            placeholder={field.placeholder}
            min={field.min}
            max={field.max}
            step={field.step}
            className={cn("input", hasError && "border-red-500 focus:ring-red-500")}
            disabled={loading}
          />
        )
      
      case 'email':
        return (
          <input
            type="email"
            value={value}
            onChange={(e) => handleInputChange(field.name, e.target.value)}
            placeholder={field.placeholder}
            className={cn("input", hasError && "border-red-500 focus:ring-red-500")}
            disabled={loading}
          />
        )
      
      case 'password':
        return (
          <input
            type="password"
            value={value}
            onChange={(e) => handleInputChange(field.name, e.target.value)}
            placeholder={field.placeholder}
            className={cn("input", hasError && "border-red-500 focus:ring-red-500")}
            disabled={loading}
          />
        )
      
      case 'date':
        return (
          <input
            type="date"
            value={value}
            onChange={(e) => handleInputChange(field.name, e.target.value)}
            className={cn("input", hasError && "border-red-500 focus:ring-red-500")}
            disabled={loading}
          />
        )
      
      default:
        return (
          <input
            type="text"
            value={value}
            onChange={(e) => handleInputChange(field.name, e.target.value)}
            placeholder={field.placeholder}
            className={cn("input", hasError && "border-red-500 focus:ring-red-500")}
            disabled={loading}
          />
        )
    }
  }

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div 
        className="absolute inset-0 bg-black bg-opacity-50"
        onClick={onClose}
      />
      
      {/* Modal */}
      <div className="relative bg-white rounded-lg shadow-xl w-full max-w-md mx-4 max-h-[90vh] overflow-hidden">
        {/* Header */}
        <div className="professional-header">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold">{title}</h3>
            <button
              onClick={onClose}
              className="p-1 hover:bg-white hover:bg-opacity-20 rounded"
              disabled={loading}
            >
              <X className="h-5 w-5" />
            </button>
          </div>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 overflow-y-auto max-h-[calc(90vh-140px)]">
          <div className="space-y-4">
            {fields.map((field) => (
              <div key={field.name}>
                <label className="block text-sm font-medium text-slate-700 mb-1">
                  {field.label}
                  {field.required && <span className="text-red-500 ml-1">*</span>}
                </label>
                
                {renderField(field)}
                
                {errors[field.name] && (
                  <div className="flex items-center mt-1 text-sm text-red-600">
                    <AlertCircle className="h-4 w-4 mr-1" />
                    {errors[field.name]}
                  </div>
                )}
                
                {field.help && (
                  <p className="mt-1 text-xs text-slate-500">{field.help}</p>
                )}
              </div>
            ))}
          </div>
        </form>

        {/* Footer */}
        <div className="px-6 py-4 bg-slate-50 border-t border-slate-200 flex justify-end gap-3">
          <button
            type="button"
            onClick={onClose}
            className="btn-secondary"
            disabled={loading}
          >
            {cancelText}
          </button>
          <button
            onClick={handleSubmit}
            className="btn-primary"
            disabled={loading}
          >
            {loading ? (
              <>
                <div className="loading-spinner h-4 w-4 mr-2" />
                Menyimpan...
              </>
            ) : (
              <>
                <Save className="h-4 w-4 mr-2" />
                {submitText}
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  )
}

export default FormModal