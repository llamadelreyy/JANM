import { useState, useRef, useEffect } from 'react'
import { MessageCircle, X, Send, Bot, User, Loader2, Minimize2 } from 'lucide-react'
import { cn } from '../../utils/cn'
import ollamaService from '../../services/ollamaService'
import ragService from '../../services/ragService'
import FormattedMessage from './FormattedMessage'
import pengarahIcon from '../../assets/Pengarah-JSPT.png'

const PublicChatbot = () => {
  const [isOpen, setIsOpen] = useState(false)
  const [messages, setMessages] = useState([
    {
      id: 1,
      type: 'bot',
      content: 'Selamat datang! Saya adalah Tango Kilo. Anda boleh bertanya tentang:\n\n• Cara bayaran saman online\n• Jenis saman yang boleh dibayar\n• Prosedur pembayaran saman individu\n• Prosedur pembayaran saman syarikat\n• Dan banyak lagi!',
      timestamp: new Date()
    }
  ])
  const [inputMessage, setInputMessage] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [isConnected, setIsConnected] = useState(false)
  const messagesEndRef = useRef(null)
  const inputRef = useRef(null)

  useEffect(() => {
    checkConnection()
  }, [])

  useEffect(() => {
    if (isOpen) {
      scrollToBottom()
      inputRef.current?.focus()
    }
  }, [isOpen, messages])

  const checkConnection = async () => {
    try {
      const status = await ollamaService.getStatus()
      setIsConnected(status.isConnected)
      if (status.isConnected) {
        await ragService.loadDocuments()
      }
    } catch (error) {
      console.error('Connection check failed:', error)
      setIsConnected(false)
    }
  }

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }

  const sendMessage = async () => {
    if (!inputMessage.trim() || isLoading) return

    const userMessage = {
      id: Date.now(),
      type: 'user',
      content: inputMessage.trim(),
      timestamp: new Date()
    }

    setMessages(prev => [...prev, userMessage])
    setInputMessage('')
    setIsLoading(true)

    try {
      const response = await ollamaService.sendMessage(userMessage.content)
      const botMessage = {
        id: Date.now() + 1,
        type: 'bot',
        content: response,
        timestamp: new Date()
      }
      setMessages(prev => [...prev, botMessage])
    } catch (error) {
      const errorMessage = {
        id: Date.now() + 1,
        type: 'bot',
        content: 'Maaf, terdapat masalah. Sila cuba lagi.',
        timestamp: new Date(),
        isError: true
      }
      setMessages(prev => [...prev, errorMessage])
    } finally {
      setIsLoading(false)
    }
  }

  const handleKeyPress = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      sendMessage()
    }
  }

  const formatTime = (timestamp) => {
    return new Intl.DateTimeFormat('ms-MY', {
      hour: '2-digit',
      minute: '2-digit'
    }).format(timestamp)
  }

  return (
    <>
      {/* Chat Toggle Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className={cn(
          "fixed bottom-6 right-6 z-50 w-16 h-16 rounded-full shadow-2xl flex items-center justify-center transition-all duration-300 overflow-hidden",
          isOpen
            ? "bg-red-500 hover:bg-red-600"
            : "bg-white hover:scale-110",
          !isConnected && !isOpen && "animate-pulse"
        )}
      >
        {isOpen ? (
          <X className="w-7 h-7 text-white" />
        ) : (
          <img
            src={pengarahIcon}
            alt="Chat"
            className="w-14 h-14 object-contain"
          />
        )}
      </button>

      {/* Chat Popup */}
      {isOpen && (
        <div className="fixed bottom-24 right-6 z-50 w-[380px] h-[550px] max-w-[calc(100vw-48px)] max-h-[calc(100vh-150px)] bg-white rounded-2xl shadow-2xl flex flex-col overflow-hidden border border-gray-200 animate-slideUp">
          {/* Header */}
          <div className="bg-gradient-to-r from-blue-800 to-blue-900 px-4 py-3 flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <div className="w-12 h-12 bg-white/20 rounded-full flex items-center justify-center overflow-hidden">
                <img src={pengarahIcon} alt="PDRM" className="w-12 h-12 object-contain" />
              </div>
              <div>
                <h3 className="text-white font-semibold text-sm">Tango Kilo</h3>
                <div className="flex items-center space-x-1">
                  <div className={cn(
                    "w-2 h-2 rounded-full",
                    isConnected ? "bg-green-400" : "bg-red-400"
                  )} />
                  <span className="text-white/80 text-xs">
                    {isConnected ? 'Online' : 'Offline'}
                  </span>
                </div>
              </div>
            </div>
            <button
              onClick={() => setIsOpen(false)}
              className="text-white/80 hover:text-white transition-colors"
            >
              <Minimize2 className="w-5 h-5" />
            </button>
          </div>

          {/* Messages */}
          <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-50">
            {messages.map((message) => (
              <div
                key={message.id}
                className={cn(
                  "flex",
                  message.type === 'user' ? "justify-end" : "justify-start"
                )}
              >
                <div className={cn(
                  "flex items-end space-x-2 max-w-[85%]",
                  message.type === 'user' ? "flex-row-reverse space-x-reverse" : ""
                )}>
                  <div className={cn(
                    "w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0 overflow-hidden",
                    message.type === 'user' ? "bg-blue-600" : "bg-gray-200"
                  )}>
                    {message.type === 'user' ? (
                      <User className="w-4 h-4 text-white" />
                    ) : (
                      <img src={pengarahIcon} alt="Bot" className="w-6 h-6 object-contain" />
                    )}
                  </div>
                  <div className={cn(
                    "px-4 py-2.5 rounded-2xl text-sm",
                    message.type === 'user' 
                      ? "bg-blue-600 text-white rounded-tr-sm" 
                      : message.isError
                        ? "bg-red-100 text-red-700 rounded-tl-sm"
                        : "bg-white text-gray-800 rounded-tl-sm shadow-sm border border-gray-100"
                  )}>
                    <FormattedMessage content={message.content} />
                    <div className={cn(
                      "text-xs mt-1",
                      message.type === 'user' ? "text-blue-200" : "text-gray-400"
                    )}>
                      {formatTime(message.timestamp)}
                    </div>
                  </div>
                </div>
              </div>
            ))}
            
            {isLoading && (
              <div className="flex justify-start">
                <div className="flex items-end space-x-2">
                  <div className="w-8 h-8 rounded-full bg-gray-200 flex items-center justify-center overflow-hidden">
                    <img src={pengarahIcon} alt="Bot" className="w-6 h-6 object-contain" />
                  </div>
                  <div className="px-4 py-2.5 rounded-2xl rounded-tl-sm bg-white shadow-sm border border-gray-100">
                    <Loader2 className="w-5 h-5 text-blue-600 animate-spin" />
                  </div>
                </div>
              </div>
            )}
            <div ref={messagesEndRef} />
          </div>

          {/* Input */}
          <div className="p-3 bg-white border-t border-gray-100">
            <div className="flex items-center space-x-2">
              <input
                ref={inputRef}
                type="text"
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                onKeyPress={handleKeyPress}
                placeholder="Taip mesej anda..."
                className="flex-1 px-4 py-2.5 bg-gray-100 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:bg-white transition-all"
                disabled={isLoading}
              />
              <button
                onClick={sendMessage}
                disabled={!inputMessage.trim() || isLoading}
                className={cn(
                  "w-10 h-10 rounded-full flex items-center justify-center transition-all",
                  inputMessage.trim() && !isLoading
                    ? "bg-blue-600 hover:bg-blue-700 text-white"
                    : "bg-gray-200 text-gray-400 cursor-not-allowed"
                )}
              >
                <Send className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>
      )}

      <style>{`
        @keyframes slideUp {
          from {
            opacity: 0;
            transform: translateY(20px) scale(0.95);
          }
          to {
            opacity: 1;
            transform: translateY(0) scale(1);
          }
        }
        .animate-slideUp {
          animation: slideUp 0.3s ease-out forwards;
        }
      `}</style>
    </>
  )
}

export default PublicChatbot