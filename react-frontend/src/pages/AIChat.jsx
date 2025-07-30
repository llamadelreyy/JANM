import { useState, useRef, useEffect } from 'react'
import { Send, Bot, User, Loader2, MessageCircle, AlertCircle, RefreshCw } from 'lucide-react'
import PageTemplate from '../components/UI/PageTemplate'
import { cn } from '../utils/cn'
import ollamaService from '../services/ollamaService'
import ragService from '../services/ragService'

const AIChat = () => {
  const [messages, setMessages] = useState([
    {
      id: 1,
      type: 'bot',
      content: 'Selamat datang! Saya adalah pembantu AI yang menggunakan model Qwen3-14B dengan akses kepada database Objective 2. Bagaimana saya boleh membantu anda hari ini?',
      timestamp: new Date()
    }
  ])
  const [inputMessage, setInputMessage] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [isConnected, setIsConnected] = useState(false)
  const [connectionError, setConnectionError] = useState('')
  const [ragStatus, setRagStatus] = useState({ isLoaded: false, loading: true })
  const [showUrlConfig, setShowUrlConfig] = useState(false)
  const [customOllamaUrl, setCustomOllamaUrl] = useState('')
  const messagesEndRef = useRef(null)
  const inputRef = useRef(null)

  // Check Ollama connection and load RAG documents on component mount
  useEffect(() => {
    checkOllamaConnection()
    loadRagDocuments()
  }, [])

  // Auto-scroll to bottom when new messages are added
  useEffect(() => {
    scrollToBottom()
  }, [messages])

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }

  const checkOllamaConnection = async () => {
    try {
      const status = await ollamaService.getStatus()
      setIsConnected(status.isConnected)
      
      if (status.isConnected) {
        if (status.isModelAvailable) {
          setConnectionError('')
        } else {
          setConnectionError(`Model ${status.model} tidak tersedia. Sila pastikan model telah dimuat turun.`)
        }
      } else {
        setConnectionError(`Ollama service tidak dapat diakses di ${status.baseUrl}. Pastikan Ollama berjalan dan boleh diakses dari peranti ini.`)
      }
    } catch (error) {
      console.error('Failed to connect to Ollama:', error)
      setIsConnected(false)
      setConnectionError(`Gagal menyemak status Ollama service: ${error.message}`)
    }
  }

  const loadRagDocuments = async () => {
    try {
      setRagStatus({ isLoaded: false, loading: true })
      await ragService.loadDocuments()
      const status = ragService.getStatus()
      setRagStatus({ ...status, loading: false })
    } catch (error) {
      console.error('Failed to load RAG documents:', error)
      setRagStatus({ isLoaded: false, loading: false, error: error.message })
    }
  }

  const updateOllamaUrl = () => {
    if (customOllamaUrl.trim()) {
      ollamaService.baseUrl = customOllamaUrl.trim()
      setShowUrlConfig(false)
      setCustomOllamaUrl('')
      checkOllamaConnection()
    }
  }

  const sendMessage = async () => {
    if (!inputMessage.trim() || isLoading || !isConnected) return

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
      console.error('Error sending message:', error)
      const errorMessage = {
        id: Date.now() + 1,
        type: 'bot',
        content: error.message || 'Maaf, terdapat masalah dalam menghubungi AI. Sila cuba lagi.',
        timestamp: new Date(),
        isError: true
      }
      setMessages(prev => [...prev, errorMessage])
    } finally {
      setIsLoading(false)
      inputRef.current?.focus()
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

  const clearChat = () => {
    setMessages([
      {
        id: 1,
        type: 'bot',
        content: 'Selamat datang! Saya adalah pembantu AI yang menggunakan model Qwen3-14B dengan akses kepada database Objective 2. Bagaimana saya boleh membantu anda hari ini?',
        timestamp: new Date()
      }
    ])
  }

  return (
    <PageTemplate 
      title="AI Chat" 
      subtitle="Berbual dengan AI menggunakan Ollama Qwen2.5" 
      icon={MessageCircle}
    >
      <div className="flex flex-col h-[calc(100vh-200px)] bg-white rounded-lg shadow-sm border border-gray-200">
        {/* Connection Status */}
        <div className={cn(
          "flex items-center justify-between px-4 py-3 border-b",
          isConnected ? "bg-green-50 border-green-200" : "bg-red-50 border-red-200"
        )}>
          <div className="flex items-center space-x-2">
            <div className={cn(
              "w-2 h-2 rounded-full",
              isConnected ? "bg-green-500" : "bg-red-500"
            )} />
            <span className={cn(
              "text-sm font-medium",
              isConnected ? "text-green-700" : "text-red-700"
            )}>
              {isConnected ? 'Tersambung ke OpenAI API' : 'Tidak tersambung'}
            </span>
            {connectionError && (
              <span className="text-xs text-red-600">- {connectionError}</span>
            )}
          </div>
          <div className="flex items-center space-x-2">
            <button
              onClick={checkOllamaConnection}
              className="p-1 text-gray-500 hover:text-gray-700 transition-colors"
              title="Semak sambungan"
            >
              <RefreshCw className="h-4 w-4" />
            </button>
            <button
              onClick={() => setShowUrlConfig(!showUrlConfig)}
              className="px-3 py-1 text-xs bg-blue-100 text-blue-600 rounded hover:bg-blue-200 transition-colors"
            >
              Config URL
            </button>
            <button
              onClick={clearChat}
              className="px-3 py-1 text-xs bg-gray-100 text-gray-600 rounded hover:bg-gray-200 transition-colors"
            >
              Kosongkan Chat
            </button>
          </div>
        </div>

        {/* URL Configuration */}
        {showUrlConfig && (
          <div className="px-4 py-3 bg-yellow-50 border-b border-yellow-200">
            <div className="flex items-center space-x-2">
              <label className="text-sm font-medium text-yellow-800">Ollama URL:</label>
              <input
                type="text"
                value={customOllamaUrl}
                onChange={(e) => setCustomOllamaUrl(e.target.value)}
                placeholder={ollamaService.baseUrl}
                className="flex-1 px-2 py-1 text-xs border border-yellow-300 rounded focus:outline-none focus:ring-1 focus:ring-yellow-500"
              />
              <button
                onClick={updateOllamaUrl}
                className="px-3 py-1 text-xs bg-yellow-600 text-white rounded hover:bg-yellow-700 transition-colors"
              >
                Update
              </button>
            </div>
            <p className="text-xs text-yellow-700 mt-1">
              Contoh: http://192.168.1.100:11434 (ganti dengan IP server anda)
            </p>
          </div>
        )}

        {/* RAG Status */}
        <div className="px-4 py-2 bg-gray-50 border-b text-xs">
          <div className="flex items-center justify-between">
            <span className="text-gray-600">
              RAG Database: {ragStatus.loading ? 'Loading...' : ragStatus.isLoaded ? 'Ready' : 'Not loaded'}
            </span>
            {ragStatus.isLoaded && (
              <span className="text-green-600">
                ✓ Summary: {ragStatus.hasObjective2Summary ? 'Loaded' : 'Missing'} |
                Details: {ragStatus.hasObjective2Details ? 'Loaded' : 'Missing'}
              </span>
            )}
            {ragStatus.error && (
              <span className="text-red-600">Error: {ragStatus.error}</span>
            )}
          </div>
          <div className="text-xs text-gray-500 mt-1">
            OpenAI API URL: {ollamaService.baseUrl}
          </div>
        </div>

        {/* Messages Area */}
        <div className="flex-1 overflow-y-auto p-4 space-y-4">
          {messages.map((message) => (
            <div
              key={message.id}
              className={cn(
                "flex items-start space-x-3",
                message.type === 'user' ? "justify-end" : "justify-start"
              )}
            >
              {message.type === 'bot' && (
                <div className={cn(
                  "flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center",
                  message.isError ? "bg-red-100" : "bg-blue-100"
                )}>
                  {message.isError ? (
                    <AlertCircle className="h-4 w-4 text-red-600" />
                  ) : (
                    <Bot className="h-4 w-4 text-blue-600" />
                  )}
                </div>
              )}
              
              <div className={cn(
                "max-w-xs lg:max-w-md xl:max-w-lg px-4 py-2 rounded-lg",
                message.type === 'user'
                  ? "bg-blue-600 text-white"
                  : message.isError
                  ? "bg-red-50 text-red-800 border border-red-200"
                  : "bg-gray-100 text-gray-800"
              )}>
                <p className="text-sm whitespace-pre-wrap">{message.content}</p>
                <p className={cn(
                  "text-xs mt-1",
                  message.type === 'user' ? "text-blue-100" : "text-gray-500"
                )}>
                  {formatTime(message.timestamp)}
                </p>
              </div>

              {message.type === 'user' && (
                <div className="flex-shrink-0 w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                  <User className="h-4 w-4 text-white" />
                </div>
              )}
            </div>
          ))}
          
          {isLoading && (
            <div className="flex items-start space-x-3">
              <div className="flex-shrink-0 w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
                <Bot className="h-4 w-4 text-blue-600" />
              </div>
              <div className="bg-gray-100 px-4 py-2 rounded-lg">
                <div className="flex items-center space-x-2">
                  <Loader2 className="h-4 w-4 animate-spin text-gray-600" />
                  <span className="text-sm text-gray-600">AI sedang menaip...</span>
                </div>
              </div>
            </div>
          )}
          
          <div ref={messagesEndRef} />
        </div>

        {/* Input Area */}
        <div className="border-t p-4">
          <div className="flex space-x-2">
            <textarea
              ref={inputRef}
              value={inputMessage}
              onChange={(e) => setInputMessage(e.target.value)}
              onKeyPress={handleKeyPress}
              placeholder={isConnected ? "Taip mesej anda di sini..." : "Sila sambung ke OpenAI API terlebih dahulu"}
              disabled={!isConnected || isLoading}
              className="flex-1 resize-none border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-100 disabled:cursor-not-allowed"
              rows="2"
            />
            <button
              onClick={sendMessage}
              disabled={!inputMessage.trim() || !isConnected || isLoading}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors"
            >
              {isLoading ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                <Send className="h-4 w-4" />
              )}
            </button>
          </div>
          <p className="text-xs text-gray-500 mt-2">
            Tekan Enter untuk hantar, Shift+Enter untuk baris baru
          </p>
        </div>
      </div>
    </PageTemplate>
  )
}

export default AIChat