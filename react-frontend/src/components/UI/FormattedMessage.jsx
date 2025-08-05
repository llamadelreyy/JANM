import { cn } from '../../utils/cn'

// Component to format AI chat messages with proper typography
const FormattedMessage = ({ content, className, ...props }) => {
  // Function to parse and format text content
  const formatText = (text) => {
    if (!text) return null

    // Split text into lines for processing
    const lines = text.split('\n')
    const formattedElements = []
    let currentIndex = 0

    lines.forEach((line, lineIndex) => {
      const trimmedLine = line.trim()
      
      // Skip empty lines but add spacing
      if (!trimmedLine) {
        formattedElements.push(
          <div key={`empty-${lineIndex}`} className="h-2" />
        )
        return
      }

      // Headers (# ## ###)
      if (trimmedLine.startsWith('### ')) {
        formattedElements.push(
          <h3 key={`h3-${lineIndex}`} className="text-lg font-semibold text-slate-800 mt-4 mb-2 leading-tight">
            {trimmedLine.substring(4)}
          </h3>
        )
      } else if (trimmedLine.startsWith('## ')) {
        formattedElements.push(
          <h2 key={`h2-${lineIndex}`} className="text-xl font-semibold text-slate-800 mt-4 mb-3 leading-tight">
            {trimmedLine.substring(3)}
          </h2>
        )
      } else if (trimmedLine.startsWith('# ')) {
        formattedElements.push(
          <h1 key={`h1-${lineIndex}`} className="text-2xl font-bold text-slate-900 mt-4 mb-3 leading-tight">
            {trimmedLine.substring(2)}
          </h1>
        )
      }
      // Bullet points
      else if (trimmedLine.startsWith('- ') || trimmedLine.startsWith('* ')) {
        formattedElements.push(
          <div key={`bullet-${lineIndex}`} className="flex items-start space-x-2 my-1">
            <span className="text-slate-600 mt-1 text-sm">•</span>
            <span className="text-slate-700 leading-relaxed">
              {formatInlineText(trimmedLine.substring(2))}
            </span>
          </div>
        )
      }
      // Numbered lists
      else if (/^\d+\.\s/.test(trimmedLine)) {
        const match = trimmedLine.match(/^(\d+)\.\s(.*)/)
        if (match) {
          formattedElements.push(
            <div key={`numbered-${lineIndex}`} className="flex items-start space-x-2 my-1">
              <span className="text-slate-600 font-medium text-sm min-w-[1.5rem]">
                {match[1]}.
              </span>
              <span className="text-slate-700 leading-relaxed">
                {formatInlineText(match[2])}
              </span>
            </div>
          )
        }
      }
      // Code blocks (```)
      else if (trimmedLine.startsWith('```')) {
        // Handle code blocks - this is a simple implementation
        formattedElements.push(
          <div key={`code-${lineIndex}`} className="bg-slate-100 border border-slate-200 rounded-md p-3 my-2 font-mono text-sm text-slate-800 overflow-x-auto">
            <code>{trimmedLine.substring(3)}</code>
          </div>
        )
      }
      // Blockquotes
      else if (trimmedLine.startsWith('> ')) {
        formattedElements.push(
          <blockquote key={`quote-${lineIndex}`} className="border-l-4 border-blue-200 pl-4 py-2 my-2 bg-blue-50 text-slate-700 italic">
            {formatInlineText(trimmedLine.substring(2))}
          </blockquote>
        )
      }
      // Regular paragraphs
      else {
        formattedElements.push(
          <p key={`p-${lineIndex}`} className="text-slate-700 leading-relaxed mb-2">
            {formatInlineText(trimmedLine)}
          </p>
        )
      }
    })

    return formattedElements
  }

  // Function to handle inline formatting (bold, italic, code)
  const formatInlineText = (text) => {
    if (!text) return text

    // Split by formatting markers and process
    const parts = []
    let currentText = text
    let partIndex = 0

    // Process bold text (**text**)
    currentText = currentText.replace(/\*\*(.*?)\*\*/g, (match, content) => {
      const placeholder = `__BOLD_${partIndex}__`
      parts.push({
        type: 'bold',
        content,
        placeholder
      })
      partIndex++
      return placeholder
    })

    // Process italic text (*text*)
    currentText = currentText.replace(/\*(.*?)\*/g, (match, content) => {
      const placeholder = `__ITALIC_${partIndex}__`
      parts.push({
        type: 'italic',
        content,
        placeholder
      })
      partIndex++
      return placeholder
    })

    // Process inline code (`code`)
    currentText = currentText.replace(/`(.*?)`/g, (match, content) => {
      const placeholder = `__CODE_${partIndex}__`
      parts.push({
        type: 'code',
        content,
        placeholder
      })
      partIndex++
      return placeholder
    })

    // Split the text and rebuild with React elements
    let result = [currentText]
    
    parts.forEach((part) => {
      result = result.flatMap((item) => {
        if (typeof item === 'string' && item.includes(part.placeholder)) {
          const splitParts = item.split(part.placeholder)
          const element = part.type === 'bold' ? (
            <strong key={part.placeholder} className="font-semibold text-slate-900">
              {part.content}
            </strong>
          ) : part.type === 'italic' ? (
            <em key={part.placeholder} className="italic text-slate-700">
              {part.content}
            </em>
          ) : (
            <code key={part.placeholder} className="bg-slate-100 text-slate-800 px-1 py-0.5 rounded text-sm font-mono">
              {part.content}
            </code>
          )
          
          return [
            splitParts[0],
            element,
            ...splitParts.slice(1)
          ].filter(item => item !== '')
        }
        return item
      })
    })

    return result
  }

  return (
    <div className={cn("formatted-message chat-message-content", className)} {...props}>
      {formatText(content)}
    </div>
  )
}

export default FormattedMessage