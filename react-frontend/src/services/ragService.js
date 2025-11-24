/**
 * Simple RAG Service for Database Search
 * Handles searching through organizational database files
 */

class RAGService {
  constructor() {
    this.documents = {
      courseMain: null,
      graduationSenate: null,
      programMain: null,
      semesterMain: null,
      studentMainPoc: null,
      studentMode: null,
      studentStatus: null
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
        { key: 'courseMain',        url: '/course_main',         name: 'Course Main Database' },
        { key: 'graduationSenate',  url: '/graduation_senate',   name: 'Graduation Senate Database' },
        { key: 'programMain',       url: '/program_main',        name: 'Program Main Database' },
        { key: 'semesterMain',      url: '/semester_main',       name: 'Semester Main Database' },
        { key: 'studentMainPoc',    url: '/student_main_poc',    name: 'Student Main POC Database' },
        { key: 'studentMode',       url: '/student_mode',        name: 'Student Mode Database' },
        { key: 'studentStatus',     url: '/student_status',      name: 'Student Status Database' }
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
   * Helper: robust row splitter (handles comma or pipe, but prefers comma for data)
   */
  splitRow(line) {
    if (!line) return []
    const trimmed = line.trim()
    // Headers often start with | and contain column names – treat them separately elsewhere
    if (trimmed.includes(','))
      return trimmed.split(',')
    if (trimmed.includes('|'))
      return trimmed.split('|')
    return [trimmed]
  }

  /**
   * Preprocess all documents into searchable chunks for faster retrieval
   * Each dataset gets schema-aware formatting so the LLM can "see" the context.
   */
  preprocessDocuments() {
    this.processedChunks = []
    
    Object.entries(this.documents).forEach(([key, content]) => {
      if (!content) return
      
      const sourceName = this.getSourceName(key)
      const lines = content.split('\n')

      // Skip obvious header / separator lines for data-centric processing
      const dataLines = lines.filter((line, idx) => {
        const t = line.trim()
        if (!t) return false
        // Skip header lines that start with a pipe or contain column headers
        if (idx === 0 && (t.startsWith('|') || t.toLowerCase().includes('sm_program') || t.toLowerCase().includes('pm_program_code') || t.toLowerCase().includes('gs_senate_seq'))) {
          return false
        }
        return true
      })

      /**********************
       * STUDENT MAIN (per-student data) – used for counting
       **********************/
      if (key === 'studentMainPoc') {
        dataLines.forEach((line, i) => {
          const fields = this.splitRow(line)
          // We expect at least Program, Race, Religion, Intake, Course...
          if (fields.length < 5) return

          const safe = (idx) => (idx < fields.length ? fields[idx].trim() : '')

          const program          = safe(0)  // DEGREE, MASTER, DIPLOMA, DPSP, etc.
          const race             = safe(1)
          const religion         = safe(2)
          const intake           = safe(3)  // e.g. A041, M221PCS2 (intake semester code)
          const course           = safe(4)  // e.g. AT03, ET01
          const statusCode       = safe(5)
          const birthState       = safe(6)
          const international    = safe(7)
          const gender           = safe(8)
          const birthDate        = safe(9)
          const intakeDate       = safe(10)
          const permanentState   = safe(11)
          const currentState     = safe(12)
          const citizen          = safe(13)
          const pDapatk          = safe(14)
          const maritalStatus    = safe(15)
          const level            = safe(16)
          const partTime         = safe(17)
          const mode             = safe(18)
          const graduationDate   = safe(19)
          const senateRef        = safe(20)

          const linesOut = []
          if (program)        linesOut.push(`Program: ${program}`)
          if (race)           linesOut.push(`Race: ${race}`)
          if (religion)       linesOut.push(`Religion: ${religion}`)
          if (intake)         linesOut.push(`Intake: ${intake}`)
          if (course)         linesOut.push(`Course: ${course}`)
          if (statusCode)     linesOut.push(`Status: ${statusCode}`)
          if (birthState)     linesOut.push(`Birth State: ${birthState}`)
          if (international)  linesOut.push(`International: ${international}`)
          if (gender)         linesOut.push(`Gender: ${gender}`)
          if (birthDate)      linesOut.push(`Birth Date: ${birthDate}`)
          if (intakeDate)     linesOut.push(`Intake Date: ${intakeDate}`)
          if (permanentState) linesOut.push(`Permanent State: ${permanentState}`)
          if (currentState)   linesOut.push(`Current State: ${currentState}`)
          if (citizen)        linesOut.push(`Citizen: ${citizen}`)
          if (pDapatk)        linesOut.push(`PDapatk: ${pDapatk}`)
          if (maritalStatus)  linesOut.push(`Marital Status: ${maritalStatus}`)
          if (level)          linesOut.push(`Level: ${level}`)
          if (partTime)       linesOut.push(`Part Time: ${partTime}`)
          if (mode)           linesOut.push(`Mode: ${mode}`)
          if (graduationDate) linesOut.push(`Graduation Date: ${graduationDate}`)
          if (senateRef)      linesOut.push(`Senate: ${senateRef}`)

          if (linesOut.length === 0) return

          const formattedContent = linesOut.join('\n')
          this.processedChunks.push({
            content: formattedContent,
            lowerContent: formattedContent.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: i + 1
          })
        })

        return
      }

      /**********************
       * STUDENT STATUS – lookup table
       **********************/
      if (key === 'studentStatus') {
        dataLines.forEach((line, i) => {
          const fields = this.splitRow(line)
          if (fields.length < 2) return

          const statusCode     = (fields[0] || '').trim()
          const statusType     = (fields[1] || '').trim() // ACTIVE / INACTIVE
          const statusDesc     = (fields[2] || '').trim()
          const statusCategory = (fields[3] || '').trim()

          const formattedContent =
            `Status Code: ${statusCode}\n` +
            `Status Type: ${statusType}\n` +
            `Description: ${statusDesc}\n` +
            `Category: ${statusCategory}`

          this.processedChunks.push({
            content: formattedContent,
            lowerContent: formattedContent.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: i + 1
          })
        })
        return
      }

      /**********************
       * STUDENT MODE – lookup table
       * mode_id, mode_desc (BM), mode_desc_eng
       **********************/
      if (key === 'studentMode') {
        dataLines.forEach((line, i) => {
          const fields = this.splitRow(line)
          if (fields.length < 2) return

          const modeId      = (fields[0] || '').trim()
          const modeDescBm  = (fields[1] || '').trim()
          const modeDescEng = (fields[2] || '').trim()

          const formattedContent =
            `Mode ID: ${modeId}\n` +
            `Description (BM): ${modeDescBm}\n` +
            `Description (EN): ${modeDescEng}`

          this.processedChunks.push({
            content: formattedContent,
            lowerContent: formattedContent.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: i + 1
          })
        })
        return
      }

      /**********************
       * PROGRAM MAIN – program information / structure
       **********************/
      if (key === 'programMain') {
        dataLines.forEach((line, i) => {
          const fields = this.splitRow(line)
          if (fields.length < 2) return

          const safe = (idx) => (idx < fields.length ? fields[idx].trim() : '')

          const programCode   = safe(0)
          const programDescBm = safe(1)
          const idPrefix      = safe(2)
          const idCount       = safe(3)
          const hierarchy     = safe(4)
          const postgraduate  = safe(5)
          const pjj           = safe(6)
          const minPartTime   = safe(7)
          const maxPartTime   = safe(8)
          const minFullTime   = safe(9)
          const maxFullTime   = safe(10)
          const programDescEn = safe(11)
          const headLevel     = safe(12)

          const linesOut = []
          if (programCode)   linesOut.push(`Program Code: ${programCode}`)
          if (programDescBm) linesOut.push(`Program (BM): ${programDescBm}`)
          if (programDescEn) linesOut.push(`Program (EN): ${programDescEn}`)
          if (postgraduate)  linesOut.push(`Postgraduate: ${postgraduate}`)
          if (pjj)           linesOut.push(`PJJ: ${pjj}`)
          if (minPartTime || maxPartTime)
            linesOut.push(`Part-Time Duration (min-max): ${minPartTime || '-'} - ${maxPartTime || '-'}`)
          if (minFullTime || maxFullTime)
            linesOut.push(`Full-Time Duration (min-max): ${minFullTime || '-'} - ${maxFullTime || '-'}`)
          if (hierarchy)     linesOut.push(`Hierarchy: ${hierarchy}`)
          if (headLevel)     linesOut.push(`Head Level: ${headLevel}`)
          if (idPrefix)      linesOut.push(`ID Prefix: ${idPrefix}`)
          if (idCount)       linesOut.push(`ID Count: ${idCount}`)

          if (!linesOut.length) return

          const formattedContent = linesOut.join('\n')
          this.processedChunks.push({
            content: formattedContent,
            lowerContent: formattedContent.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: i + 1
          })
        })
        return
      }

      /**********************
       * GRADUATION SENATE – senate meeting records
       **********************/
      if (key === 'graduationSenate') {
        dataLines.forEach((line, i) => {
          const fields = this.splitRow(line)
          if (fields.length < 4) return

          const safe = (idx) => (idx < fields.length ? fields[idx].trim() : '')

          const seq         = safe(0)
          const programCode = safe(1)
          const semester    = safe(2)
          const senateDesc  = safe(3)
          const senateDate  = safe(4)
          const senateNo    = safe(5)
          const senateRef   = safe(6)
          const senateEng   = safe(7)

          const linesOut = []
          if (seq)         linesOut.push(`Senate Seq: ${seq}`)
          if (programCode) linesOut.push(`Program: ${programCode}`)
          if (semester)    linesOut.push(`Semester Code: ${semester}`)
          if (senateDesc)  linesOut.push(`Senate Description: ${senateDesc}`)
          if (senateEng)   linesOut.push(`Senate Description (EN): ${senateEng}`)
          if (senateDate)  linesOut.push(`Senate Date: ${senateDate}`)
          if (senateNo)    linesOut.push(`Senate No: ${senateNo}`)
          if (senateRef)   linesOut.push(`Senate Ref: ${senateRef}`)

          if (!linesOut.length) return

          const formattedContent = linesOut.join('\n')
          this.processedChunks.push({
            content: formattedContent,
            lowerContent: formattedContent.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: i + 1
          })
        })
        return
      }

      /**********************
       * SEMESTER MAIN – semester schedules and info
       **********************/
      if (key === 'semesterMain') {
        dataLines.forEach((line, i) => {
          const fields = this.splitRow(line)
          if (fields.length < 3) return

          const safe = (idx) => (idx < fields.length ? fields[idx].trim() : '')

          const semCode        = safe(0)
          const programCode    = safe(1)
          const start          = safe(2)
          const end            = safe(3)
          const semesterDescBm = safe(4)
          const semesterNo     = safe(5)
          const type           = safe(6)
          const session        = safe(8)
          const semesterDescEn = safe(9)
          const startAttend    = safe(10)
          const endAttend      = safe(11)
          const assessmentYear = safe(12)

          const linesOut = []
          if (semCode)        linesOut.push(`Semester Code: ${semCode}`)
          if (programCode)    linesOut.push(`Program Code: ${programCode}`)
          if (semesterNo)     linesOut.push(`Semester No: ${semesterNo}`)
          if (type)           linesOut.push(`Type: ${type}`)
          if (session)        linesOut.push(`Session: ${session}`)
          if (semesterDescBm) linesOut.push(`Description (BM): ${semesterDescBm}`)
          if (semesterDescEn) linesOut.push(`Description (EN): ${semesterDescEn}`)
          if (start)          linesOut.push(`Start Date: ${start}`)
          if (end)            linesOut.push(`End Date: ${end}`)
          if (startAttend || endAttend)
            linesOut.push(`Attendance Window: ${startAttend || '-'} to ${endAttend || '-'}`)
          if (assessmentYear) linesOut.push(`Assessment Year: ${assessmentYear}`)

          if (!linesOut.length) return

          const formattedContent = linesOut.join('\n')
          this.processedChunks.push({
            content: formattedContent,
            lowerContent: formattedContent.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: i + 1
          })
        })
        return
      }

      /**********************
       * COURSE MAIN – treat as generic rows (same schema as student_main_poc in your sample,
       * but we will not use it for counting to avoid double-counting; it's for context only).
       **********************/
      if (key === 'courseMain') {
        dataLines.forEach((line, i) => {
          const fields = this.splitRow(line)
          if (fields.length < 5) return

          const safe = (idx) => (idx < fields.length ? fields[idx].trim() : '')

          const program        = safe(0)
          const race           = safe(1)
          const religion       = safe(2)
          const intake         = safe(3)
          const course         = safe(4)
          const statusCode     = safe(5)
          const birthState     = safe(6)
          const international  = safe(7)
          const gender         = safe(8)
          const birthDate      = safe(9)
          const intakeDate     = safe(10)
          const permanentState = safe(11)
          const currentState   = safe(12)
          const citizen        = safe(13)
          const pDapatk        = safe(14)
          const maritalStatus  = safe(15)
          const level          = safe(16)
          const partTime       = safe(17)
          const mode           = safe(18)
          const graduationDate = safe(19)
          const senateRef      = safe(20)

          const linesOut = []
          if (program)        linesOut.push(`Program: ${program}`)
          if (course)         linesOut.push(`Course: ${course}`)
          if (intake)         linesOut.push(`Intake: ${intake}`)
          if (statusCode)     linesOut.push(`Status: ${statusCode}`)
          if (gender)         linesOut.push(`Gender: ${gender}`)
          if (birthDate)      linesOut.push(`Birth Date: ${birthDate}`)
          if (intakeDate)     linesOut.push(`Intake Date: ${intakeDate}`)
          if (currentState)   linesOut.push(`Current State: ${currentState}`)
          if (graduationDate) linesOut.push(`Graduation Date: ${graduationDate}`)
          if (senateRef)      linesOut.push(`Senate: ${senateRef}`)

          if (!linesOut.length) return

          const formattedContent = linesOut.join('\n')
          this.processedChunks.push({
            content: formattedContent,
            lowerContent: formattedContent.toLowerCase(),
            source: sourceName,
            dataset: key,
            startLine: i,
            endLine: i + 1
          })
        })
        return
      }

      /**********************
       * Fallback: generic chunking for any unknown dataset
       **********************/
      for (let i = 0; i < lines.length; i += 8) {
        const chunkLines = lines.slice(i, i + 12)
        const chunkText = chunkLines.join('\n').trim()
        
        if (chunkText.length > 20) {
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
      courseMain: 'Course Main Database',
      graduationSenate: 'Graduation Senate Database',
      programMain: 'Program Main Database',
      semesterMain: 'Semester Main Database',
      studentMainPoc: 'Student Main POC Database',
      studentMode: 'Student Mode Database',
      studentStatus: 'Student Status Database'
    }
    return sourceNames[key] || key
  }

  /**
   * Get status lookup table from student_status data
   */
  getStatusLookup() {
    const statusLookup = {}
    
    if (this.documents.studentStatus) {
      const lines = this.documents.studentStatus.split('\n')
      // Skip header row
      const dataLines = lines.slice(1)
      
      dataLines.forEach(line => {
        if (line.trim().length > 0) {
          // Handle both comma and pipe-separated values
          const fields = line.includes('|') ? line.split('|') : line.split(',')
          if (fields.length >= 2) {
            const statusCode = fields[0].trim()
            const statusType = fields[1].trim() // ACTIVE or INACTIVE
            const statusDesc = fields.length > 2 ? fields[2].trim() : ''
            
            // Only add if we have valid status code and type
            if (statusCode && statusType) {
              statusLookup[statusCode] = {
                type: statusType,
                description: statusDesc
              }
            }
          }
        }
      })
    }
    
    console.log('Status lookup created:', statusLookup)
    return statusLookup
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
        queryLower.includes('senarai') || queryLower.includes('cari') ||
        queryLower.includes('active') || queryLower.includes('inactive') ||
        queryLower.includes('status') || queryLower.includes('intake') ||
        queryLower.includes('program') || queryLower.includes('course') ||
        queryLower.includes('gender') || queryLower.includes('jantina') ||
        queryLower.includes('race') || queryLower.includes('bangsa') ||
        queryLower.includes('religion') || queryLower.includes('agama') ||
        queryLower.includes('negeri') || queryLower.includes('state')) {
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
      
      let relevance = 0
      let hasMatch = false

      // Per-student data
      if (chunk.dataset === 'studentMainPoc') {
        // Exact query match
        if (chunk.lowerContent.includes(queryLower)) {
          relevance += 20
          hasMatch = true
        }

        const fields = chunk.content.split('\n')
        fields.forEach(field => {
          const fieldLower = field.toLowerCase()
          if (fieldLower.includes(queryLower)) {
            relevance += 15
            hasMatch = true
          }
          
          searchTerms.forEach(term => {
            if (fieldLower.includes(term)) {
              relevance += 5
              hasMatch = true
            }
          })
        })

        const chunkNumberMatches = numberMatches.filter(num => chunk.content.includes(num))
        if (chunkNumberMatches.length > 0) {
          relevance += chunkNumberMatches.length * 5
          hasMatch = true
        }
      } else if (chunk.dataset === 'studentStatus') {
        // Status lookup table – only for status-related queries
        if (queryLower.includes('status') || queryLower.includes('active') || queryLower.includes('inactive')) {
          if (chunk.lowerContent.includes(queryLower)) {
            relevance += 15
            hasMatch = true
          }
          
          searchTerms.forEach(term => {
            if (chunk.lowerContent.includes(term)) {
              relevance += 3
              hasMatch = true
            }
          })
        }
      } else {
        // Generic relevance for other datasets
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
    relevantContent = relevantContent.slice(0, 8)

    const endTime = performance.now()
    console.log(`RAG search completed in ${(endTime - startTime).toFixed(2)}ms, processed ${processedChunks} chunks, found ${relevantContent.length} results`)

    if (relevantContent.length === 0) {
      return 'No relevant information found in the database.'
    }

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
   * Internal helper: detect requested program (DIPLOMA, DEGREE, MASTER, PHD, etc.)
   * from natural-language query ("diploma", "ijazah sarjana muda", "phd", etc.)
   */
  detectProgramFilter(queryLower) {
    const mappings = [
      { value: 'DIPLOMA',  keywords: ['diploma', 'pra diploma', 'pradip'] },
      { value: 'DEGREE',   keywords: ['degree', 'ijazah sarjana muda', 'ijazah'] },
      { value: 'MASTER',   keywords: ['master', 'sarjana'] },
      { value: 'PHD',      keywords: ['phd', 'doktor falsafah'] },
      { value: 'EDD',      keywords: ['edd', 'doktor pendidikan'] },
      { value: 'DPSP',     keywords: ['dpsp', 'postgraduate diploma'] },
      { value: 'DIPPEND',  keywords: ['dippend'] },
      { value: 'MOBILITY', keywords: ['mobiliti'] },
      { value: 'KJP',      keywords: ['kursus jangka pendek'] },
    ]

    for (const m of mappings) {
      if (m.keywords.some(kw => queryLower.includes(kw))) {
        return m.value
      }
    }
    return null
  }

  /**
   * Handle aggregate queries for UPSI data
   */
  handleAggregateQuery(query) {
    // Only use actual per-student data for counting
    const studentChunks = this.processedChunks.filter(chunk =>
      chunk.dataset === 'studentMainPoc'
    )

    const queryLower = query.toLowerCase()
    
    // Get status lookup table for mapping status codes
    const statusLookup = this.getStatusLookup()

    // Detect if query implicitly refers to a specific program (diploma/degree/master/etc.)
    const programFilter = this.detectProgramFilter(queryLower)
    
    // Helper function to count by field
    const countByField = (fieldName, displayName, valueFilter = null) => {
      const counts = {}
      studentChunks.forEach(chunk => {
        const match = chunk.content.match(new RegExp(`${fieldName}: (.+?)(?:\\n|$)`))
        if (match) {
          const value = match[1]
          counts[value] = (counts[value] || 0) + 1
        }
      })
      
      const entries = Object.entries(counts)
        .sort(([,a], [,b]) => b - a)

      // If a specific value is requested (e.g. DIPLOMA / DEGREE), return just that count
      if (valueFilter) {
        const filtered = entries.filter(([value]) => value.toLowerCase() === valueFilter.toLowerCase())
        if (filtered.length > 0) {
          return `Terdapat ${filtered[0][1]} pelajar ${valueFilter}.`
        }
        return `Tiada pelajar ${valueFilter} dalam pangkalan data.`
      }

      let response = `Jumlah pelajar mengikut ${displayName}:\n`
      entries.forEach(([value, count]) => {
        response += `${value}: ${count} pelajar\n`
      })
      return response
    }

    /********** STATUS QUERIES (ACTIVE / INACTIVE) **********/
    if (queryLower.includes('active') || queryLower.includes('inactive') || queryLower.includes('status')) {
      const statusCounts = { active: 0, inactive: 0, unknown: 0 }
      const statusCodeCounts = {}
      
      studentChunks.forEach(chunk => {
        const statusMatch = chunk.content.match(/Status: (.+?)(?:\n|$)/)
        if (statusMatch) {
          const statusCode = statusMatch[1].trim()
          statusCodeCounts[statusCode] = (statusCodeCounts[statusCode] || 0) + 1
          
          const statusInfo = statusLookup[statusCode]
          
          if (statusInfo) {
            if (statusInfo.type === 'ACTIVE') {
              statusCounts.active++
            } else if (statusInfo.type === 'INACTIVE') {
              statusCounts.inactive++
            } else {
              statusCounts.unknown++
            }
          } else {
            statusCounts.unknown++
          }
        }
      })
      
      console.log('Status code distribution:', statusCodeCounts)
      console.log('Status lookup sample:', Object.keys(statusLookup).slice(0, 10))
      console.log('Final counts:', statusCounts)
      
      if (queryLower.includes('inactive')) {
        let response = `Terdapat ${statusCounts.inactive} pelajar berstatus INACTIVE dalam pangkalan data.`
        if (statusCounts.inactive > 0) {
          response += '\n\nContoh status INACTIVE yang ditemui:'
          Object.entries(statusCodeCounts).forEach(([code, count]) => {
            const statusInfo = statusLookup[code]
            if (statusInfo && statusInfo.type === 'INACTIVE') {
              response += `\n- Status ${code} (${statusInfo.description}): ${count} pelajar`
            }
          })
        }
        return response
      } else if (queryLower.includes('active')) {
        let response = `Terdapat ${statusCounts.active} pelajar berstatus ACTIVE dalam pangkalan data.`
        if (statusCounts.active > 0) {
          response += '\n\nContoh status ACTIVE yang ditemui:'
          Object.entries(statusCodeCounts).forEach(([code, count]) => {
            const statusInfo = statusLookup[code]
            if (statusInfo && statusInfo.type === 'ACTIVE') {
              response += `\n- Status ${code} (${statusInfo.description}): ${count} pelajar`
            }
          })
        }
        return response
      } else {
        let response = `Status pelajar:\n- ACTIVE: ${statusCounts.active} pelajar\n- INACTIVE: ${statusCounts.inactive} pelajar`
        if (statusCounts.unknown > 0) {
          response += `\n- UNKNOWN: ${statusCounts.unknown} pelajar`
        }
        response += `\n\nJumlah keseluruhan: ${statusCounts.active + statusCounts.inactive + statusCounts.unknown} pelajar`
        
        response += '\n\nStatus codes yang ditemui:'
        Object.entries(statusCodeCounts)
          .sort(([,a], [,b]) => b - a)
          .slice(0, 10)
          .forEach(([code, count]) => {
            const statusInfo = statusLookup[code]
            const type = statusInfo ? statusInfo.type : 'UNKNOWN'
            const desc = statusInfo ? statusInfo.description : 'Unknown'
            response += `\n- ${code} (${type}): ${count} pelajar - ${desc}`
          })
        
        return response
      }
    }

    /********** PROGRAM QUERIES (including implicit ones like "pelajar diploma") **********/
    if (queryLower.includes('program') || queryLower.includes('kursus pengajian') || programFilter) {
      // If user said "diploma", "degree", "master", "phd", etc., programFilter will be set
      return countByField('Program', 'program', programFilter || null)
    }

    /********** OTHER DIMENSION QUERIES **********/
    if (queryLower.includes('intake') || queryLower.includes('kemasukan'))
      return countByField('Intake', 'intake')
    
    if (queryLower.includes('course') || queryLower.includes('kursus') || queryLower.includes('subjek'))
      return countByField('Course', 'kursus')
    
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
    
    // B40/M40/T20 queries (if SES Category is available in data)
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
    
    return 'Sila nyatakan apa yang ingin anda kira (contoh: pelajar aktif, mengikut program, intake, course, status, negeri, bangsa, agama, jantina, status antarabangsa, mod pengajian, kategori SES (B40/M40/T20), tahap, status perkahwinan)'
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }
