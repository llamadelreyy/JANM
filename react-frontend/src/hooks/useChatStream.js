import { useState, useRef, useCallback } from 'react'

/**
 * Custom hook for handling streaming chat responses
 * Provides token-by-token rendering capabilities
 */
const useChatStream = () => {
  const [answer, setAnswer] = useState('')
  const [thinking, setThinking] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  
  const abortControllerRef = useRef(null)
  const accumulatedAnswer = useRef('')

  /**
   * Send a streaming request to the AI service
   * @param {Array} messages - Array of messages in OpenAI format
   * @param {Object} options - Additional options for the request
   */
  const send = useCallback(async (messages, options = {}) => {
    // Reset state
    setAnswer('')
    setError(null)
    setLoading(true)
    setThinking(true)
    accumulatedAnswer.current = ''

    // Create abort controller for cancellation
    abortControllerRef.current = new AbortController()

    try {
      // Get the base URL from environment or use default
      const baseUrl = import.meta.env.VITE_OPENAI_API_BASE_URL || 'http://60.51.17.97:9501/v1'
      const model = import.meta.env.VITE_OPENAI_MODEL || 'llm_model'
      const apiKey = import.meta.env.VITE_OPENAI_API_KEY || 'dummy-key'

      const requestBody = {
        model: model,
        messages: messages,
        temperature: options.temperature || 0.7,
        top_p: options.top_p || 0.9,
        max_tokens: options.max_tokens || 8000,
        stream: true
      }

      const response = await fetch(`${baseUrl}/chat/completions`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${apiKey}`
        },
        body: JSON.stringify(requestBody),
        signal: abortControllerRef.current.signal
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const reader = response.body.getReader()
      const decoder = new TextDecoder()
      
      setThinking(false) // Stop thinking animation once we start receiving data

      while (true) {
        const { done, value } = await reader.read()
        
        if (done) {
          setLoading(false)
          break
        }

        const chunk = decoder.decode(value)
        const lines = chunk.split('\n').filter(line => line.trim())

        for (const line of lines) {
          if (line.startsWith('data: ')) {
            const data = line.slice(6)
            
            if (data === '[DONE]') {
              setLoading(false)
              return
            }
            
            try {
              const parsed = JSON.parse(data)
              const content = parsed.choices?.[0]?.delta?.content
              
              if (content) {
                // Clean streaming content from structured format and markdown
                const cleanContent = cleanStreamingContent(content)
                
                if (cleanContent) {
                  accumulatedAnswer.current += cleanContent
                  setAnswer(accumulatedAnswer.current)
                }
              }
            } catch (parseError) {
              console.warn('Failed to parse chunk:', parseError)
            }
          }
        }
      }
    } catch (err) {
      if (err.name === 'AbortError') {
        console.log('Stream was cancelled')
      } else {
        console.error('Streaming error:', err)
        setError(err.message || 'Failed to get response from AI')
      }
      setLoading(false)
      setThinking(false)
    }
  }, [])

  /**
   * Cancel the current streaming request
   */
  const cancel = useCallback(() => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort()
      setLoading(false)
      setThinking(false)
    }
  }, [])

  /**
   * Reset the hook state
   */
  const reset = useCallback(() => {
    setAnswer('')
    setError(null)
    setLoading(false)
    setThinking(false)
    accumulatedAnswer.current = ''
    
    if (abortControllerRef.current) {
      abortControllerRef.current.abort()
    }
  }, [])

  /**
   * Clean streaming content chunks to remove structured format patterns
   * @param {string} chunk - Streaming content chunk
   * @returns {string} - Cleaned chunk or empty string if it should be filtered
   */
  const cleanStreamingContent = (chunk) => {
    if (!chunk) return chunk

    // Filter out structured format headers completely during streaming
    if (chunk.includes('JAWAPAN:') ||
        chunk.includes('MAKLUMAT TAMBAHAN:') ||
        chunk.includes('SUMBER:')) {
      return '' // Skip these chunks entirely
    }

    // Remove markdown formatting from the chunk
    return removeMarkdownFormatting(chunk)
  }

  /**
   * Remove markdown formatting from text
   * @param {string} text - Text that may contain markdown
   * @returns {string} - Clean text without markdown
   */
  const removeMarkdownFormatting = (text) => {
    if (!text) return text
    
    return text
      // Remove bold formatting
      .replace(/\*\*(.*?)\*\*/g, '$1')
      // Remove italic formatting
      .replace(/\*(.*?)\*/g, '$1')
      // Remove underline formatting
      .replace(/__(.*?)__/g, '$1')
      .replace(/_(.*?)_/g, '$1')
      // Remove code formatting
      .replace(/`(.*?)`/g, '$1')
      // Remove headers
      .replace(/^#{1,6}\s+/gm, '')
      // Remove strikethrough
      .replace(/~~(.*?)~~/g, '$1')
  }

  return {
    answer,
    thinking,
    loading,
    error,
    send,
    cancel,
    reset
  }
}

export default useChatStream