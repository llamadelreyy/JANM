import { useState, useEffect } from 'react'
import api from '../services/api'

export default function Admin({ user, onLogout }) {
  const [documents, setDocuments] = useState([])
  const [status, setStatus] = useState(null)
  const [loading, setLoading] = useState(true)
  const [uploading, setUploading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [selectedFile, setSelectedFile] = useState(null)

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const [docsData, statusData] = await Promise.all([
        api.getDocuments(),
        api.getStatus()
      ])
      setDocuments(docsData.documents || [])
      setStatus(statusData.status)
    } catch (err) {
      setError('Failed to load data')
    } finally {
      setLoading(false)
    }
  }

  const handleFileSelect = (e) => {
    const file = e.target.files[0]
    if (file) {
      if (!file.name.endsWith('.txt') && !file.name.endsWith('.md')) {
        setError('Only .txt and .md files are allowed')
        return
      }
      setSelectedFile(file)
      setError('')
    }
  }

  const handleUpload = async () => {
    if (!selectedFile) {
      setError('Please select a file first')
      return
    }

    setUploading(true)
    setError('')
    setSuccess('')

    try {
      await api.uploadDocument(selectedFile)
      setSuccess('Document uploaded successfully!')
      setSelectedFile(null)
      // Reset file input
      document.getElementById('file-input').value = ''
      await loadData()
    } catch (err) {
      setError(err.message)
    } finally {
      setUploading(false)
    }
  }

  const handleDelete = async (filename) => {
    if (!confirm(`Are you sure you want to delete "${filename}"?`)) {
      return
    }

    try {
      await api.deleteDocument(filename)
      setSuccess('Document deleted successfully!')
      await loadData()
    } catch (err) {
      setError(err.message)
    }
  }

  const handleLogout = async () => {
    await api.logout()
    onLogout()
  }

  if (loading) {
    return (
      <div className="container">
        <div className="card" style={{ textAlign: 'center' }}>
          <p>Loading...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="container">
      <div className="admin-header">
        <h1>🤖 WhatsApp AI Chat Admin</h1>
        <div>
          <span style={{ marginRight: '15px' }}>Welcome, {user.username}</span>
          <button className="btn btn-secondary" onClick={handleLogout}>Logout</button>
        </div>
      </div>

      {error && <div className="error">{error}</div>}
      {success && <div className="success">{success}</div>}

      <div className="admin-content">
        {/* Upload Section */}
        <div className="card">
          <h2>📤 Upload Knowledge Base Document</h2>
          
          <div 
            className="upload-area"
            onClick={() => document.getElementById('file-input').click()}
          >
            <input
              id="file-input"
              type="file"
              accept=".txt,.md"
              onChange={handleFileSelect}
            />
            <div className="icon">📄</div>
            <p>{selectedFile ? selectedFile.name : 'Click to select a file'}</p>
            <p style={{ fontSize: '12px', color: '#666' }}>
              Only .txt and .md files are allowed (max 10MB)
            </p>
          </div>

          {selectedFile && (
            <div style={{ marginBottom: '15px' }}>
              <p><strong>Selected:</strong> {selectedFile.name}</p>
              <p><strong>Size:</strong> {(selectedFile.size / 1024).toFixed(2)} KB</p>
            </div>
          )}

          <button 
            className="btn btn-primary" 
            onClick={handleUpload}
            disabled={uploading || !selectedFile}
          >
            {uploading ? 'Uploading...' : 'Upload Document'}
          </button>
        </div>

        {/* Status Section */}
        <div className="card">
          <h2>📊 System Status</h2>
          
          {status && (
            <>
              <div className="status-card">
                <h3>Database Status</h3>
                <div className="status-item">
                  <span>Database Loaded:</span>
                  <span className={status.isLoaded ? 'status-yes' : 'status-no'}>
                    {status.isLoaded ? 'Yes' : 'No'}
                  </span>
                </div>
                <div className="status-item">
                  <span>Static Documents:</span>
                  <span>{status.hasJpanFAQ ? '✅' : '❌'} JPAN FAQ</span>
                </div>
                <div className="status-item">
                  <span>Uploaded Documents:</span>
                  <span>{status.uploadedDocumentsCount || 0}</span>
                </div>
              </div>

              {status.uploadedDocuments && status.uploadedDocuments.length > 0 && (
                <div className="status-card">
                  <h3>Active Knowledge Base Files</h3>
                  <ul style={{ listStyle: 'none', padding: 0 }}>
                    {status.uploadedDocuments.map((doc, index) => (
                      <li key={index} style={{ padding: '5px 0', borderBottom: '1px solid #eee' }}>
                        📄 {doc}
                      </li>
                    ))}
                  </ul>
                </div>
              )}
            </>
          )}
        </div>
      </div>

      {/* Document List */}
      <div className="card">
        <h2>📚 Uploaded Documents</h2>
        
        {documents.length === 0 ? (
          <p style={{ color: '#666', textAlign: 'center', padding: '20px' }}>
            No documents uploaded yet. Upload your first document above.
          </p>
        ) : (
          <ul className="document-list">
            {documents.map((doc) => (
              <li key={doc.filename} className="document-item">
                <div className="info">
                  <div className="name">📄 {doc.filename}</div>
                  <div className="meta">
                    Size: {(doc.size / 1024).toFixed(2)} KB | 
                    Uploaded: {new Date(doc.uploadedAt).toLocaleString()}
                  </div>
                </div>
                <button 
                  className="btn btn-danger"
                  onClick={() => handleDelete(doc.filename)}
                >
                  Delete
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  )
}