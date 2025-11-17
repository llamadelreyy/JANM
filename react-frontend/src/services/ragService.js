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
      ros: null,
      upsi: null
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
        { key: 'ros', url: '/ROS', name: 'ROS Database' },
        { key: 'upsi', url: '/UPSI', name: 'UPSI Database' }
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
      
      if (key === 'upsi') {
        // Special handling for UPSI data in CSV format
        // Skip the header row
        const dataLines = lines.slice(1)
        dataLines.forEach((line, i) => {
          if (line.trim().length > 0) {
            const fields = line.split('|')
            // Check if it's a valid data row with all expected fields
            if (fields.length >= 13) {
              // Format all available fields from the UPSI data
              const formattedContent = [
                `Program: ${fields[0]}`,
                `Race: ${fields[1]}`,
                `Religion: ${fields[2]}`,
                `Intake: ${fields[3]}`,
                `Course: ${fields[4]}`,
                `Status: ${fields[5]}`,
                `Birth State: ${fields[6]}`,
                `International: ${fields[7]}`,
                `Gender: ${fields[8]}`,
                `Birth Date: ${fields[9]}`,
                `Intake Date: ${fields[10]}`,
                `Permanent State: ${fields[11]}`,
                `Current State: ${fields[12]}`,
                `Citizen: ${fields[13]}`,
                `PDapatk: ${fields[14]}`,
                `Marital Status: ${fields[15]}`,
                `Level: ${fields[16]}`,
                `Part Time: ${fields[17]}`,
                `Mode: ${fields[18]}`,
                `Graduation Date: ${fields[19]}`,
                `Senate: ${fields[20]}`,
                `SES Category: ${fields[21]}`,
                `Expected Graduation: ${fields[22]}`,
                `Current Year: ${fields[23]}`,
                `Current Semester: ${fields[24]}`
              ].join('\n')

              this.processedChunks.push({
                content: formattedContent,
                lowerContent: formattedContent.toLowerCase(),
                source: sourceName,
                dataset: key,
                startLine: i,
                endLine: i + 1
              })
            }
          }
        })
      } else {
        // Original chunk processing for other datasets
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
      ros: 'ROS Database',
      upsi: 'UPSI Database'
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
    const queryLower = query.toLowerCase()

    // Special handling for aggregate queries on UPSI data (English and Malay)
    if (queryLower.includes('how many') || queryLower.includes('count') ||
        queryLower.includes('berapa') || queryLower.includes('jumlah') ||
        queryLower.includes('kira') || queryLower.includes('bilangan') ||
        queryLower.includes('senarai') || queryLower.includes('cari')) {
      return this.handleAggregateQuery(queryLower)
    }

    // Regular search processing
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

      // Special handling for UPSI dataset
      if (chunk.dataset === 'upsi') {
        // Check for exact query match (highest priority)
        if (chunk.lowerContent.includes(queryLower)) {
          relevance += 20
          hasMatch = true
        }

        // Check each field separately for better matching
        const fields = chunk.content.split('\n')
        fields.forEach(field => {
          const fieldLower = field.toLowerCase()
          if (fieldLower.includes(queryLower)) {
            relevance += 15
            hasMatch = true
          }
          
          // Check individual search terms
          searchTerms.forEach(term => {
            if (fieldLower.includes(term)) {
              relevance += 5
              hasMatch = true
            }
          })
        })

        // Check number matches
        const chunkNumberMatches = numberMatches.filter(num => chunk.content.includes(num))
        if (chunkNumberMatches.length > 0) {
          relevance += chunkNumberMatches.length * 5
          hasMatch = true
        }
      } else {
        // Original relevance calculation for other datasets
        if (chunk.lowerContent.includes(queryLower)) {
          relevance += 20
          hasMatch = true
        }
        
        const termMatches = searchTerms.filter(term => chunk.lowerContent.includes(term))
        if (termMatches.length > 0) {
          relevance += termMatches.length * 3
          hasMatch = true
        }
        
        const chunkNumberMatches = numberMatches.filter(num => chunk.content.includes(num))
        if (chunkNumberMatches.length > 0) {
          relevance += chunkNumberMatches.length * 5
          hasMatch = true
        }
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

  /**
   * Handle aggregate queries for UPSI data
   */
  handleAggregateQuery(query) {
    const upsiChunks = this.processedChunks.filter(chunk => chunk.dataset === 'upsi')
    const queryLower = query.toLowerCase()
    
    // Helper function to count by field
    const countByField = (fieldName, displayName, valueFilter = null) => {
      const counts = {}
      upsiChunks.forEach(chunk => {
        const match = chunk.content.match(new RegExp(`${fieldName}: (.+?)(?:\\n|$)`))
        if (match) {
          const value = match[1]
          counts[value] = (counts[value] || 0) + 1
        }
      })
      
      let response = `Jumlah pelajar mengikut ${displayName}:\n`
      const entries = Object.entries(counts)
        .sort(([,a], [,b]) => b - a)
        
      if (valueFilter) {
        const filtered = entries.filter(([value]) => value.toLowerCase() === valueFilter.toLowerCase())
        if (filtered.length > 0) {
          return `Terdapat ${filtered[0][1]} pelajar ${valueFilter}.`
        }
        return `Tiada pelajar ${valueFilter} dalam pangkalan data.`
      }
      
      entries.forEach(([value, count]) => {
        response += `${value}: ${count} pelajar\n`
      })
      return response
    }

    // Count active students
    if (query.includes('active')) {
      const activeCount = upsiChunks.filter(chunk =>
        chunk.content.toLowerCase().includes('status: active')
      ).length
      return `There are ${activeCount} active students in the database.`
    }

    // Handle various counting queries in English and Malay
    if (queryLower.includes('program') || queryLower.includes('kursus pengajian'))
      return countByField('Program', 'program')
    
    if (queryLower.includes('status') || queryLower.includes('keadaan'))
      return countByField('Status', 'status')
    
    if (queryLower.includes('state') || queryLower.includes('negeri') || queryLower.includes('tempat'))
      return countByField('Current State', 'negeri')
    
    if (queryLower.includes('race') || queryLower.includes('bangsa') || queryLower.includes('kaum'))
      return countByField('Race', 'bangsa')
    
    if (queryLower.includes('religion') || queryLower.includes('agama') || queryLower.includes('ugama'))
      return countByField('Religion', 'agama')
    
    if (queryLower.includes('gender') || queryLower.includes('jantina') || queryLower.includes('lelaki') || queryLower.includes('perempuan'))
      return countByField('Gender', 'jantina')
    
    if (queryLower.includes('international') || queryLower.includes('antarabangsa') || queryLower.includes('luar negara'))
      return countByField('International', 'status antarabangsa')
    
    if (queryLower.includes('mode') || queryLower.includes('mod') || queryLower.includes('cara'))
      return countByField('Mode', 'mod pengajian')
    
    // Special handling for B40/M40/T20 queries with variations
    if (queryLower.includes('b40') || queryLower.includes('b 40') || queryLower.includes('b-40'))
      return countByField('SES Category', 'kategori SES', 'B40')
    
    if (queryLower.includes('m40') || queryLower.includes('m 40') || queryLower.includes('m-40'))
      return countByField('SES Category', 'kategori SES', 'M40')
    
    if (queryLower.includes('t20') || queryLower.includes('t 20') || queryLower.includes('t-20'))
      return countByField('SES Category', 'kategori SES', 'T20')
    
    if (queryLower.includes('ses') || queryLower.includes('category') || queryLower.includes('kategori') ||
        queryLower.includes('ekonomi') || queryLower.includes('pendapatan'))
      return countByField('SES Category', 'kategori SES')
    
    if (queryLower.includes('level') || queryLower.includes('tahap') || queryLower.includes('peringkat'))
      return countByField('Level', 'tahap')
    
    if (queryLower.includes('marital') || queryLower.includes('kahwin') || queryLower.includes('perkahwinan'))
      return countByField('Marital Status', 'status perkahwinan')
    
    if (queryLower.includes('course') || queryLower.includes('kursus') || queryLower.includes('subjek'))
      return countByField('Course', 'kursus')

    return 'Sila nyatakan apa yang ingin anda kira (contoh: pelajar aktif, mengikut program, status, negeri, bangsa, agama, jantina, status antarabangsa, mod pengajian, kategori SES (B40/M40/T20), tahap, status perkahwinan, atau kursus)'
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }

/**
 * UPSI Data Format:
 * 1. Program - Study program (e.g., Diploma, Masters)
 * 2. Race - Student's race
 * 3. Religion - Student's religion
 * 4. Intake - Intake semester (e.g., Apr2020)
 * 5. Course - Course code
 * 6. Status - Student status (Active, Graduated, etc.)
 * 7. Birth State - State of birth
 * 8. International - International student flag (0/1)
 * 9. Gender - Student's gender
 * 10. Birth Date - Date of birth
 * 11. Intake Date - Date of intake
 * 12. Permanent State - Permanent residence state
 * 13. Current State - Current residence state
 * 14. Citizen - Citizenship status
 * 15. PDapatk - PDapatk status (Yes/No)
 * 16. Marital Status - Marital status
 * 17. Level - Study level (L1-L4)
 * 18. Part Time - Part-time status (0/1)
 * 19. Mode - Study mode (Full-Time/Part-Time)
 * 20. Graduation Date - Actual graduation date
 * 21. Senate - Senate reference number
 * 22. SES Category - Socioeconomic status (B40/M40/T20)
 * 23. Expected Graduation - Expected graduation date
 * 24. Current Year - Current year of study
 * 25. Current Semester - Current semester
 */