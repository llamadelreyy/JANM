/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      roadTransportRegulations: null,
      aadk: null,
      jpn: null,
      kdnFaq: null,
      mmea: null,
      pdrm: null,
      relaFaq: null,
      ros: null
    }
    this.processedChunks = []
    this.isLoaded = false
  }

  /**
   * Load database from the public files
   */
  async loadDocuments() {
    try {
      // Load all available datasets
      const datasets = [
        { key: 'roadTransportRegulations', url: '/kuala-kurau', name: 'KDN Database' },
        { key: 'aadk', url: '/AADK', name: 'AADK Database' },
        { key: 'jpn', url: '/JPN', name: 'JPN Database' },
        { key: 'kdnFaq', url: '/KDN FAQ', name: 'KDN FAQ Database' },
        { key: 'mmea', url: '/MMEA', name: 'MMEA Database' },
        { key: 'pdrm', url: '/PDRM', name: 'PDRM Database' },
        { key: 'relaFaq', url: '/RELA - FAQ', name: 'RELA FAQ Database' },
        { key: 'ros', url: '/ROS', name: 'ROS Database' }
      ]

      // Load all datasets in parallel for better performance
      const loadPromises = datasets.map(async (dataset) => {
        try {
          const response = await fetch(dataset.url)
          if (response.ok) {
            this.documents[dataset.key] = await response.text()
            console.log(`Loaded ${dataset.name}: ${this.documents[dataset.key].length} characters`)
            return { success: true, name: dataset.name }
          } else {
            console.warn(`Failed to load ${dataset.name}: ${response.status}`)
            return { success: false, name: dataset.name }
          }
        } catch (error) {
          console.warn(`Error loading ${dataset.name}:`, error)
          return { success: false, name: dataset.name }
        }
      })

      const results = await Promise.all(loadPromises)
      const successCount = results.filter(r => r.success).length
      
      // Preprocess all loaded documents into searchable chunks
      this.preprocessDocuments()

      this.isLoaded = true
      console.log(`RAG database loaded successfully: ${successCount}/${datasets.length} datasets`)
    } catch (error) {
      console.error('Failed to load RAG database:', error)
      this.isLoaded = false
    }
  }

  /**
   * Preprocess all documents into searchable chunks for faster retrieval
   */
  preprocessDocuments() {
    this.processedChunks = []
    
    // Process each loaded document
    Object.entries(this.documents).forEach(([key, content]) => {
      if (!content) return
      
      const sourceName = this.getSourceName(key)
      const lines = content.split('\n')
      
      // Create chunks of related content (every 8-12 lines)
      for (let i = 0; i < lines.length; i += 8) {
        const chunkLines = lines.slice(i, i + 12)
        const chunkText = chunkLines.join('\n').trim()
        
        if (chunkText.length > 20) { // Skip very short chunks
          this.processedChunks.push({
            content: chunkText,
            lowerContent: chunkText.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: Math.min(i + 12, lines.length)
          })
        }
      }
    })
    
    console.log(`Preprocessed ${this.processedChunks.length} chunks from all datasets for fast retrieval`)
  }

  /**
   * Get human-readable source name for dataset key
   */
  getSourceName(key) {
    const sourceNames = {
      roadTransportRegulations: 'KDN Database',
      aadk: 'AADK Database',
      jpn: 'JPN Database',
      kdnFaq: 'KDN FAQ Database',
      mmea: 'MMEA Database',
      pdrm: 'PDRM Database',
      relaFaq: 'RELA FAQ Database',
      ros: 'ROS Database'
    }
    return sourceNames[key] || key
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
          source: chunk.source,
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
    const loadedDatasets = Object.entries(this.documents)
      .filter(([key, content]) => !!content)
      .map(([key, content]) => ({
        name: this.getSourceName(key),
        size: content.length
      }))

    return {
      isLoaded: this.isLoaded,
      loadedDatasets: loadedDatasets,
      totalChunks: this.processedChunks.length,
      totalSize: Object.values(this.documents).reduce((sum, doc) => sum + (doc ? doc.length : 0), 0)
    }
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }