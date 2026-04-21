/**
 * OpenAI API Service for AI Chat Integration
 * Handles communication with OpenAI-compatible API endpoint
 */

import ragService from './ragService'

class OpenAIService {
  constructor() {
    this.model = import.meta.env.VITE_OPENAI_MODEL || 'qwen3.5-397b-a17b-fp8-instruct'
    this.apiKey = import.meta.env.VITE_OPENAI_API_KEY || 'dummy-key' // Some OpenAI-compatible APIs don't require a real key
    
    // Auto-detect the appropriate base URL
    this.baseUrl = this.getBaseUrl()
  }

  /**
   * Automatically determine the correct base URL based on the current environment
   * @returns {string} The appropriate base URL
   */
  getBaseUrl() {
    // If environment variable is set, use it
    if (import.meta.env.VITE_OPENAI_API_BASE_URL) {
      return import.meta.env.VITE_OPENAI_API_BASE_URL
    }

    // Use the new model endpoint
    return 'http://60.51.17.97:9999/v1'
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
    // Try the new model endpoint first, then fallbacks
    const urlsToTry = [
      'http://60.51.17.97:9999/v1', // New model endpoint
      this.baseUrl,
      `${window.location.origin}/v1`, // Proxy route
    ]

    for (const url of urlsToTry) {
      try {
        console.log(`Trying to connect to: ${url}`)
        const response = await fetch(`${url}/models`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${this.apiKey}`
          },
        })
        
        if (response.ok) {
          console.log(`Successfully connected to: ${url}`)
          // Update baseUrl to the working one
          this.baseUrl = url
          return true
        }
      } catch (error) {
        console.error(`Connection failed for ${url}:`, error)
      }
    }

    console.error('All connection attempts failed')
    return false
  }

  /**
   * Get list of available models
   * @returns {Promise<Array>}
   */
  async getModels() {
    try {
      const response = await fetch(`${this.baseUrl}/models`, {
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
          content: `You are a helpful FAQ assistant that provides clear, direct answers based on database information. Your role is to give users straightforward, informative responses that are neither too long nor too short.

Context from database:
${relevantContent}

FORMATTING RULES:
1. NEVER use markdown symbols like asterisks, underscores, hashtags, or backticks
2. Use ONLY plain text without special formatting characters
3. Structure responses with clear, simple headings
4. Use proper spacing and line breaks for readability

RESPONSE STYLE:
- Be direct and helpful like an FAQ bot
- Provide clear, concise answers with relevant details
- Include ALL specific information from the database when available
- Keep responses focused and practical
- Use professional but approachable language
- When the database contains lists (a, b, c... or 1, 2, 3...), include ALL items, not just the first few
- If there are points a-f in the database, provide ALL points from a to f
- Don't truncate or summarize complete lists - give the full information

COMPLETENESS REQUIREMENTS:
- Always provide COMPLETE lists and numbered items from the database
- If the database shows points (a) through (f), include ALL points
- If there are steps 1-10, include ALL steps
- Don't cut off information halfway through a list or sequence
- Ensure users get the full answer they need

RESPONSE STRUCTURE (NO MARKDOWN):
JAWAPAN: [Direct answer to the question]

MAKLUMAT TAMBAHAN:
[Key details and context from the database - include ALL relevant points and lists]

SUMBER:
[Which database the information came from]

If the database doesn't contain relevant information, clearly state this and suggest what type of information would be helpful.

Always use "database" instead of "worksheet" or "spreadsheet" when referring to data sources.

Be helpful, accurate, and complete while ensuring the user gets ALL the information they need from the database.`
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

      const response = await fetch(`${this.baseUrl}/chat/completions`, {
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
      
      // Remove any markdown formatting that might still appear
      return this.removeMarkdownFormatting(content)
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
          content: `You are a helpful FAQ assistant that provides clear, direct answers based on database information. Your role is to give users straightforward, informative responses that are neither too long nor too short.

Context from database:
${relevantContent}

FORMATTING RULES:
1. NEVER use markdown symbols like asterisks, underscores, hashtags, or backticks
2. Use ONLY plain text without special formatting characters
3. Structure responses with clear, simple headings
4. Use proper spacing and line breaks for readability

RESPONSE STYLE:
- Be direct and helpful like an FAQ bot
- Provide clear, concise answers with relevant details
- Include ALL specific information from the database when available
- Keep responses focused and practical
- Use professional but approachable language
- When the database contains lists (a, b, c... or 1, 2, 3...), include ALL items, not just the first few
- If there are points a-f in the database, provide ALL points from a to f
- Don't truncate or summarize complete lists - give the full information

COMPLETENESS REQUIREMENTS:
- Always provide COMPLETE lists and numbered items from the database
- If the database shows points (a) through (f), include ALL points
- If there are steps 1-10, include ALL steps
- Don't cut off information halfway through a list or sequence
- Ensure users get the full answer they need

RESPONSE STRUCTURE (NO MARKDOWN):
JAWAPAN: [Direct answer to the question]

MAKLUMAT TAMBAHAN:
[Key details and context from the database - include ALL relevant points and lists]

SUMBER:
[Which database the information came from]

If the database doesn't contain relevant information, clearly state this and suggest what type of information would be helpful.

Always use "database" instead of "worksheet" or "spreadsheet" when referring to data sources.

Be helpful, accurate, and complete while ensuring the user gets ALL the information they need from the database.`
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

      const response = await fetch(`${this.baseUrl}/chat/completions`, {
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
                // Remove markdown formatting from streaming content
                const cleanContent = this.removeMarkdownFormatting(content)
                onChunk(cleanContent)
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