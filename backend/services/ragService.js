/**
 * RAG Service for Database Search - Node.js Version
 * Handles searching through organizational database files
 * Supports both static files and uploaded documents
 */

import fs from 'fs/promises'
import path from 'path'

class RAGService {
  constructor() {
    this.documents = {
      uploadedDocuments: {}
    }
    this.isLoaded = false
    this.uploadedDocumentsDir = path.join(process.cwd(), './uploads')
  }

  /**
   * Load database from uploaded documents only
   */
  async loadDocuments() {
    try {
      // Load uploaded documents
      await this.loadUploadedDocuments()

      this.isLoaded = true
      console.log('RAG database loaded successfully')
      console.log(`Loaded ${Object.keys(this.documents.uploadedDocuments).length} uploaded documents`)
    } catch (error) {
      console.error('Failed to load RAG database:', error)
      this.isLoaded = false
    }
  }

  /**
   * Load uploaded documents from uploads directory
   */
  async loadUploadedDocuments() {
    try {
      await fs.access(this.uploadedDocumentsDir)
      const files = await fs.readdir(this.uploadedDocumentsDir)
      
      for (const file of files) {
        if (file.endsWith('.txt') || file.endsWith('.md')) {
          const filePath = path.join(this.uploadedDocumentsDir, file)
          try {
            const content = await fs.readFile(filePath, 'utf8')
            this.documents.uploadedDocuments[file] = content
            console.log(`Loaded uploaded document: ${file}`)
          } catch (error) {
            console.warn(`Failed to load uploaded document ${file}:`, error.message)
          }
        }
      }
    } catch (error) {
      // Uploads directory doesn't exist yet, that's okay
      console.log('No uploaded documents found')
    }
  }

  /**
   * Upload a new document
   * @param {string} filename - Original filename
   * @param {Buffer} content - File content as buffer
   * @returns {object} - Upload result
   */
  async uploadDocument(filename, content) {
    try {
      // Ensure uploads directory exists
      await fs.mkdir(this.uploadedDocumentsDir, { recursive: true })

      // Sanitize filename
      const sanitizedFilename = filename.replace(/[^a-zA-Z0-9._-]/g, '_')
      const filePath = path.join(this.uploadedDocumentsDir, sanitizedFilename)

      // Write file
      await fs.writeFile(filePath, content)

      // Load into memory
      const textContent = content.toString('utf8')
      this.documents.uploadedDocuments[sanitizedFilename] = textContent

      console.log(`Uploaded document: ${sanitizedFilename}`)

      return {
        success: true,
        filename: sanitizedFilename,
        size: content.length
      }
    } catch (error) {
      console.error('Failed to upload document:', error)
      throw error
    }
  }

  /**
   * Delete an uploaded document
   * @param {string} filename - Filename to delete
   * @returns {object} - Delete result
   */
  async deleteDocument(filename) {
    try {
      const filePath = path.join(this.uploadedDocumentsDir, filename)
      
      // Delete from filesystem
      try {
        await fs.unlink(filePath)
      } catch (error) {
        throw new Error('File not found')
      }

      // Remove from memory
      delete this.documents.uploadedDocuments[filename]

      console.log(`Deleted document: ${filename}`)

      return {
        success: true,
        filename
      }
    } catch (error) {
      console.error('Failed to delete document:', error)
      throw error
    }
  }

  /**
   * Get list of uploaded documents
   * @returns {array} - List of document info
   */
  async getUploadedDocuments() {
    try {
      await fs.access(this.uploadedDocumentsDir)
      const files = await fs.readdir(this.uploadedDocumentsDir)
      
      const documents = []
      for (const file of files) {
        if (file.endsWith('.txt') || file.endsWith('.md')) {
          const filePath = path.join(this.uploadedDocumentsDir, file)
          const stats = await fs.stat(filePath)
          documents.push({
            filename: file,
            size: stats.size,
            uploadedAt: stats.mtime.toISOString()
          })
        }
      }
      
      return documents
    } catch (error) {
      return []
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

    // Check if there are any documents
    const uploadedDocCount = Object.keys(this.documents.uploadedDocuments).length
    if (uploadedDocCount === 0) {
      return 'No knowledge base documents uploaded yet. Please upload documents through the admin panel.'
    }

    // Enhanced search terms - include original query, split terms, and numbers
    const originalQuery = query.toLowerCase()
    const searchTerms = query.toLowerCase().split(' ').filter(term => term.length > 2)
    
    // Extract numbers from query for better number matching
    const numberMatches = query.match(/\d+/g) || []
    
    let relevantContent = []

    // Search in uploaded documents only
    for (const [filename, content] of Object.entries(this.documents.uploadedDocuments)) {
      const results = this.searchInDocument(
        content,
        query,
        originalQuery,
        searchTerms,
        numberMatches,
        `Knowledge Base: ${filename}`
      )
      relevantContent = relevantContent.concat(results)
    }

    // Sort by relevance and limit results
    relevantContent.sort((a, b) => b.relevance - a.relevance)
    relevantContent = relevantContent.slice(0, 50)

    if (relevantContent.length === 0) {
      return 'No relevant information found in the uploaded documents.'
    }

    // Format the results
    let formattedResults = 'Relevant information found:\n\n'
    relevantContent.forEach((item, index) => {
      formattedResults += `${index + 1}. From ${item.source}:\n${item.content}\n\n`
    })

    return formattedResults
  }

  searchInDocument(documentContent, query, originalQuery, searchTerms, numberMatches, sourceName) {
    const results = []
    const lines = documentContent.split('\n')
    
    lines.forEach((line, index) => {
      const lowerLine = line.toLowerCase()
      const originalLine = line
      
      // Check for exact query match, search terms, or number matches
      const hasExactMatch = lowerLine.includes(originalQuery)
      const hasTermMatch = searchTerms.some(term => lowerLine.includes(term))
      const hasNumberMatch = numberMatches.some(num => originalLine.includes(num))
      
      if (hasExactMatch || hasTermMatch || hasNumberMatch) {
        // Include context
        let start = Math.max(0, index - 10)
        let end = Math.min(lines.length, index + 15)
        
        // Look for section boundaries
        for (let i = index; i >= start; i--) {
          const currentLine = lines[i]
          if (currentLine.match(/^#{1,6}\s+/) ||
              currentLine.match(/^\d+\.\s+/) ||
              currentLine.match(/^[A-Z][^a-z]*:/) ||
              currentLine.match(/^\*\*[^*]+\*\*/) ||
              currentLine.match(/^#{1,6}/) ||
              currentLine.match(/^[A-Za-z\s]+:$/) ||
              currentLine.trim() === '') {
            if (i < index - 2) {
              start = i
              break
            }
          }
        }
        
        for (let i = index; i <= end; i++) {
          const currentLine = lines[i] || ''
          const nextLine = lines[i + 1] || ''
          
          if (i > index + 5 && (
              nextLine.match(/^#{1,6}\s+/) ||
              nextLine.match(/^\d+\.\s+/) ||
              nextLine.match(/^[A-Z][^a-z]*:/) ||
              nextLine.match(/^\*\*[^*]+\*\*/) ||
              (currentLine.trim() === '' && nextLine.trim() === '') ||
              nextLine.match(/^[A-Za-z\s]+:$/)
            )) {
            end = i + 1
            break
          }
        }
        
        const context = lines.slice(start, end).join('\n')
        
        let relevance = 0
        if (hasExactMatch) relevance += 10
        if (hasNumberMatch) relevance += 5
        relevance += searchTerms.filter(term => lowerLine.includes(term)).length
        
        results.push({
          source: sourceName,
          content: context,
          relevance: relevance
        })
      }
    })
    
    return results
  }

  /**
   * Get document status
   */
  getStatus() {
    return {
      isLoaded: this.isLoaded,
      uploadedDocumentsCount: Object.keys(this.documents.uploadedDocuments).length,
      uploadedDocuments: Object.keys(this.documents.uploadedDocuments)
    }
  }
}

// Create and export a singleton instance
const ragService = new RAGService()
export default ragService

// Also export the class for testing or custom instances
export { RAGService }