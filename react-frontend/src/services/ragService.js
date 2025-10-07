/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      roadTransportRegulations: null
    }
    this.processedChunks = []
    this.isLoaded = false
  }

  /**
   * Load database from the public files
   */
  async loadDocuments() {
    try {
      // Load road transport regulations file
      const roadTransportResponse = await fetch('/kuala-kurau')
      if (roadTransportResponse.ok) {
        this.documents.roadTransportRegulations = await roadTransportResponse.text()
        // Preprocess document into searchable chunks for faster retrieval
        this.preprocessDocument()
      }

      this.isLoaded = true
      console.log('RAG database loaded and preprocessed successfully')
    } catch (error) {
      console.error('Failed to load RAG database:', error)
      this.isLoaded = false
    }
  }

  /**
   * Preprocess document into searchable chunks for faster retrieval
   */
  preprocessDocument() {
    if (!this.documents.roadTransportRegulations) return

    const lines = this.documents.roadTransportRegulations.split('\n')
    this.processedChunks = []
    
    // Create chunks of related content (every 8-12 lines)
    for (let i = 0; i < lines.length; i += 8) {
      const chunkLines = lines.slice(i, i + 12)
      const chunkText = chunkLines.join('\n').trim()
      
      if (chunkText.length > 20) { // Skip very short chunks
        this.processedChunks.push({
          content: chunkText,
          lowerContent: chunkText.toLowerCase(),
          startLine: i,
          endLine: Math.min(i + 12, lines.length)
        })
      }
    }
    
    console.log(`Preprocessed ${this.processedChunks.length} chunks for fast retrieval`)
  }

  /**
   * Search for relevant content based on query
   * @param {string} query - User's search query
   * @returns {string} - Relevant content found
   */
  searchDocuments(query) {
    if (!this.isLoaded || this.processedChunks.length === 0) {
      return 'Database not loaded yet. Please wait...'
    }

    const startTime = performance.now()
    
    // Optimize search terms preprocessing
    const queryLower = query.toLowerCase()
    const searchTerms = queryLower.split(/\s+/).filter(term => term.length > 2)
    const numberMatches = query.match(/\d+/g) || []
    
    let relevantContent = []
    let processedChunks = 0

    // Fast chunk-based search with early termination
    for (const chunk of this.processedChunks) {
      processedChunks++
      
      // Quick relevance check
      let relevance = 0
      let hasMatch = false
      
      // Check for exact query match (highest priority)
      if (chunk.lowerContent.includes(queryLower)) {
        relevance += 20
        hasMatch = true
      }
      
      // Check search terms
      const termMatches = searchTerms.filter(term => chunk.lowerContent.includes(term))
      if (termMatches.length > 0) {
        relevance += termMatches.length * 3
        hasMatch = true
      }
      
      // Check number matches
      const chunkNumberMatches = numberMatches.filter(num => chunk.content.includes(num))
      if (chunkNumberMatches.length > 0) {
        relevance += chunkNumberMatches.length * 5
        hasMatch = true
      }
      
      if (hasMatch) {
        relevantContent.push({
          source: 'Kuala Kurau Database',
          content: chunk.content,
          relevance: relevance
        })
      }
      
      // Early termination if we have enough high-quality results
      if (relevantContent.length >= 15 && relevance < 5) {
        break
      }
    }

    // Sort by relevance and limit to top results for faster processing
    relevantContent.sort((a, b) => b.relevance - a.relevance)
    relevantContent = relevantContent.slice(0, 8) // Reduced to 8 for faster LLM processing

    const endTime = performance.now()
    console.log(`RAG search completed in ${(endTime - startTime).toFixed(2)}ms, processed ${processedChunks} chunks, found ${relevantContent.length} results`)

    if (relevantContent.length === 0) {
      return 'No relevant information found in the database.'
    }

    // Optimized result formatting
    let formattedResults = 'Relevant information found:\n\n'
    relevantContent.forEach((item, index) => {
      formattedResults += `${index + 1}. From ${item.source}:\n${item.content}\n\n`
    })

    return formattedResults
  }

  /**
   * Get document status
   */
  getStatus() {
    return {
      isLoaded: this.isLoaded,
      hasRoadTransportRegulations: !!this.documents.roadTransportRegulations,
      roadTransportRegulationsSize: this.documents.roadTransportRegulations ? this.documents.roadTransportRegulations.length : 0
    }
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }