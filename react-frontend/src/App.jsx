import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './stores/authStore'
import Layout from './components/Layout/Layout'
import ProtectedRoute from './components/Auth/ProtectedRoute'
import PublicChatbot from './components/UI/PublicChatbot'

// Import all pages
import {
  Login,
  Landing,
  AIChat
} from './pages/index.jsx'

function App() {
  const { isAuthenticated } = useAuthStore()

  return (
    <div className="min-h-screen bg-gray-50">
      <Routes>
        {/* Public routes */}
        <Route
          path="/"
          element={
            isAuthenticated ? <Navigate to="/ai-chat" replace /> : <Landing />
          }
        />
        <Route
          path="/login"
          element={
            isAuthenticated ? <Navigate to="/ai-chat" replace /> : <Login />
          }
        />
        
        {/* Protected routes */}
        <Route 
          path="/*" 
          element={
            <ProtectedRoute>
              <Layout>
                <Routes>
                  <Route path="/ai-chat" element={<AIChat />} />
                  
                  {/* Catch all route */}
                  <Route path="*" element={<Navigate to="/ai-chat" replace />} />
                </Routes>
              </Layout>
            </ProtectedRoute>
          } 
        />
      </Routes>
      
      {/* Sticky Public Chatbot - Available on all pages */}
      <PublicChatbot />
    </div>
  )
}

export default App