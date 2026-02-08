const API_BASE = '/api'

class ApiService {
  getToken() {
    return localStorage.getItem('token')
  }

  setToken(token) {
    localStorage.setItem('token', token)
  }

  removeToken() {
    localStorage.removeItem('token')
  }

  getHeaders() {
    const headers = {
      'Content-Type': 'application/json'
    }
    const token = this.getToken()
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }
    return headers
  }

  // Auth endpoints
  async register(username, email, password) {
    const response = await fetch(`${API_BASE}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, email, password })
    })
    const data = await response.json()
    if (!response.ok) {
      throw new Error(data.error || 'Registration failed')
    }
    return data
  }

  async login(email, password) {
    const response = await fetch(`${API_BASE}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    })
    const data = await response.json()
    if (!response.ok) {
      throw new Error(data.error || 'Login failed')
    }
    return data
  }

  async logout() {
    const response = await fetch(`${API_BASE}/auth/logout`, {
      method: 'POST',
      headers: this.getHeaders()
    })
    this.removeToken()
    return response.json()
  }

  async verify() {
    const response = await fetch(`${API_BASE}/auth/verify`, {
      headers: this.getHeaders()
    })
    if (!response.ok) {
      this.removeToken()
      return null
    }
    return response.json()
  }

  // Document endpoints
  async getDocuments() {
    const response = await fetch(`${API_BASE}/documents`, {
      headers: this.getHeaders()
    })
    const data = await response.json()
    if (!response.ok) {
      throw new Error(data.error || 'Failed to get documents')
    }
    return data
  }

  async uploadDocument(file) {
    const formData = new FormData()
    formData.append('document', file)

    const response = await fetch(`${API_BASE}/documents/upload`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${this.getToken()}`
      },
      body: formData
    })
    const data = await response.json()
    if (!response.ok) {
      throw new Error(data.error || 'Upload failed')
    }
    return data
  }

  async deleteDocument(filename) {
    const response = await fetch(`${API_BASE}/documents/${encodeURIComponent(filename)}`, {
      method: 'DELETE',
      headers: this.getHeaders()
    })
    const data = await response.json()
    if (!response.ok) {
      throw new Error(data.error || 'Delete failed')
    }
    return data
  }

  async getStatus() {
    const response = await fetch(`${API_BASE}/documents/status`, {
      headers: this.getHeaders()
    })
    const data = await response.json()
    if (!response.ok) {
      throw new Error(data.error || 'Failed to get status')
    }
    return data
  }
}

export default new ApiService()