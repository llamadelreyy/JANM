/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      objective2Summary: null,
      objective2Details: null
    }
    this.isLoaded = false
  }

  /**
   * Load database from the public files
   */
  async loadDocuments() {
    try {
      // Load objective 2 summary file
      const objective2SummaryResponse = await fetch('/r_OBJECTIVE_2_Summry 24072025_markdown.txt')
      if (objective2SummaryResponse.ok) {
        this.documents.objective2Summary = await objective2SummaryResponse.text()
      }

      // Load objective 2 details file
      const objective2DetailsResponse = await fetch('/r_OBJECTIVE_2_Details 22072025 (1)_markdown.txt')
      if (objective2DetailsResponse.ok) {
        this.documents.objective2Details = await objective2DetailsResponse.text()
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

    const searchTerms = query.toLowerCase().split(' ').filter(term => term.length > 2)
    let relevantContent = []

    // Search in objective 2 summary document
    if (this.documents.objective2Summary) {
      const summaryLines = this.documents.objective2Summary.split('\n')
      summaryLines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        if (searchTerms.some(term => lowerLine.includes(term))) {
          // Include context (previous and next lines)
          const start = Math.max(0, index - 2)
          const end = Math.min(summaryLines.length, index + 3)
          const context = summaryLines.slice(start, end).join('\n')
          relevantContent.push({
            source: 'Objective 2 Summary Database',
            content: context,
            relevance: searchTerms.filter(term => lowerLine.includes(term)).length
          })
        }
      })
    }

    // Search in objective 2 details document
    if (this.documents.objective2Details) {
      const detailsLines = this.documents.objective2Details.split('\n')
      detailsLines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        if (searchTerms.some(term => lowerLine.includes(term))) {
          // Include context (previous and next lines)
          const start = Math.max(0, index - 2)
          const end = Math.min(detailsLines.length, index + 3)
          const context = detailsLines.slice(start, end).join('\n')
          relevantContent.push({
            source: 'Objective 2 Details Database',
            content: context,
            relevance: searchTerms.filter(term => lowerLine.includes(term)).length
          })
        }
      })
    }

    // Sort by relevance and limit results
    relevantContent.sort((a, b) => b.relevance - a.relevance)
    relevantContent = relevantContent.slice(0, 5) // Top 5 most relevant

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
      hasObjective2Summary: !!this.documents.objective2Summary,
      hasObjective2Details: !!this.documents.objective2Details,
      objective2SummarySize: this.documents.objective2Summary ? this.documents.objective2Summary.length : 0,
      objective2DetailsSize: this.documents.objective2Details ? this.documents.objective2Details.length : 0
    }
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }