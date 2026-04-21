import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuthStore } from '../stores/authStore'
import { Eye, EyeOff, User, Lock, Shield, CreditCard, Receipt } from 'lucide-react'
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
  const [animatedElements, setAnimatedElements] = useState([])

  useEffect(() => {
    setTimeout(() => setAnimatedElements(['logo', 'title', 'form', 'footer']), 100)
  }, [])

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
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-950 to-slate-900 relative overflow-hidden font-sans">
      {/* Google Inter Font */}
      <style>
        {`@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap');`}
      </style>

      {/* Animated Background */}
      <div className="absolute inset-0 overflow-hidden">
        {/* Floating Icons */}
        <div className="absolute top-20 left-10 opacity-5 animate-float">
          <CreditCard className="w-32 h-32 text-white" />
        </div>
        <div className="absolute top-40 right-20 opacity-5 animate-float-delayed">
          <Receipt className="w-24 h-24 text-white" />
        </div>
        <div className="absolute bottom-40 left-1/4 opacity-5 animate-float">
          <Shield className="w-20 h-20 text-white" />
        </div>
        
        {/* Grid Pattern */}
        <div className="absolute inset-0 bg-grid-pattern opacity-5" />
        
        {/* Glowing Orbs */}
        <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-blue-600/20 rounded-full blur-3xl animate-pulse-slow" />
        <div className="absolute bottom-1/4 right-1/4 w-80 h-80 bg-blue-500/10 rounded-full blur-3xl animate-pulse-slow-delayed" />
      </div>

      {/* Main Content */}
      <div className="relative min-h-screen flex flex-col items-center justify-center px-4 py-8">
        {/* Logo Section */}
        <div className={`transform transition-all duration-700 ${animatedElements.includes('logo') ? 'translate-y-0 opacity-100' : 'translate-y-8 opacity-0'}`}>
          <div className="mb-6">
            <img
              src={mdkLogo}
              alt="PDRM Logo"
              className="h-20 w-20 object-contain drop-shadow-lg mx-auto"
            />
          </div>
        </div>

        {/* Title Section */}
        <div className={`text-center mb-6 transform transition-all duration-700 delay-200 ${animatedElements.includes('title') ? 'translate-y-0 opacity-100' : 'translate-y-8 opacity-0'}`}>
          <h1 className="text-4xl md:text-5xl font-extrabold text-white mb-3 tracking-tight">
            <span className="bg-gradient-to-r from-white via-blue-100 to-white bg-clip-text text-transparent">
              MyBayar PDRM
            </span>
          </h1>
          <div className="max-w-md mx-auto">
            <p className="text-blue-200 text-sm font-medium leading-relaxed">
              Check and pay summon online without service charges.
            </p>
            <p className="text-blue-300 text-sm font-medium mt-2">
              You can now pay individual summon and company summon via MyBayar PDRM - Quick and easy!
            </p>
          </div>
        </div>

        {/* Login Form */}
        <div className={`w-full max-w-md transform transition-all duration-700 delay-300 ${animatedElements.includes('form') ? 'translate-y-0 opacity-100' : 'translate-y-8 opacity-0'}`}>
          <div className="bg-white/10 backdrop-blur-xl rounded-2xl shadow-2xl border border-white/20 overflow-hidden">
            {/* Form Header */}
            <div className="bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4">
              <h2 className="text-xl font-bold text-white flex items-center justify-center space-x-2">
                <Lock className="w-5 h-5" />
                <span>Log Masuk Sistem</span>
              </h2>
            </div>

            {/* Form Body */}
            <div className="p-6 space-y-5">
              <p className="text-center text-blue-200 text-sm">
                Sila masukkan maklumat pengguna anda
              </p>

              <form className="space-y-4" onSubmit={handleSubmit}>
                {/* Username Field */}
                <div className="space-y-2">
                  <label htmlFor="username" className="block text-sm font-semibold text-blue-100">
                    Nama Pengguna
                  </label>
                  <div className="relative group">
                    <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                      <User className="h-5 w-5 text-blue-400 group-focus-within:text-blue-300 transition-colors" />
                    </div>
                    <input
                      id="username"
                      name="username"
                      type="text"
                      required
                      value={formData.username}
                      onChange={handleChange}
                      className="w-full pl-12 pr-4 py-3 bg-slate-800/50 border border-slate-600 rounded-xl text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                      placeholder="Masukkan nama pengguna"
                    />
                  </div>
                </div>

                {/* Password Field */}
                <div className="space-y-2">
                  <label htmlFor="password" className="block text-sm font-semibold text-blue-100">
                    Katalaluan
                  </label>
                  <div className="relative group">
                    <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                      <Lock className="h-5 w-5 text-blue-400 group-focus-within:text-blue-300 transition-colors" />
                    </div>
                    <input
                      id="password"
                      name="password"
                      type={showPassword ? 'text' : 'password'}
                      required
                      value={formData.password}
                      onChange={handleChange}
                      className="w-full pl-12 pr-12 py-3 bg-slate-800/50 border border-slate-600 rounded-xl text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                      placeholder="Masukkan katalaluan"
                    />
                    <button
                      type="button"
                      onClick={() => setShowPassword(!showPassword)}
                      className="absolute inset-y-0 right-0 pr-4 flex items-center text-slate-400 hover:text-white transition-colors"
                    >
                      {showPassword ? (
                        <EyeOff className="h-5 w-5" />
                      ) : (
                        <Eye className="h-5 w-5" />
                      )}
                    </button>
                  </div>
                </div>

                {/* Submit Button */}
                <button
                  type="submit"
                  disabled={isLoading}
                  className="w-full py-3.5 px-4 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white font-bold rounded-xl shadow-lg hover:shadow-xl transform hover:-translate-y-0.5 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none flex items-center justify-center space-x-2"
                >
                  {isLoading ? (
                    <>
                      <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                      <span>Sedang log masuk...</span>
                    </>
                  ) : (
                    <>
                      <Shield className="w-5 h-5" />
                      <span>Log Masuk Sistem</span>
                    </>
                  )}
                </button>
              </form>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className={`mt-8 text-center transform transition-all duration-700 delay-500 ${animatedElements.includes('footer') ? 'translate-y-0 opacity-100' : 'translate-y-8 opacity-0'}`}>
          <p className="text-slate-500 text-xs">
            © 2026 Polis Diraja Malaysia. Hak Cipta Terpelihara.
          </p>
        </div>
      </div>

      {/* CSS Animations */}
      <style>{`
        @keyframes float {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          50% { transform: translateY(-20px) rotate(5deg); }
        }
        @keyframes float-delayed {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          50% { transform: translateY(-15px) rotate(-5deg); }
        }
        @keyframes pulse-slow {
          0%, 100% { opacity: 0.2; transform: scale(1); }
          50% { opacity: 0.3; transform: scale(1.1); }
        }
        .animate-float { animation: float 6s ease-in-out infinite; }
        .animate-float-delayed { animation: float-delayed 8s ease-in-out infinite; }
        .animate-pulse-slow { animation: pulse-slow 4s ease-in-out infinite; }
        .animate-pulse-slow-delayed { animation: pulse-slow 5s ease-in-out infinite 1s; }
        .bg-grid-pattern {
          background-image: 
            linear-gradient(rgba(255,255,255,0.03) 1px, transparent 1px),
            linear-gradient(90deg, rgba(255,255,255,0.03) 1px, transparent 1px);
          background-size: 50px 50px;
        }
        .font-sans {
          font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        }
      `}</style>
    </div>
  )
}

export default Login