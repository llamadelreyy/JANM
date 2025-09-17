/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      roadTransportRegulations: null
    }
    this.isLoaded = false
  }

  /**
   * Load database from the public files
   */
  async loadDocuments() {
    try {
      // Load road transport regulations file
      const roadTransportResponse = await fetch('/KAEDAH-KAEDAH PENGANGKUTAN JALAN.txt')
      if (roadTransportResponse.ok) {
        this.documents.roadTransportRegulations = await roadTransportResponse.text()
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

    // Search in road transport regulations document
    if (this.documents.roadTransportRegulations) {
      const roadTransportLines = this.documents.roadTransportRegulations.split('\n')
      roadTransportLines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        const originalLine = line
        
        // Check for exact query match, search terms, or number matches
        const hasExactMatch = lowerLine.includes(originalQuery)
        const hasTermMatch = searchTerms.some(term => lowerLine.includes(term))
        const hasNumberMatch = numberMatches.some(num => originalLine.includes(num))
        
        if (hasExactMatch || hasTermMatch || hasNumberMatch) {
          // Include much more context to capture complete sections and lists
          let start = Math.max(0, index - 10)
          let end = Math.min(roadTransportLines.length, index + 15)
          
          // Look for section boundaries to include complete information
          // Find the start of the current section (look for headers, numbered items, etc.)
          for (let i = index; i >= start; i--) {
            const currentLine = roadTransportLines[i]
            if (currentLine.match(/^#{1,6}\s+/) || // Markdown headers
                currentLine.match(/^\d+\.\s+/) || // Numbered lists
                currentLine.match(/^[A-Z][^a-z]*:/) || // All caps headers with colon
                currentLine.match(/^\*\*[^*]+\*\*/) || // Bold headers
                currentLine.match(/^#{1,6}/) || // Hash headers
                currentLine.match(/^[A-Za-z\s]+:$/) || // Simple headers ending with colon
                currentLine.trim() === '') { // Empty line indicating section break
              if (i < index - 2) { // Don't go too far back
                start = i
                break
              }
            }
          }
          
          // Find the end of the current section (look for next section or natural break)
          for (let i = index; i <= end; i++) {
            const currentLine = roadTransportLines[i] || ''
            const nextLine = roadTransportLines[i + 1] || ''
            
            // Look for natural section endings
            if (i > index + 5 && (
                nextLine.match(/^#{1,6}\s+/) || // Next header
                nextLine.match(/^\d+\.\s+/) || // Next numbered item
                nextLine.match(/^[A-Z][^a-z]*:/) || // Next all caps header
                nextLine.match(/^\*\*[^*]+\*\*/) || // Next bold header
                (currentLine.trim() === '' && nextLine.trim() === '') || // Double empty line
                nextLine.match(/^[A-Za-z\s]+:$/) // Next simple header
              )) {
              end = i + 1
              break
            }
          }
          
          const context = roadTransportLines.slice(start, end).join('\n')
          
          let relevance = 0
          if (hasExactMatch) relevance += 10
          if (hasNumberMatch) relevance += 5
          relevance += searchTerms.filter(term => lowerLine.includes(term)).length
          
          relevantContent.push({
            source: 'Road Transport Regulations Database',
            content: context,
            relevance: relevance
          })
        }
      })
    }

    // Sort by relevance and limit results
    relevantContent.sort((a, b) => b.relevance - a.relevance)
    relevantContent = relevantContent.slice(0, 50) // Top 10 most relevant for better coverage

    if (relevantContent.length === 0) {
      return 'No relevant information found in the database.'
    }

    // Format the results
    let formattedResults = 'Relevant information found:\n\n'
    relevantContent.forEach((item, index) => {
      formattedResults += `${index + 1}. From ${item.source}:\n${item.content}\n\n`
    })
    
    // Log the search results to console
    console.log('RAG Search Results:', relevantContent)

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