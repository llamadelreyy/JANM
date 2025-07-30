import { useState } from 'react'
import Sidebar from './Sidebar'
import Header from './Header'

const Layout = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(false)

  return (
    <div className="flex h-screen bg-gray-100 overflow-hidden">
      {/* Main content - full width without sidebar */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <Header onMenuClick={() => setSidebarOpen(true)} />
        
        {/* Page content - with padding for proper spacing */}
        <main className="flex-1 overflow-hidden bg-gray-50 p-6">
          <div className="h-full max-w-7xl mx-auto">
            {children}
          </div>
        </main>
      </div>
    </div>
  )
}

export default Layout