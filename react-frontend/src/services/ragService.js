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

    // Enhanced search terms - include original query, split terms, and numbers
    const originalQuery = query.toLowerCase()
    const searchTerms = query.toLowerCase().split(' ').filter(term => term.length > 2)
    
    // Extract numbers from query for better number matching
    const numberMatches = query.match(/\d+/g) || []
    
    let relevantContent = []

    // Search in BWTD data document
    if (this.documents.bwtdData) {
      const bwtdLines = this.documents.bwtdData.split('\n')
      bwtdLines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        const originalLine = line
        
        // Check for exact query match, search terms, or number matches
        const hasExactMatch = lowerLine.includes(originalQuery)
        const hasTermMatch = searchTerms.some(term => lowerLine.includes(term))
        const hasNumberMatch = numberMatches.some(num => originalLine.includes(num))
        
        if (hasExactMatch || hasTermMatch || hasNumberMatch) {
          // Include more context for better understanding
          const start = Math.max(0, index - 3)
          const end = Math.min(bwtdLines.length, index + 4)
          const context = bwtdLines.slice(start, end).join('\n')
          
          let relevance = 0
          if (hasExactMatch) relevance += 10
          if (hasNumberMatch) relevance += 5
          relevance += searchTerms.filter(term => lowerLine.includes(term)).length
          
          relevantContent.push({
            source: 'BWTD 2024 Database',
            content: context,
            relevance: relevance
          })
        }
      })
    }

    // Search in Objective 1 data document
    if (this.documents.obj1Data) {
      const obj1Lines = this.documents.obj1Data.split('\n')
      obj1Lines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        const originalLine = line
        
        // Check for exact query match, search terms, or number matches
        const hasExactMatch = lowerLine.includes(originalQuery)
        const hasTermMatch = searchTerms.some(term => lowerLine.includes(term))
        const hasNumberMatch = numberMatches.some(num => originalLine.includes(num))
        
        if (hasExactMatch || hasTermMatch || hasNumberMatch) {
          // Include more context for better understanding
          const start = Math.max(0, index - 3)
          const end = Math.min(obj1Lines.length, index + 4)
          const context = obj1Lines.slice(start, end).join('\n')
          
          let relevance = 0
          if (hasExactMatch) relevance += 10
          if (hasNumberMatch) relevance += 5
          relevance += searchTerms.filter(term => lowerLine.includes(term)).length
          
          relevantContent.push({
            source: 'Objective 1 Database',
            content: context,
            relevance: relevance
          })
        }
      })
    }

    // Search in objective 2 details document
    if (this.documents.objective2Details) {
      const detailsLines = this.documents.objective2Details.split('\n')
      detailsLines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        const originalLine = line
        
        // Check for exact query match, search terms, or number matches
        const hasExactMatch = lowerLine.includes(originalQuery)
        const hasTermMatch = searchTerms.some(term => lowerLine.includes(term))
        const hasNumberMatch = numberMatches.some(num => originalLine.includes(num))
        
        if (hasExactMatch || hasTermMatch || hasNumberMatch) {
          // Include more context for better understanding
          const start = Math.max(0, index - 3)
          const end = Math.min(detailsLines.length, index + 4)
          const context = detailsLines.slice(start, end).join('\n')
          
          let relevance = 0
          if (hasExactMatch) relevance += 10
          if (hasNumberMatch) relevance += 5
          relevance += searchTerms.filter(term => lowerLine.includes(term)).length
          
          relevantContent.push({
            source: 'Objective 2 Details Database',
            content: context,
            relevance: relevance
          })
        }
      })
    }

    // Search in objective 2 summary document
    if (this.documents.objective2Summary) {
      const summaryLines = this.documents.objective2Summary.split('\n')
      summaryLines.forEach((line, index) => {
        const lowerLine = line.toLowerCase()
        const originalLine = line
        
        // Check for exact query match, search terms, or number matches
        const hasExactMatch = lowerLine.includes(originalQuery)
        const hasTermMatch = searchTerms.some(term => lowerLine.includes(term))
        const hasNumberMatch = numberMatches.some(num => originalLine.includes(num))
        
        if (hasExactMatch || hasTermMatch || hasNumberMatch) {
          // Include more context for better understanding
          const start = Math.max(0, index - 3)
          const end = Math.min(summaryLines.length, index + 4)
          const context = summaryLines.slice(start, end).join('\n')
          
          let relevance = 0
          if (hasExactMatch) relevance += 10
          if (hasNumberMatch) relevance += 5
          relevance += searchTerms.filter(term => lowerLine.includes(term)).length
          
          relevantContent.push({
            source: 'Objective 2 Summary Database',
            content: context,
            relevance: relevance
          })
        }
      })
    }

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