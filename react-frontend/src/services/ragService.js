/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      bwtdData: null,
      obj1Data: null,
      objective2Details: null,
      objective2Summary: null
    }
    this.isLoaded = false
  }

  /**
   * Load database from the public files
   */
  async loadDocuments() {
    try {
      // Load BWTD data file
      const bwtdDataResponse = await fetch('/BWTD_2024_06 24072025xlsx_markdown.txt')
      if (bwtdDataResponse.ok) {
        this.documents.bwtdData = await bwtdDataResponse.text()
      }

      // Load Objective 1 data file
      const obj1DataResponse = await fetch('/OBJ1_24_BWTD 24072025_markdown.txt')
      if (obj1DataResponse.ok) {
        this.documents.obj1Data = await obj1DataResponse.text()
      }

      // Load objective 2 details file
      const objective2DetailsResponse = await fetch('/r_OBJECTIVE_2_Details 22072025 (1)_markdown.txt')
      if (objective2DetailsResponse.ok) {
        this.documents.objective2Details = await objective2DetailsResponse.text()
      }

      // Load objective 2 summary file
      const objective2SummaryResponse = await fetch('/r_OBJECTIVE_2_Summry 24072025_markdown.txt')
      if (objective2SummaryResponse.ok) {
        this.documents.objective2Summary = await objective2SummaryResponse.text()
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

    // Search in BWTD data document
    if (this.documents.bwtdData) {
      const bwtdLines = this.documents.bwtdData.split('\n')
      bwtdLines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        if (searchTerms.some(term => lowerLine.includes(term))) {
          // Include context (previous and next lines)
          const start = Math.max(0, index - 2)
          const end = Math.min(bwtdLines.length, index + 3)
          const context = bwtdLines.slice(start, end).join('\n')
          relevantContent.push({
            source: 'BWTD 2024 Database',
            content: context,
            relevance: searchTerms.filter(term => lowerLine.includes(term)).length
          })
        }
      })
    }

    // Search in Objective 1 data document
    if (this.documents.obj1Data) {
      const obj1Lines = this.documents.obj1Data.split('\n')
      obj1Lines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        if (searchTerms.some(term => lowerLine.includes(term))) {
          // Include context (previous and next lines)
          const start = Math.max(0, index - 2)
          const end = Math.min(obj1Lines.length, index + 3)
          const context = obj1Lines.slice(start, end).join('\n')
          relevantContent.push({
            source: 'Objective 1 Database',
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
      hasBwtdData: !!this.documents.bwtdData,
      hasObj1Data: !!this.documents.obj1Data,
      hasObjective2Details: !!this.documents.objective2Details,
      hasObjective2Summary: !!this.documents.objective2Summary,
      bwtdDataSize: this.documents.bwtdData ? this.documents.bwtdData.length : 0,
      obj1DataSize: this.documents.obj1Data ? this.documents.obj1Data.length : 0,
      objective2DetailsSize: this.documents.objective2Details ? this.documents.objective2Details.length : 0,
      objective2SummarySize: this.documents.objective2Summary ? this.documents.objective2Summary.length : 0
    }
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }