import { useState, useRef, useEffect } from 'react'
import { Send, Bot, User, Loader2, AlertCircle, RefreshCw, Square } from 'lucide-react'
import { cn } from '../utils/cn'
import ollamaService from '../services/ollamaService'
import ragService from '../services/ragService'
import FormattedMessage from '../components/UI/FormattedMessage'
import { useAuthStore } from '../stores/authStore'
import useChatStream from '../hooks/useChatStream'

const AIChat = () => {
  const { user } = useAuthStore()
  const userName = user?.fullname || 'Pengguna'
  
  const [messages, setMessages] = useState([
    {
      id: 1,
      type: 'bot',
      content: `Selamat datang ${userName}! Saya adalah AI Assistant dengan akses kepada database organisasi. Bagaimana saya boleh membantu anda hari ini?`,
      timestamp: new Date()
    }
  ])
  const [inputMessage, setInputMessage] = useState('')
  const [isConnected, setIsConnected] = useState(false)
  const [connectionError, setConnectionError] = useState('')
  const [ragStatus, setRagStatus] = useState({ isLoaded: false, loading: true })
  const [showUrlConfig, setShowUrlConfig] = useState(false)
  const [customOllamaUrl, setCustomOllamaUrl] = useState('')
  const [useStreaming, setUseStreaming] = useState(true)
  const messagesEndRef = useRef(null)
  const inputRef = useRef(null)
  
  // Use the streaming hook
  const { answer, thinking, loading: streamLoading, error: streamError, send: sendStream, cancel: cancelStream, reset: resetStream } = useChatStream()

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
        setConnectionError("Database tidak boleh diakses.")
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
      ollamaService.setBaseUrl(customOllamaUrl.trim())
      setShowUrlConfig(false)
      setCustomOllamaUrl('')
      checkOllamaConnection()
    }
  }

  const sendMessage = async () => {
    if (!inputMessage.trim() || streamLoading || !isConnected) return

    const userMessage = {
      id: Date.now(),
      type: 'user',
      content: inputMessage.trim(),
      timestamp: new Date()
    }

    setMessages(prev => [...prev, userMessage])
    const currentMessage = inputMessage.trim()
    setInputMessage('')

    if (useStreaming) {
      // Use streaming for token-by-token rendering
      try {
        // Search for relevant content using RAG
        const relevantContent = ragService.searchDocuments(currentMessage)
        
        // Create messages array for the API
        const apiMessages = [
          {
            role: 'system',
            content: `ABSOLUTELY CRITICAL: DO NOT use "JAWAPAN:", "MAKLUMAT TAMBAHAN:", or "SUMBER:" in your response. These words are BANNED.

WRONG FORMAT (DO NOT USE):
JAWAPAN: Hello! How can I assist you today?
MAKLUMAT TAMBAHAN:
I'm here to help with any questions...
SUMBER: database

CORRECT FORMAT (USE THIS):
Hello! I'm here to help you with any questions about government services, regulations, or municipal affairs. What would you like to know?

You are a helpful conversational assistant. Write naturally like a human would speak.

Available context from database:
${relevantContent}

INSTRUCTIONS:
- Start responses immediately without headers
- Write in natural conversation style
- Be detailed and thorough using all available information
- Use complete sentences and paragraphs
- Include all relevant details, procedures, and requirements
- Explain processes step by step when needed
- Use plain text only - no special symbols or formatting
- If greeting users, just say "Hello!" or "Hi there!" naturally
- Provide comprehensive explanations making full use of available context

Example good responses:
"Hello! I can help you with information about government services and regulations. What specific topic are you interested in?"
"To apply for that permit, you'll need to follow several steps. First, you'll need to gather these documents..."
"The licensing process involves multiple stages. Let me walk you through each one in detail..."

Never use formal section headers. Just write naturally and conversationally.`
          },
          {
            role: 'user',
            content: currentMessage
          }
        ]

        // Add streaming message placeholder with typing animation
        const streamingMessageId = Date.now() + 1
        const streamingMessage = {
          id: streamingMessageId,
          type: 'bot',
          content: '',
          timestamp: new Date(),
          isStreaming: true,
          showTyping: true
        }
        setMessages(prev => [...prev, streamingMessage])

        // Reset the stream state
        resetStream()

        // Send the streaming request
        await sendStream(apiMessages, {
          temperature: 0.7,
          top_p: 0.9,
          max_tokens: 8000
        })

      } catch (error) {
        console.error('Error sending streaming message:', error)
        const errorMessage = {
          id: Date.now() + 1,
          type: 'bot',
          content: error.message || 'Maaf, terdapat masalah dalam menghubungi AI. Sila cuba lagi.',
          timestamp: new Date(),
          isError: true
        }
        setMessages(prev => [...prev, errorMessage])
      }
    } else {
      // Use non-streaming (original method)
      try {
        const response = await ollamaService.sendMessage(currentMessage)
        
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
      }
    }

    inputRef.current?.focus()
  }

  const handleKeyPress = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      sendMessage()
    }
  }

  // Update streaming message content in real-time
  useEffect(() => {
    if (answer && useStreaming) {
      setMessages(prev =>
        prev.map(msg =>
          msg.isStreaming ? {
            ...msg,
            content: answer,
            isStreaming: streamLoading,
            showTyping: answer.length === 0 // Only show typing if no content yet
          } : msg
        )
      )
    }
  }, [answer, streamLoading, useStreaming])

  // Handle stream errors
  useEffect(() => {
    if (streamError) {
      setMessages(prev =>
        prev.map(msg =>
          msg.isStreaming ? {
            ...msg,
            content: streamError,
            isError: true,
            isStreaming: false,
            showTyping: false
          } : msg
        )
      )
    }
  }, [streamError])

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
        content: `Selamat datang ${userName}! Saya adalah pembantu AI yang menggunakan model Qwen3-235B dengan akses kepada database organisasi. Bagaimana saya boleh membantu anda hari ini?`,
        timestamp: new Date()
      }
    ])
  }

  return (
    <div className="h-full bg-white rounded-lg shadow-sm border border-gray-200 flex flex-col overflow-hidden">
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
              {isConnected ? 'Connected to AI Database' : 'Tidak tersambung'}
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
              onClick={() => setUseStreaming(!useStreaming)}
              className={cn(
                "px-3 py-1 text-xs rounded transition-colors",
                useStreaming
                  ? "bg-green-100 text-green-600 hover:bg-green-200"
                  : "bg-gray-100 text-gray-600 hover:bg-gray-200"
              )}
            >
              {useStreaming ? 'Streaming: ON' : 'Streaming: OFF'}
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
              Contoh: http://192.168.1.100:5501/v1 (ganti dengan IP server anda)
            </p>
            <p className="text-xs text-yellow-600 mt-1">
              URL semasa: {ollamaService.baseUrl}
            </p>
          </div>
        )}


        {/* Messages Area - Only this section is scrollable */}
        <div className="flex-1 overflow-y-auto p-4 space-y-4 min-h-0">
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
                "max-w-sm md:max-w-lg lg:max-w-2xl xl:max-w-4xl px-4 py-3 rounded-lg",
                message.type === 'user'
                  ? "bg-blue-600 text-white"
                  : message.isError
                  ? "bg-red-50 text-red-800 border border-red-200"
                  : "bg-gray-50 text-gray-800 border border-gray-200"
              )}>
                {message.showTyping ? (
                  // Typing animation with 3 dots
                  <div className="flex items-center space-x-1">
                    <div className="flex space-x-1">
                      <div className="w-2 h-2 bg-gray-500 rounded-full animate-bounce"></div>
                      <div className="w-2 h-2 bg-gray-500 rounded-full animate-bounce" style={{animationDelay: '0.1s'}}></div>
                      <div className="w-2 h-2 bg-gray-500 rounded-full animate-bounce" style={{animationDelay: '0.2s'}}></div>
                    </div>
                  </div>
                ) : message.type === 'bot' && !message.isError ? (
                  <FormattedMessage
                    content={message.content}
                    className="text-sm"
                  />
                ) : (
                  <p className="text-sm whitespace-pre-wrap">{message.content}</p>
                )}
                <p className={cn(
                  "text-xs mt-2 pt-2 border-t",
                  message.type === 'user'
                    ? "text-blue-100 border-blue-500"
                    : message.isError
                    ? "text-red-500 border-red-200"
                    : "text-gray-500 border-gray-200"
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
          
          
          <div ref={messagesEndRef} />
        </div>

        {/* Input Area - Fixed at bottom */}
        <div className="border-t p-4 flex-shrink-0">
          <div className="flex space-x-3 max-w-6xl mx-auto">
            <textarea
              ref={inputRef}
              value={inputMessage}
              onChange={(e) => setInputMessage(e.target.value)}
              onKeyPress={handleKeyPress}
              placeholder={isConnected ? "Taip mesej anda di sini..." : "Sila sambung ke backend server terlebih dahulu"}
              disabled={!isConnected || streamLoading}
              className="flex-1 resize-none border border-gray-300 rounded-lg px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-100 disabled:cursor-not-allowed"
              rows="3"
            />
            <button
              onClick={sendMessage}
              disabled={!inputMessage.trim() || !isConnected || streamLoading}
              className="px-5 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors"
            >
              {streamLoading ? (
                <Loader2 className="h-5 w-5 animate-spin" />
              ) : (
                <Send className="h-5 w-5" />
              )}
            </button>
          </div>
          <div className="max-w-6xl mx-auto">
            <div className="flex items-center justify-between mt-2">
              <p className="text-xs text-gray-500">
                Tekan Enter untuk hantar, Shift+Enter untuk baris baru
              </p>
              {useStreaming && (
                <p className="text-xs text-green-600">
                  🔄 Token streaming enabled
                </p>
              )}
            </div>
          </div>
        </div>
    </div>
  )
}

export default AIChat