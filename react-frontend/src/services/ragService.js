/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      aadkData: null,
      jpnData: null,
      mmeaData: null,
      pdrmData: null,
      relaFaqData: null,
      rosData: null
    }
    this.isLoaded = false
  }

  /**
   * Load database from the public files
   */
  async loadDocuments() {
    try {
      // Load AADK text file
      const aadkDataResponse = await fetch('/AADK')
      if (aadkDataResponse.ok) {
        this.documents.aadkData = await aadkDataResponse.text()
      }

      // Load JPN text file
      const jpnDataResponse = await fetch('/JPN')
      if (jpnDataResponse.ok) {
        this.documents.jpnData = await jpnDataResponse.text()
      }

      // Load MMEA text file
      const mmeaDataResponse = await fetch('/MMEA')
      if (mmeaDataResponse.ok) {
        this.documents.mmeaData = await mmeaDataResponse.text()
      }

      // Load PDRM text file
      const pdrmDataResponse = await fetch('/PDRM')
      if (pdrmDataResponse.ok) {
        this.documents.pdrmData = await pdrmDataResponse.text()
      }

      // Load RELA FAQ text file
      const relaFaqDataResponse = await fetch('/RELA - FAQ')
      if (relaFaqDataResponse.ok) {
        this.documents.relaFaqData = await relaFaqDataResponse.text()
      }

      // Load ROS text file
      const rosDataResponse = await fetch('/ROS')
      if (rosDataResponse.ok) {
        this.documents.rosData = await rosDataResponse.text()
      }

      this.isLoaded = true
      console.log('RAG database loaded successfully')
    } catch (error) {
      console.error('Failed to load RAG database:', error)
      this.isLoaded = false
    }
  }

  /**
   * Search for relevant content based on query
   * @param {string} query - User's search query
   * @returns {string} - Relevant content found
   */
  searchDocuments(query) {
    if (!this.isLoaded) {
      return 'Database not loaded yet. Please wait...'
    }

    // Enhanced search terms - include original query, split terms, and numbers
    const originalQuery = query.toLowerCase()
    const searchTerms = query.toLowerCase().split(' ').filter(term => term.length > 2)
    
    // Extract numbers from query for better number matching
    const numberMatches = query.match(/\d+/g) || []
    
    let relevantContent = []

    // Helper function to search in a document
    const searchInDocument = (documentData, sourceName) => {
      if (!documentData) return
      
      const lines = documentData.split('\n')
      lines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        const originalLine = line
        
        // Check for exact query match, search terms, or number matches
        const hasExactMatch = lowerLine.includes(originalQuery)
        const hasTermMatch = searchTerms.some(term => lowerLine.includes(term))
        const hasNumberMatch = numberMatches.some(num => originalLine.includes(num))
        
        if (hasExactMatch || hasTermMatch || hasNumberMatch) {
          // Include more context for better understanding
          const start = Math.max(0, index - 3)
          const end = Math.min(lines.length, index + 4)
          const context = lines.slice(start, end).join('\n')
          
          let relevance = 0
          if (hasExactMatch) relevance += 10
          if (hasNumberMatch) relevance += 5
          relevance += searchTerms.filter(term => lowerLine.includes(term)).length
          
          relevantContent.push({
            source: sourceName,
            content: context,
            relevance: relevance
          })
        }
      })
    }

    // Search in all organizational databases
    searchInDocument(this.documents.aadkData, 'AADK Database')
    searchInDocument(this.documents.jpnData, 'JPN Database')
    searchInDocument(this.documents.mmeaData, 'MMEA Database')
    searchInDocument(this.documents.pdrmData, 'PDRM Database')
    searchInDocument(this.documents.relaFaqData, 'RELA FAQ Database')
    searchInDocument(this.documents.rosData, 'ROS Database')

    // Sort by relevance and limit results
    relevantContent.sort((a, b) => b.relevance - a.relevance)
    relevantContent = relevantContent.slice(0, 10) // Top 10 most relevant for better coverage

    if (relevantContent.length === 0) {
      return 'No relevant information found in the database.'
    }

    // Format the results
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
      hasAadkData: !!this.documents.aadkData,
      hasJpnData: !!this.documents.jpnData,
      hasMmeaData: !!this.documents.mmeaData,
      hasPdrmData: !!this.documents.pdrmData,
      hasRelaFaqData: !!this.documents.relaFaqData,
      hasRosData: !!this.documents.rosData,
      aadkDataSize: this.documents.aadkData ? this.documents.aadkData.length : 0,
      jpnDataSize: this.documents.jpnData ? this.documents.jpnData.length : 0,
      mmeaDataSize: this.documents.mmeaData ? this.documents.mmeaData.length : 0,
      pdrmDataSize: this.documents.pdrmData ? this.documents.pdrmData.length : 0,
      relaFaqDataSize: this.documents.relaFaqData ? this.documents.relaFaqData.length : 0,
      rosDataSize: this.documents.rosData ? this.documents.rosData.length : 0
    }
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }