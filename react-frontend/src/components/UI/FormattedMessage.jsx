const FormattedMessage = ({ content, className = '' }) => {
  // Simple message formatter that handles basic markdown-like formatting
  const formatContent = (text) => {
    if (!text) return null
    
    // Split by newlines and process each line
    const lines = text.split('\n')
    
    return lines.map((line, index) => {
      // Handle bold text with **
      const parts = line.split(/(\*\*[^*]+\*\*)/g)
      
      return (
        <span key={index}>
          {parts.map((part, i) => {
            if (part.startsWith('**') && part.endsWith('**')) {
              return <strong key={i} className="font-semibold">{part.slice(2, -2)}</strong>
            }
            return part
          })}
          {index < lines.length - 1 && <br />}
        </span>
      )
    })
  }

  return (
    <p className={`text-sm whitespace-pre-wrap ${className}`}>
      {formatContent(content)}
    </p>
  )
}

export default FormattedMessage