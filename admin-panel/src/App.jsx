import { useState, useEffect } from 'react'
import Login from './pages/Login'
import Register from './pages/Register'
import Admin from './pages/Admin'
import api from './services/api'

export default function App() {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Check if user is already logged in
    const checkAuth = async () => {
      const token = api.getToken()
      if (token) {
        const data = await api.verify()
        if (data && data.user) {
          setUser(data.user)
        }
      }
      setLoading(false)
    }
    checkAuth()
  }, [])

  const handleLogin = (userData) => {
    setUser(userData)
  }

  const handleLogout = () => {
    setUser(null)
  }

  const handleRegister = () => {
    window.location.href = '/login'
  }

  if (loading) {
    return (
      <div className="auth-container">
        <div className="card auth-card" style={{ textAlign: 'center' }}>
          <p>Loading...</p>
        </div>
      </div>
    )
  }

  // Simple routing based on URL path
  const path = window.location.pathname

  if (path === '/register') {
    return <Register onRegister={handleRegister} />
  }

  if (user) {
    return <Admin user={user} onLogout={handleLogout} />
  }

  if (path === '/login' || path === '/') {
    return <Login onLogin={handleLogin} />
  }

  // Default to login
  return <Login onLogin={handleLogin} />
}