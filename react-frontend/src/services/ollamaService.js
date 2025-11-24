/**
 * OpenAI API Service for AI Chat Integration
 * Handles communication with OpenAI-compatible API endpoint
 */

import ragService from './ragService'

class OpenAIService {
  constructor() {
    this.model = import.meta.env.VITE_OPENAI_MODEL || 'llm_model'
    this.apiKey = import.meta.env.VITE_OPENAI_API_KEY || 'dummy-key' // Some OpenAI-compatible APIs don't require a real key
    
    // Available model endpoints
    this.modelEndpoints = {
      'localhost:11434': '/ollama/chat/completions',
      '9501': '/remote/chat/completions'
    }
    
    // Default to localhost model
    this.baseUrl = this.modelEndpoints['localhost:11434']
  }

  /**
   * Automatically determine the correct base URL based on the current environment
   * @returns {string} The appropriate base URL
   */
  /**
   * Get list of available model endpoints
   * @returns {Object} Map of endpoint names to URLs
   */
  getModelEndpoints() {
    return this.modelEndpoints
  }

  /**
   * Get current model endpoint name
   * @returns {string} Current endpoint name
   */
  getCurrentEndpoint() {
    return Object.entries(this.modelEndpoints)
      .find(([_, url]) => url === this.baseUrl)?.[0] || 'localhost:11434'
  }

  /**
   * Update the base URL (useful for manual configuration)
   * @param {string} newUrl - The new base URL
   */
  setBaseUrl(newUrl) {
    this.baseUrl = newUrl
  }

  /**
   * Check if OpenAI service is available with fallback URLs
   * @returns {Promise<boolean>}
   */
  async checkConnection() {
    try {
      const basePath = this.getCurrentEndpoint() === 'localhost:11434' ? '/ollama' : '/remote'
      console.log(`Trying to connect to: ${basePath}/models`)
      const response = await fetch(`${this.baseUrl.replace('/chat/completions', '')}/models`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.apiKey}`
        },
      })
      
      if (response.ok) {
        console.log(`Successfully connected to: ${this.baseUrl}`)
        return true
      }
      return false
    } catch (error) {
      console.error(`Connection failed for ${this.baseUrl}:`, error)
      return false
    }
  }

  /**
   * Get list of available models
   * @returns {Promise<Array>}
   */
  async getModels() {
    try {
      const basePath = this.getCurrentEndpoint() === 'localhost:11434' ? '/ollama' : '/remote'
      const response = await fetch(`${basePath}/models`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.apiKey}`
        }
      })
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }
      
      const data = await response.json()
      return data.data || []
    } catch (error) {
      console.error('Failed to get models:', error)
      return []
    }
  }

  /**
   * Check if the specified model is available
   * @returns {Promise<boolean>}
   */
  async isModelAvailable() {
    try {
      const models = await this.getModels()
      return models.some(model => model.id === this.model)
    } catch (error) {
      console.error('Failed to check model availability:', error)
      return false
    }
  }

  /**
   * Remove markdown formatting from text
   * @param {string} text - Text that may contain markdown
   * @returns {string} - Clean text without markdown
   */
  removeMarkdownFormatting(text) {
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

  /**
   * Clean response to remove structured format and markdown
   * @param {string} text - Raw response from AI
   * @returns {string} - Cleaned conversational response
   */
  cleanResponse(text) {
    if (!text) return text

    console.log('Original response:', text) // Debug log

    // First remove markdown formatting
    let cleaned = this.removeMarkdownFormatting(text)

    // More aggressive cleaning of structured format
    cleaned = cleaned
      // Remove JAWAPAN: and everything up to the first meaningful content
      .replace(/^JAWAPAN:\s*/i, '')
      // Remove MAKLUMAT TAMBAHAN: section and empty lines after it
      .replace(/\n\s*MAKLUMAT TAMBAHAN:\s*\n*/gi, ' ')
      // Remove SUMBER: section and everything after it
      .replace(/\n\s*SUMBER:\s*[\s\S]*$/gi, '')
      // Clean up multiple spaces and newlines
      .replace(/\n{2,}/g, '\n')
      .replace(/\s{2,}/g, ' ')
      .trim()

    console.log('Cleaned response:', cleaned) // Debug log

    // If the response is too short, provide a better fallback
    if (!cleaned || cleaned.length < 20) {
      const fallback = "Hello! I'm here to help you with information about government services and regulations. What would you like to know?"
      console.log('Using fallback response:', fallback) // Debug log
      return fallback
    }

    return cleaned
  }

  /**
   * Clean streaming content chunks to remove structured format patterns
   * @param {string} chunk - Streaming content chunk
   * @returns {string} - Cleaned chunk or empty string if it should be filtered
   */
  cleanStreamingContent(chunk) {
    if (!chunk) return chunk

    // Filter out structured format headers completely during streaming
    if (chunk.includes('JAWAPAN:') ||
        chunk.includes('MAKLUMAT TAMBAHAN:') ||
        chunk.includes('SUMBER:')) {
      return '' // Skip these chunks entirely
    }

    // Remove markdown formatting from the chunk
    return this.removeMarkdownFormatting(chunk)
  }

  /**
   * Send a message to the AI model with RAG support
   * @param {string} message - The user message
   * @param {Object} options - Additional options
   * @returns {Promise<string>}
   */
  async sendMessage(message, options = {}) {
    try {
      // Search for relevant content using RAG
      const relevantContent = ragService.searchDocuments(message)
      
      // Create messages array for OpenAI API format
      const messages = [
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
          content: message
        }
      ]

      const requestBody = {
        model: this.model,
        messages: messages,
        temperature: options.temperature || 0.7,
        top_p: options.top_p || 0.9,
        max_tokens: options.max_tokens || 8000,
        stream: false
      }

      const response = await fetch(this.baseUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.apiKey}`
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const data = await response.json()
      const content = data.choices?.[0]?.message?.content || 'Maaf, saya tidak dapat memberikan respons.'
      
      // Remove structured format and markdown formatting
      return this.cleanResponse(content)
    } catch (error) {
      console.error('Failed to send message:', error)
      throw new Error('Gagal menghantar mesej ke AI. Sila cuba lagi.')
    }
  }

  /**
   * Send a message with streaming response and RAG support
   * @param {string} message - The user message
   * @param {Function} onChunk - Callback for each chunk of response
   * @param {Object} options - Additional options
   * @returns {Promise<void>}
   */
  async sendMessageStream(message, onChunk, options = {}) {
    try {
      // Search for relevant content using RAG
      const relevantContent = ragService.searchDocuments(message)
      
      // Create messages array for OpenAI API format
      const messages = [
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
          content: message
        }
      ]

      const requestBody = {
        model: this.model,
        messages: messages,
        temperature: options.temperature || 0.7,
        top_p: options.top_p || 0.9,
        max_tokens: options.max_tokens || 120000,
        stream: true
      }

      const response = await fetch(this.baseUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.apiKey}`
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const reader = response.body.getReader()
      const decoder = new TextDecoder()

      while (true) {
        const { done, value } = await reader.read()
        if (done) break

        const chunk = decoder.decode(value)
        const lines = chunk.split('\n').filter(line => line.trim())

        for (const line of lines) {
          if (line.startsWith('data: ')) {
            const data = line.slice(6)
            if (data === '[DONE]') {
              return
            }
            
            try {
              const parsed = JSON.parse(data)
              const content = parsed.choices?.[0]?.delta?.content
              if (content) {
                // Clean streaming content from structured format and markdown
                const cleanContent = this.cleanStreamingContent(content)
                if (cleanContent) {
                  onChunk(cleanContent)
                }
              }
            } catch (parseError) {
              console.warn('Failed to parse chunk:', parseError)
            }
          }
        }
      }
    } catch (error) {
      console.error('Failed to send streaming message:', error)
      throw new Error('Gagal menghantar mesej ke AI. Sila cuba lagi.')
    }
  }

  /**
   * Get service status and information
   * @returns {Promise<Object>}
   */
  async getStatus() {
    try {
      const isConnected = await this.checkConnection()
      const models = isConnected ? await this.getModels() : []
      const isModelAvailable = isConnected ? await this.isModelAvailable() : false

      return {
        isConnected,
        baseUrl: this.baseUrl,
        model: this.model,
        isModelAvailable,
        availableModels: models,
        timestamp: new Date().toISOString()
      }
    } catch (error) {
      console.error('Failed to get status:', error)
      return {
        isConnected: false,
        baseUrl: this.baseUrl,
        model: this.model,
        isModelAvailable: false,
        availableModels: [],
        error: error.message,
        timestamp: new Date().toISOString()
      }
    }
  }
}

// Create and export a singleton instance
const ollamaService = new OpenAIService()
export default ollamaService

// Also export the class for testing or custom instances
export { OpenAIService }