import axios from 'axios'
import { useAuthStore } from '../stores/authStore'

// Create axios instance
const api = axios.create({
  baseURL: '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = useAuthStore.getState().accessToken
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      useAuthStore.getState().logout()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Dashboard API
export const dashboardApi = {
  getDashboardData: () => api.get('/Dashboard/GetDashboardData'),
  getFinancialDashboard: () => api.get('/Dashboard/GetFinancialDashboard'),
}

// Menu API
export const menuApi = {
  getMenuByAuth: () => api.get('/Menu/GetListByAuth'),
  getMenuByRole: (roleId) => api.get(`/Menu/GetListByRole?roleId=${roleId}`),
}

// User API
export const userApi = {
  getCurrentUser: () => api.get('/User/GetCurrentUser'),
  updateProfile: (data) => api.put('/User/UpdateProfile', data),
  changePassword: (data) => api.put('/User/ChangePassword', data),
  getUsers: () => api.get('/User/GetList'),
  addUser: (data) => api.post('/User/Add', data),
  updateUser: (id, data) => api.put(`/User/Update/${id}`, data),
  deleteUser: (id) => api.delete(`/User/Remove/${id}`),
}

// Role API
export const roleApi = {
  getRoles: () => api.get('/Role/GetList'),
  addRole: (data) => api.post('/Role/Add', data),
  updateRole: (id, data) => api.put(`/Role/Update/${id}`, data),
  deleteRole: (id) => api.delete(`/Role/Remove/${id}`),
}

// Department API
export const departmentApi = {
  getDepartments: () => api.get('/Department/GetList'),
  addDepartment: (data) => api.post('/Department/Add', data),
  updateDepartment: (id, data) => api.put(`/Department/Update/${id}`, data),
  deleteDepartment: (id) => api.delete(`/Department/Remove/${id}`),
}

// Unit API
export const unitApi = {
  getUnits: () => api.get('/Unit/GetList'),
  addUnit: (data) => api.post('/Unit/Add', data),
  updateUnit: (id, data) => api.put(`/Unit/Update/${id}`, data),
  deleteUnit: (id) => api.delete(`/Unit/Remove/${id}`),
}

// Location APIs
export const locationApi = {
  getCountries: () => api.get('/Country/GetList'),
  getStates: () => api.get('/State/GetList'),
  getDistricts: (stateId) => api.get(`/District/GetList?stateId=${stateId}`),
  getTowns: (districtId) => api.get(`/Town/GetList?districtId=${districtId}`),
}

// Search API
export const searchApi = {
  getPremisDetails: () => api.get('/Search/GetListPremisDetails'),
  searchPremis: (query) => api.get(`/Search/SearchPremis?query=${query}`),
}

// Report APIs
export const reportApi = {
  getKompaunReport: (params) => api.get('/Report/GetKompaunReport', { params }),
  getNotisReport: (params) => api.get('/Report/GetNotisReport', { params }),
  getInspectionReport: (params) => api.get('/Report/GetInspectionReport', { params }),
  getConfiscationReport: (params) => api.get('/Report/GetConfiscationReport', { params }),
}

// Auth API
export const authApi = {
  login: (credentials) => api.post('/Auth/Login', credentials),
  logout: () => api.post('/Auth/Logout'),
  refreshToken: () => api.post('/Auth/RefreshToken'),
  forgotPassword: (email) => api.post('/Auth/ForgotPassword', { email }),
  resetPassword: (data) => api.post('/Auth/ResetPassword', data),
}

// AI Services API (FastAPI backend on port 8002)
export const aiApi = {
  // Whisper Speech-to-Text API
  transcribeAudio: async (audioBlob) => {
    const formData = new FormData()
    formData.append('audio', audioBlob, 'recording.wav')
    
    const response = await fetch('/api/whisper', {
      method: 'POST',
      body: formData,
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  },

  // LLM Chat API
  chatWithLLM: async (message, conversationHistory = []) => {
    const response = await fetch('/api/llm', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        message,
        user_name: 'User',
      }),
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  },

  // TTS (Text-to-Speech) API
  synthesizeSpeech: async (text, voice = 'female', speed = 1.0) => {
    const response = await fetch('/api/tts', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        text,
        voice,
        speed,
        user_name: 'User'
      }),
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    const data = await response.json()
    
    // Convert base64 to blob
    const audioBase64 = data.audio_base64
    const binaryString = atob(audioBase64)
    const bytes = new Uint8Array(binaryString.length)
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i)
    }
    
    const audioBlob = new Blob([bytes], { type: 'audio/wav' })
    return audioBlob
  },

  // Health check
  healthCheck: async () => {
    const response = await fetch('/health')
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    return await response.json()
  },
}

export default api