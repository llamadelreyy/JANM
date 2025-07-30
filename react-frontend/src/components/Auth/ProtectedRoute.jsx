import { Navigate } from 'react-router-dom'
import { useAuthStore } from '../../stores/authStore'
import { useEffect } from 'react'

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, initializeAuth } = useAuthStore()

  useEffect(() => {
    // Initialize auth for development
    if (process.env.NODE_ENV === 'development' && !isAuthenticated) {
      initializeAuth()
    }
  }, [isAuthenticated, initializeAuth])

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return children
}

export default ProtectedRoute