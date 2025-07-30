const LoadingSpinner = ({ size = 'md', text = 'Capaian Data...' }) => {
  const sizeClasses = {
    sm: 'h-4 w-4',
    md: 'h-8 w-8',
    lg: 'h-12 w-12'
  }

  return (
    <div className="flex flex-col items-center justify-center min-h-[200px] space-y-4">
      <div className={`animate-spin rounded-full border-2 border-gray-300 border-t-blue-600 ${sizeClasses[size]}`}></div>
      {text && (
        <p className="text-gray-600 text-sm">{text}</p>
      )}
    </div>
  )
}

export default LoadingSpinner