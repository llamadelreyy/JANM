import { useState } from 'react'
import { useLocation } from 'react-router-dom'
import Sidebar from './Sidebar'
import Header from './Header'

const Layout = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const location = useLocation()
  
  // Check if current page is AI Chat to provide more space
  const isAIChat = location.pathname === '/ai-chat'

  return (
    <div className="flex h-screen bg-gray-100 overflow-hidden">
      {/* Main content - full width without sidebar */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <Header onMenuClick={() => setSidebarOpen(true)} />
        
        {/* Page content - conditional styling for AI Chat */}
        <main className={`flex-1 overflow-hidden bg-gray-50 ${isAIChat ? 'p-2' : 'p-6'}`}>
          <div className={`h-full ${isAIChat ? 'max-w-none' : 'max-w-7xl'} mx-auto`}>
            {children}
          </div>
        </main>
      </div>
    </div>
  )
}

export default Layout