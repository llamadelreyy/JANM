import { Menu, Bell, User, LogOut, Settings } from 'lucide-react'
import { useAuthStore } from '../../stores/authStore'
import { useState, useRef, useEffect } from 'react'
import { Link } from 'react-router-dom'
import mdkLogo from '../../assets/jata-logo.png'

const Header = ({ onMenuClick }) => {
  const { user, logout } = useAuthStore()
  const [dropdownOpen, setDropdownOpen] = useState(false)
  const dropdownRef = useRef(null)

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setDropdownOpen(false)
      }
    }

    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  const getInitials = (name) => {
    if (!name) return 'U'
    const words = name.split(' ')
    if (words.length === 1) return words[0][0].toUpperCase()
    return (words[0][0] + words[words.length - 1][0]).toUpperCase()
  }

  const handleLogout = () => {
    logout()
    setDropdownOpen(false)
  }

  return (
    <header className="gov-header shadow-lg border-b border-blue-800">
      <div className="flex items-center justify-between px-6 py-4">
        {/* Left side */}
        <div className="flex items-center space-x-4">
          {/* Mobile menu button */}
          <button
            onClick={onMenuClick}
            className="lg:hidden p-2 rounded-lg text-white hover:bg-white hover:bg-opacity-20 transition-colors focus:outline-none focus:ring-2 focus:ring-white focus:ring-opacity-50"
          >
            <Menu className="h-6 w-6" />
          </button>
          
          {/* Logo/Title */}
          <div className="flex items-center space-x-3">
            <div className="h-10 w-10 bg-white rounded-lg flex items-center justify-center border border-white border-opacity-30">
              <img src={mdkLogo} alt="MDK Logo" className="h-6 w-6 object-contain" />
            </div>
            <div>
              <h1 className="text-xl font-bold text-white">
                Jabatan Akauntan Negara Malaysia GPT 
              </h1>
              <p className="text-blue-100 text-sm hidden lg:block">
                Sistem Kecerdasan Buatan JANM
              </p>
            </div>
          </div>
        </div>

        {/* Right side */}
        <div className="flex items-center space-x-4">
          {/* Notifications */}
          <button className="p-2 text-white hover:bg-white hover:bg-opacity-20 rounded-lg relative transition-colors">
            <Bell className="h-5 w-5" />
            <span className="absolute top-1 right-1 h-2 w-2 bg-red-400 rounded-full"></span>
          </button>

          {/* User dropdown */}
          <div className="relative" ref={dropdownRef}>
            <button
              onClick={() => setDropdownOpen(!dropdownOpen)}
              className="flex items-center space-x-3 p-2 rounded-lg text-white hover:bg-white hover:bg-opacity-20 transition-colors focus:outline-none focus:ring-2 focus:ring-white focus:ring-opacity-50"
            >
              <div className="flex items-center space-x-2">
                <div className="h-9 w-9 bg-white bg-opacity-20 rounded-lg flex items-center justify-center text-white text-sm font-semibold border border-white border-opacity-30">
                  {getInitials(user?.fullname)}
                </div>
                <div className="hidden md:block text-left">
                  <div className="text-sm font-semibold text-white">
                    {user?.fullname || 'Pengguna Sistem'}
                  </div>
                  <div className="text-xs text-blue-100">
                    {user?.role || 'Administrator'}
                  </div>
                </div>
              </div>
            </button>

            {/* Dropdown menu */}
            {dropdownOpen && (
              <div className="absolute right-0 mt-2 w-56 bg-white rounded-lg shadow-xl border border-slate-200 z-50 overflow-hidden">
                <div className="px-4 py-3 bg-slate-50 border-b border-slate-200">
                  <p className="text-sm font-semibold text-slate-900">{user?.fullname || 'Pengguna Sistem'}</p>
                  <p className="text-xs text-slate-500">{user?.email || 'admin@pbt.gov.my'}</p>
                </div>
                <div className="py-1">
                  <Link
                    to="/user_profile"
                    className="flex items-center px-4 py-3 text-sm text-slate-700 hover:bg-slate-50 transition-colors"
                    onClick={() => setDropdownOpen(false)}
                  >
                    <User className="h-4 w-4 mr-3 text-slate-500" />
                    Profil Pengguna
                  </Link>
                  <Link
                    to="/user_password"
                    className="flex items-center px-4 py-3 text-sm text-slate-700 hover:bg-slate-50 transition-colors"
                    onClick={() => setDropdownOpen(false)}
                  >
                    <Settings className="h-4 w-4 mr-3 text-slate-500" />
                    Tukar Katalaluan
                  </Link>
                  <hr className="my-1 border-slate-200" />
                  <button
                    onClick={handleLogout}
                    className="flex items-center w-full px-4 py-3 text-sm text-red-600 hover:bg-red-50 transition-colors"
                  >
                    <LogOut className="h-4 w-4 mr-3" />
                    Log Keluar
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  )
}

export default Header