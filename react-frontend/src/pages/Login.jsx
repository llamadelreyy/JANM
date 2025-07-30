import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuthStore } from '../stores/authStore'
import { Eye, EyeOff, User, Lock } from 'lucide-react'
import { toast } from 'sonner'
import mdkLogo from '../assets/jata-logo.png'

const Login = () => {
  const [formData, setFormData] = useState({
    username: '',
    password: ''
  })
  const [showPassword, setShowPassword] = useState(false)
  const { login, isLoading } = useAuthStore()
  const navigate = useNavigate()

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (!formData.username || !formData.password) {
      toast.error('Sila masukkan nama pengguna dan katalaluan')
      return
    }

    const result = await login(formData)
    
    if (result.success) {
      toast.success('Berjaya log masuk')
      navigate('/ai-chat')
    } else {
      toast.error(result.error || 'Ralat log masuk')
    }
  }

  const handleChange = (e) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }))
  }

  return (
    <div className="h-screen bg-gradient-to-br from-slate-50 to-blue-50 flex items-center justify-center p-4 overflow-hidden">
      <div className="w-full max-w-md">
        {/* Header */}
        <div className="text-center mb-6">
          <div className="flex justify-center mb-4">
            <div className="h-16 w-16 bg-white rounded-xl flex items-center justify-center shadow-lg border border-slate-200">
              <img src={mdkLogo} alt="MDK Logo" className="h-12 w-12 object-contain" />
            </div>
          </div>
          <h2 className="text-2xl font-bold text-slate-900">Sistem Kecerdasan Buatan JANM</h2>
          <p className="text-sm text-slate-600 font-medium mt-1">
            Sistem Kecerdasan Buatan JANM
          </p>
          <p className="text-xs text-slate-500">Kerajaan Malaysia</p>
        </div>

        {/* Login Form */}
        <div className="bg-white py-6 px-6 shadow-xl border border-slate-200 rounded-xl">
          <div className="mb-4 text-center">
            <h3 className="text-lg font-semibold text-slate-900">Log Masuk Sistem</h3>
            <p className="text-sm text-slate-600 mt-1">Sila masukkan maklumat pengguna anda</p>
          </div>

          <form className="space-y-4" onSubmit={handleSubmit}>
            <div>
              <label htmlFor="username" className="block text-sm font-semibold text-slate-700 mb-1">
                Nama Pengguna
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <User className="h-4 w-4 text-slate-400" />
                </div>
                <input
                  id="username"
                  name="username"
                  type="text"
                  required
                  value={formData.username}
                  onChange={handleChange}
                  className="gov-input block w-full pl-9 pr-3 py-2.5 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm transition-colors"
                  placeholder="Masukkan nama pengguna"
                />
              </div>
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-semibold text-slate-700 mb-1">
                Katalaluan
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Lock className="h-4 w-4 text-slate-400" />
                </div>
                <input
                  id="password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  required
                  value={formData.password}
                  onChange={handleChange}
                  className="gov-input block w-full pl-9 pr-9 py-2.5 border border-slate-300 rounded-lg placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm transition-colors"
                  placeholder="Masukkan katalaluan"
                />
                <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="text-slate-400 hover:text-slate-600 transition-colors"
                  >
                    {showPassword ? (
                      <EyeOff className="h-4 w-4" />
                    ) : (
                      <Eye className="h-4 w-4" />
                    )}
                  </button>
                </div>
              </div>
            </div>

            <div className="pt-2">
              <button
                type="submit"
                disabled={isLoading}
                className="gov-button-primary w-full flex justify-center py-2.5 px-4 border border-transparent rounded-lg text-sm font-semibold text-white bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 shadow-lg"
              >
                {isLoading ? (
                  <div className="flex items-center">
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                    Sedang log masuk...
                  </div>
                ) : (
                  'Log Masuk Sistem'
                )}
              </button>
            </div>
          </form>

          {/* Development notice */}
          {process.env.NODE_ENV === 'development' && (
            <div className="mt-4">
              <div className="relative">
                <div className="absolute inset-0 flex items-center">
                  <div className="w-full border-t border-slate-200" />
                </div>
                <div className="relative flex justify-center text-sm">
                  <span className="px-2 bg-white text-slate-500 font-medium">Demo</span>
                </div>
              </div>

              <div className="mt-3 p-3 bg-slate-50 rounded-lg border border-slate-200">
                <div className="text-center text-xs text-slate-600">
                  <p className="font-semibold mb-1">Akaun Demo:</p>
                  <p><span className="font-medium">Username:</span> admin</p>
                  <p><span className="font-medium">Password:</span> admin</p>
                </div>
              </div>
            </div>
          )}

          <div className="mt-4 text-center">
            <p className="text-xs text-slate-500">
              © 2024 Kerajaan Malaysia. Hak Cipta Terpelihara.
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Login