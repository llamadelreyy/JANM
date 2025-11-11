/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      bwtd2024: null,
      obj1Bwtd: null,
      objective2Details: null,
      objective2Summary: null
    }
    this.processedChunks = []
    this.isLoaded = false
  }

  /**
   * Load database from the public files
   */
  async loadDocuments() {
    try {
      // Load all markdown files from public folder
      const documentFiles = [
        { key: 'bwtd2024', path: '/BWTD_2024_06 24072025xlsx_markdown.txt' },
        { key: 'obj1Bwtd', path: '/OBJ1_24_BWTD 24072025_markdown.txt' },
        { key: 'objective2Details', path: '/r_OBJECTIVE_2_Details 22072025 (1)_markdown.txt' },
        { key: 'objective2Summary', path: '/r_OBJECTIVE_2_Summry 24072025_markdown.txt' }
      ]

      const loadPromises = documentFiles.map(async (file) => {
        try {
          const response = await fetch(file.path)
          if (response.ok) {
            this.documents[file.key] = await response.text()
            console.log(`Loaded ${file.key}: ${this.documents[file.key].length} characters`)
          } else {
            console.warn(`Failed to load ${file.path}: ${response.status}`)
          }
        } catch (error) {
          console.error(`Error loading ${file.path}:`, error)
        }
      })

      await Promise.all(loadPromises)
      
      // Preprocess documents into searchable chunks for faster retrieval
      this.preprocessDocuments()
      this.isLoaded = true
      console.log('RAG database loaded and preprocessed successfully')
    } catch (error) {
      console.error('Failed to load RAG database:', error)
      this.isLoaded = false
    }
  }

  /**
   * Preprocess documents into searchable chunks for faster retrieval
   */
  preprocessDocuments() {
    this.processedChunks = []
    
    Object.entries(this.documents).forEach(([docKey, docContent]) => {
      if (!docContent) return

      const lines = docContent.split('\n')
      
      // Create chunks of related content (every 8-12 lines)
      for (let i = 0; i < lines.length; i += 8) {
        const chunkLines = lines.slice(i, i + 12)
        const chunkText = chunkLines.join('\n').trim()
        
        if (chunkText.length > 20) { // Skip very short chunks
          this.processedChunks.push({
            content: chunkText,
            lowerContent: chunkText.toLowerCase(),
            source: docKey,
            startLine: i,
            endLine: Math.min(i + 12, lines.length)
          })
        }
      }
    })
    
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
          source: this.getSourceDisplayName(chunk.source),
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
   * Get display name for document source
   * @param {string} sourceKey - Internal source key
   * @returns {string} - Human readable source name
   */
  getSourceDisplayName(sourceKey) {
    const sourceNames = {
      bwtd2024: 'BWTD 2024 Report',
      obj1Bwtd: 'Objective 1 BWTD Report',
      objective2Details: 'Objective 2 Details Report',
      objective2Summary: 'Objective 2 Summary Report'
    }
    return sourceNames[sourceKey] || sourceKey
  }

  /**
   * Get document status
   */
  getStatus() {
    return {
      isLoaded: this.isLoaded,
      loadedDocuments: Object.keys(this.documents).filter(key => this.documents[key]),
      totalChunks: this.processedChunks.length,
      documentSizes: Object.entries(this.documents).reduce((acc, [key, content]) => {
        acc[key] = content ? content.length : 0
        return acc
      }, {})
    }
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }