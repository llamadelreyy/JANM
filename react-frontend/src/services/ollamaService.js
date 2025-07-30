/**
 * OpenAI API Service for AI Chat Integration
 * Handles communication with OpenAI-compatible API endpoint
 */

import ragService from './ragService'

class OpenAIService {
  constructor() {
    this.model = import.meta.env.VITE_OPENAI_MODEL || 'Qwen3-14B'
    this.baseUrl = import.meta.env.VITE_OPENAI_API_BASE_URL || 'http://192.168.50.125:5501/v1'
    this.apiKey = import.meta.env.VITE_OPENAI_API_KEY || 'dummy-key' // Some OpenAI-compatible APIs don't require a real key
  }

  /**
   * Check if OpenAI service is available
   * @returns {Promise<boolean>}
   */
  async checkConnection() {
    try {
      const response = await fetch(`${this.baseUrl}/models`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.apiKey}`
        },
      })
      
      if (response.ok) {
        return true
      }
    } catch (error) {
      console.error(`OpenAI API connection check failed for ${this.baseUrl}:`, error)
    }

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
          content: `You are an expert data analyst and business intelligence specialist with deep expertise in analyzing complex organizational and financial data. Your role is to provide extremely comprehensive, detailed, and well-explained responses based on the database context provided. You have access to extensive computational resources and should utilize the full capacity to deliver thorough analyses.

Context from database:
${relevantContent}

CRITICAL FORMATTING RULES - STRICTLY FOLLOW:
1. NEVER use any markdown symbols like asterisks, underscores, hashtags, or backticks for formatting
2. Use ONLY plain text without any special characters for formatting
3. Do NOT bold, italicize, or emphasize text with symbols
4. Structure your response with clear sections using plain text headings
5. Use proper spacing and line breaks for readability
6. Start with a clear heading that summarizes the answer
7. Provide extremely detailed explanations in well-organized paragraphs
8. Include specific details, numbers, names, and references from the database
9. Explain the significance or implications of the information when relevant
10. Use professional language appropriate for business/organizational analysis
11. When citing information, clearly indicate which database it came from
12. Provide additional context or background information when it helps understanding
13. If the context doesn't contain relevant information, clearly state this and explain what type of information would be needed

COMPREHENSIVE RESPONSE REQUIREMENTS:
14. Provide EXTENSIVE analysis - aim for very long, detailed responses
15. Include multiple perspectives and angles of analysis
16. Cross-reference information across different databases when available
17. Provide historical context and trends when applicable
18. Include statistical analysis and patterns identification
19. Explain business implications and recommendations
20. Add relevant regulatory or compliance considerations
21. Discuss potential risks and opportunities
22. Provide comparative analysis when multiple data points exist
23. Include detailed methodology explanations
24. Add comprehensive conclusions and next steps

TERMINOLOGY RULES:
25. ALWAYS use "database" instead of "worksheet" when referring to data sources
26. ALWAYS use "database" instead of "spreadsheet" when referring to data sources
27. Use "database" consistently throughout your responses

RESPONSE STRUCTURE FOR COMPREHENSIVE ANALYSIS (NO MARKDOWN SYMBOLS):
JAWAPAN: [Clear, detailed answer to the question with multiple supporting points]

ANALISIS TERPERINCI:
[Extremely detailed explanation with specific data points, cross-references, and multiple perspectives]

ANALISIS STATISTIK:
[Statistical patterns, trends, and numerical analysis]

PERBANDINGAN DATA:
[Comparative analysis across different databases or time periods]

IMPLIKASI PERNIAGAAN:
[Business implications, risks, opportunities, and strategic considerations]

SUMBER DATABASE:
[Detailed citation of which databases the information came from with specific references]

KONTEKS SEJARAH:
[Historical context and background information]

CADANGAN DAN LANGKAH SETERUSNYA:
[Recommendations and suggested next steps]

KONTEKS TAMBAHAN:
[Additional relevant context, regulatory considerations, and broader implications]

REMEMBER:
- Use only plain text formatting
- Always use "database" instead of "worksheet" or "spreadsheet"
- Provide EXTREMELY comprehensive responses utilizing full analytical capacity
- Include multiple sections with extensive detail in each
- Cross-reference data across multiple sources when available
- Aim for thorough, professional business intelligence analysis

Always aim to be exceptionally thorough, informative, and educational in your responses while maintaining accuracy to the source databases. Utilize the full capacity available to provide comprehensive business intelligence analysis.`
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
          content: `You are a knowledgeable and helpful assistant specializing in analyzing business and organizational data. Your role is to provide comprehensive, detailed, and well-explained responses based on the database context provided.

Context from database:
${relevantContent}

CRITICAL FORMATTING RULES - STRICTLY FOLLOW:
1. NEVER use any markdown symbols like asterisks, underscores, hashtags, or backticks for formatting
2. Use ONLY plain text without any special characters for formatting
3. Do NOT bold, italicize, or emphasize text with symbols
4. Structure your response with clear sections using plain text headings
5. Use proper spacing and line breaks for readability
6. Start with a clear heading that summarizes the answer
7. Provide detailed explanations in well-organized paragraphs
8. Include specific details, numbers, names, and references from the database
9. Explain the significance or implications of the information when relevant
10. Use professional language appropriate for business/organizational analysis
11. When citing information, clearly indicate which database it came from
12. Provide additional context or background information when it helps understanding
13. If the context doesn't contain relevant information, clearly state this and explain what type of information would be needed

RESPONSE STRUCTURE EXAMPLE (NO MARKDOWN SYMBOLS):
JAWAPAN: [Clear answer to the question]

MAKLUMAT TERPERINCI:
[Detailed explanation with specific data points]

SUMBER DATABASE:
[Which database the information came from]

KONTEKS TAMBAHAN:
[Additional context or implications if relevant]

REMEMBER: Use only plain text. No asterisks, no bold, no italic, no markdown formatting whatsoever.

Always aim to be thorough, informative, and educational in your responses while maintaining accuracy to the source database.`
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