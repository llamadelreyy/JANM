import { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { 
  GraduationCap, 
  Microscope, 
  Cpu, 
  Lightbulb, 
  Users, 
  Globe, 
  ArrowRight, 
  Play,
  ChevronDown,
  Sparkles,
  Building2,
  FileText,
  MessageSquare,
  Bot
} from 'lucide-react'

const Landing = () => {
  const navigate = useNavigate()
  const [animatedElements, setAnimatedElements] = useState([])
  const [scrollY, setScrollY] = useState(0)
  const [activeFeature, setActiveFeature] = useState(0)
  const canvasRef = useRef(null)

  useEffect(() => {
    // Staggered animation on mount
    const timer = setTimeout(() => {
      setAnimatedElements(['hero', 'stats', 'features', 'about', 'cta'])
    }, 100)
    return () => clearTimeout(timer)
  }, [])

  useEffect(() => {
    const handleScroll = () => setScrollY(window.scrollY)
    window.addEventListener('scroll', handleScroll)
    return () => window.removeEventListener('scroll', handleScroll)
  }, [])

  // Particle animation canvas
  useEffect(() => {
    const canvas = canvasRef.current
    if (!canvas) return
    const ctx = canvas.getContext('2d')
    canvas.width = window.innerWidth
    canvas.height = window.innerHeight

    const particles = []
    const particleCount = 80

    for (let i = 0; i < particleCount; i++) {
      particles.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        vx: (Math.random() - 0.5) * 0.5,
        vy: (Math.random() - 0.5) * 0.5,
        size: Math.random() * 2 + 1,
        opacity: Math.random() * 0.5 + 0.2
      })
    }

    let animationId
    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height)
      
      particles.forEach((p, i) => {
        p.x += p.vx
        p.y += p.vy

        if (p.x < 0 || p.x > canvas.width) p.vx *= -1
        if (p.y < 0 || p.y > canvas.height) p.vy *= -1

        ctx.beginPath()
        ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2)
        ctx.fillStyle = `rgba(59, 130, 246, ${p.opacity})`
        ctx.fill()

        // Connect nearby particles
        particles.forEach((p2, j) => {
          if (i === j) return
          const dx = p.x - p2.x
          const dy = p.y - p2.y
          const dist = Math.sqrt(dx * dx + dy * dy)
          if (dist < 150) {
            ctx.beginPath()
            ctx.moveTo(p.x, p.y)
            ctx.lineTo(p2.x, p2.y)
            ctx.strokeStyle = `rgba(59, 130, 246, ${0.1 * (1 - dist / 150)})`
            ctx.stroke()
          }
        })
      })

      animationId = requestAnimationFrame(animate)
    }
    animate()

    const handleResize = () => {
      canvas.width = window.innerWidth
      canvas.height = window.innerHeight
    }
    window.addEventListener('resize', handleResize)

    return () => {
      cancelAnimationFrame(animationId)
      window.removeEventListener('resize', handleResize)
    }
  }, [])

  const features = [
    {
      icon: Bot,
      title: 'AI Chatbot Cerdik',
      description: 'Bertanya apa-apa tentang pendidikan, sains, dan teknologi dengan chatbot AI kami yang bijak.',
      color: 'from-blue-500 to-cyan-500'
    },
    {
      icon: FileText,
      title: 'Dasar & Dokument',
      description: 'Akses mudah kepada dasar-dasar pendidikan negeri Sabah dan dokumen rasmi kerajaan.',
      color: 'from-violet-500 to-purple-500'
    },
    {
      icon: Lightbulb,
      title: 'Inovasi & Penyelidikan',
      description: 'Terokai program inovasi dan peluang penyelidikan untuk pembangunan modal insan.',
      color: 'from-amber-500 to-orange-500'
    },
    {
      icon: Users,
      title: 'Komuniti Pembelajaran',
      description: 'Sertai komuniti untuk berkongsi ilmu dan terbaik practices dalam pendidikan.',
      color: 'from-emerald-500 to-teal-500'
    }
  ]

  const stats = [
    { value: '500+', label: 'Sekolah Seluruh Sabah' },
    { value: '10,000+', label: 'Pendidik Terdaftar' },
    { value: '150+', label: 'Program STEM' },
    { value: '50+', label: 'Kerjasama Global' }
  ]

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-950 via-blue-950 to-slate-950 relative overflow-hidden font-sans">
      {/* Particle Canvas Background */}
      <canvas 
        ref={canvasRef} 
        className="absolute inset-0 w-full h-full pointer-events-none"
        style={{ opacity: scrollY > 100 ? Math.max(0, 1 - scrollY / 500) : 1 }}
      />

      {/* Animated Background Elements */}
      <div className="absolute inset-0 overflow-hidden">
        {/* Glowing Orbs */}
        <div 
          className="absolute top-1/4 left-1/4 w-[500px] h-[500px] bg-blue-600/20 rounded-full blur-[120px] animate-pulse-slow"
          style={{ transform: `translate(${scrollY * 0.1}px, ${scrollY * 0.05}px)` }}
        />
        <div 
          className="absolute bottom-1/4 right-1/4 w-[400px] h-[400px] bg-violet-600/15 rounded-full blur-[100px] animate-pulse-slow-delayed"
          style={{ transform: `translate(${-scrollY * 0.1}px, ${-scrollY * 0.05}px)` }}
        />
        <div 
          className="absolute top-3/4 left-1/2 w-[300px] h-[300px] bg-cyan-500/10 rounded-full blur-[80px] animate-pulse-slow"
          style={{ transform: `translate(${scrollY * 0.05}px, ${-scrollY * 0.1}px)` }}
        />
        
        {/* Grid Pattern */}
        <div className="absolute inset-0 bg-grid-pattern opacity-[0.03]" />
        
        {/* Floating Geometric Shapes */}
        <div className="absolute top-32 right-20 w-32 h-32 border border-blue-500/20 rotate-45 animate-float" />
        <div className="absolute top-1/2 left-10 w-24 h-24 border border-violet-500/20 rounded-full animate-float-delayed" />
        <div className="absolute bottom-40 right-1/4 w-20 h-20 border border-cyan-500/20 rotate-12 animate-float" />
      </div>

      {/* Navigation */}
      <nav className="relative z-50 px-6 py-4 lg:px-12">
        <div className="max-w-7xl mx-auto flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-cyan-500 rounded-xl flex items-center justify-center shadow-lg shadow-blue-500/30">
              <GraduationCap className="w-7 h-7 text-white" />
            </div>
            <div>
              <h1 className="text-white font-bold text-lg leading-tight">KEMENTERIAN PENDIDIKAN</h1>
              <p className="text-blue-300 text-xs font-medium">Sains, Teknologi & Inovasi Sabah</p>
            </div>
          </div>
          
          <button
            onClick={() => navigate('/login')}
            className="group px-6 py-2.5 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-500 hover:to-cyan-600 text-white font-semibold rounded-xl shadow-lg shadow-blue-500/30 hover:shadow-blue-500/50 transition-all duration-300 flex items-center space-x-2"
          >
            <span>Log Masuk</span>
            <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
          </button>
        </div>
      </nav>

      {/* Hero Section */}
      <section className={`relative z-10 px-6 lg:px-12 pt-12 pb-24 transition-all duration-1000 ${animatedElements.includes('hero') ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
        <div className="max-w-7xl mx-auto">
          <div className="grid lg:grid-cols-2 gap-12 items-center">
            {/* Left Content */}
            <div className="space-y-8">
              {/* Badge */}
              <div className="inline-flex items-center space-x-2 px-4 py-2 bg-blue-500/10 border border-blue-500/20 rounded-full">
                <Sparkles className="w-4 h-4 text-blue-400" />
                <span className="text-blue-300 text-sm font-medium">Platform Digital Kementerian</span>
              </div>

              {/* Main Heading */}
              <div className="space-y-4">
                <h1 className="text-4xl md:text-5xl lg:text-6xl font-extrabold text-white leading-tight">
                  <span className="bg-gradient-to-r from-white via-blue-100 to-white bg-clip-text text-transparent">
                    Membangun Masa Hadapan
                  </span>
                  <br />
                  <span className="bg-gradient-to-r from-blue-400 via-cyan-300 to-blue-400 bg-clip-text text-transparent">
                    Melalui Pendidikan & Inovasi
                  </span>
                </h1>
                <p className="text-lg text-blue-200/80 max-w-xl leading-relaxed">
                  Kementerian Pendidikan, Sains, Teknologi dan Inovasi Negeri Sabah - Memperkasakan generasi muda dengan ilmu pengetahuan dan kemahiran digital untuk era baharu.
                </p>
              </div>

              {/* CTA Buttons */}
              <div className="flex flex-wrap gap-4">
                <button
                  onClick={() => navigate('/login')}
                  className="group px-8 py-4 bg-gradient-to-r from-blue-600 to-cyan-600 hover:from-blue-500 hover:to-cyan-500 text-white font-bold rounded-2xl shadow-xl shadow-blue-500/30 hover:shadow-blue-500/50 transition-all duration-300 flex items-center space-x-3"
                >
                  <span>Mula Menggunakan</span>
                  <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform" />
                </button>
                <button className="group px-8 py-4 bg-white/5 hover:bg-white/10 border border-white/10 hover:border-white/20 text-white font-semibold rounded-2xl backdrop-blur-sm transition-all duration-300 flex items-center space-x-3">
                  <Play className="w-5 h-5" />
                  <span>Lihat Demo</span>
                </button>
              </div>
            </div>

            {/* Right Content - Abstract Visual */}
            <div className="relative hidden lg:block">
              <div className="relative w-full aspect-square">
                {/* Central Glow */}
                <div className="absolute inset-0 bg-gradient-to-br from-blue-500/20 to-cyan-500/20 rounded-full blur-3xl animate-pulse-slow" />
                
                {/* Rotating Ring */}
                <div className="absolute inset-8 border-2 border-dashed border-blue-500/30 rounded-full animate-spin-slow" />
                
                {/* Floating Icons */}
                <div className="absolute top-1/4 left-1/4 w-16 h-16 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl flex items-center justify-center shadow-xl shadow-blue-500/40 animate-float">
                  <GraduationCap className="w-8 h-8 text-white" />
                </div>
                <div className="absolute top-1/4 right-1/4 w-14 h-14 bg-gradient-to-br from-violet-500 to-violet-600 rounded-2xl flex items-center justify-center shadow-xl shadow-violet-500/40 animate-float-delayed">
                  <Microscope className="w-7 h-7 text-white" />
                </div>
                <div className="absolute bottom-1/3 right-1/3 w-12 h-12 bg-gradient-to-br from-cyan-500 to-cyan-600 rounded-xl flex items-center justify-center shadow-xl shadow-cyan-500/40 animate-float">
                  <Cpu className="w-6 h-6 text-white" />
                </div>
                <div className="absolute bottom-1/4 left-1/3 w-14 h-14 bg-gradient-to-br from-amber-500 to-amber-600 rounded-2xl flex items-center justify-center shadow-xl shadow-amber-500/40 animate-float-delayed">
                  <Lightbulb className="w-7 h-7 text-white" />
                </div>
                
                {/* Center Emblem */}
                <div className="absolute inset-0 flex items-center justify-center">
                  <div className="w-32 h-32 bg-gradient-to-br from-slate-800 to-slate-900 rounded-full flex items-center justify-center border-4 border-blue-500/50 shadow-2xl">
                    <Building2 className="w-16 h-16 text-blue-400" />
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Scroll Indicator */}
          <div className="absolute bottom-8 left-1/2 -translate-x-1/2 animate-bounce">
            <ChevronDown className="w-8 h-8 text-blue-400/50" />
          </div>
        </div>
      </section>

      {/* Stats Section */}
      <section className={`relative z-10 px-6 lg:px-12 py-16 bg-gradient-to-t from-slate-900/80 to-transparent backdrop-blur-sm transition-all duration-1000 delay-200 ${animatedElements.includes('stats') ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
        <div className="max-w-7xl mx-auto">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
            {stats.map((stat, index) => (
              <div 
                key={index}
                className="text-center group"
                style={{ animationDelay: `${index * 100}ms` }}
              >
                <div className="text-3xl md:text-4xl font-extrabold bg-gradient-to-r from-blue-400 to-cyan-400 bg-clip-text text-transparent mb-2 group-hover:scale-105 transition-transform">
                  {stat.value}
                </div>
                <div className="text-blue-300/70 text-sm font-medium">{stat.label}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className={`relative z-10 px-6 lg:px-12 py-24 transition-all duration-1000 delay-300 ${animatedElements.includes('features') ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
        <div className="max-w-7xl mx-auto">
          {/* Section Header */}
          <div className="text-center mb-16">
            <div className="inline-flex items-center space-x-2 px-4 py-2 bg-violet-500/10 border border-violet-500/20 rounded-full mb-6">
              <Cpu className="w-4 h-4 text-violet-400" />
              <span className="text-violet-300 text-sm font-medium">Perkhidmatan Digital</span>
            </div>
            <h2 className="text-3xl md:text-4xl font-extrabold text-white mb-4">
              <span className="bg-gradient-to-r from-white to-blue-200 bg-clip-text text-transparent">
                Perkhidmatan Kami
              </span>
            </h2>
            <p className="text-blue-200/60 max-w-2xl mx-auto">
              Pelbagai perkhidmatan digital untuk membantu anda mendapatkan maklumat dan bantuan berkaitan pendidikan di Sabah.
            </p>
          </div>

          {/* Features Grid */}
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            {features.map((feature, index) => (
              <div
                key={index}
                onMouseEnter={() => setActiveFeature(index)}
                className={`group relative p-6 rounded-2xl backdrop-blur-sm border transition-all duration-500 cursor-pointer ${
                  activeFeature === index 
                    ? 'bg-white/10 border-white/20 shadow-xl' 
                    : 'bg-white/5 border-white/10 hover:bg-white/8'
                }`}
              >
                {/* Gradient Background */}
                <div className={`absolute inset-0 bg-gradient-to-br ${feature.color} opacity-0 group-hover:opacity-10 rounded-2xl transition-opacity duration-500`} />
                
                {/* Icon */}
                <div className={`w-14 h-14 rounded-xl bg-gradient-to-br ${feature.color} flex items-center justify-center mb-4 shadow-lg group-hover:scale-110 transition-transform duration-300`}>
                  <feature.icon className="w-7 h-7 text-white" />
                </div>

                {/* Content */}
                <h3 className="text-white font-bold text-lg mb-2 group-hover:text-blue-200 transition-colors">
                  {feature.title}
                </h3>
                <p className="text-blue-200/60 text-sm leading-relaxed">
                  {feature.description}
                </p>

                {/* Arrow */}
                <div className="mt-4 flex items-center space-x-2 text-blue-400 opacity-0 group-hover:opacity-100 transition-opacity">
                  <span className="text-sm font-medium">Ketahui lebih lanjut</span>
                  <ArrowRight className="w-4 h-4" />
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* About Section */}
      <section className={`relative z-10 px-6 lg:px-12 py-24 transition-all duration-1000 delay-400 ${animatedElements.includes('about') ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
        <div className="max-w-7xl mx-auto">
          <div className="grid lg:grid-cols-2 gap-16 items-center">
            {/* Left - Image/Visual */}
            <div className="relative">
              <div className="relative aspect-video rounded-2xl overflow-hidden">
                <div className="absolute inset-0 bg-gradient-to-br from-blue-600/30 to-violet-600/30" />
                <div className="absolute inset-0 flex items-center justify-center">
                  <div className="text-center">
                    <Building2 className="w-24 h-24 text-white/50 mx-auto mb-4" />
                    <p className="text-white/50 font-medium">Ikon Kementerian</p>
                  </div>
                </div>
                {/* Decorative Elements */}
                <div className="absolute top-4 left-4 w-20 h-20 border border-white/10 rounded-full" />
                <div className="absolute bottom-4 right-4 w-32 h-32 border border-white/10 rotate-45" />
              </div>
              
              {/* Floating Badge */}
              <div className="absolute -bottom-6 -right-6 px-6 py-3 bg-gradient-to-r from-blue-600 to-cyan-600 rounded-xl shadow-xl">
                <div className="flex items-center space-x-3">
                  <Globe className="w-5 h-5 text-white" />
                  <span className="text-white font-semibold">Sabah, Malaysia</span>
                </div>
              </div>
            </div>

            {/* Right - Content */}
            <div className="space-y-8">
              <div>
                <div className="inline-flex items-center space-x-2 px-4 py-2 bg-emerald-500/10 border border-emerald-500/20 rounded-full mb-6">
                  <Building2 className="w-4 h-4 text-emerald-400" />
                  <span className="text-emerald-300 text-sm font-medium">Tentang Kami</span>
                </div>
                <h2 className="text-3xl md:text-4xl font-extrabold text-white mb-6">
                  <span className="bg-gradient-to-r from-white to-blue-200 bg-clip-text text-transparent">
                    Misi Kami untuk Sabah
                  </span>
                </h2>
                <div className="space-y-4 text-blue-200/70 leading-relaxed">
                  <p>
                    Kementerian Pendidikan, Sains, Teknologi dan Inovasi Negeri Sabah beriltizam untuk menyediakan pendidikan berkualiti tinggi kepada semua lapisan masyarakat di Sabah.
                  </p>
                  <p>
                    Kami berusaha untuk memupuk inovasi dan kreativiti dalam kalangan pelajar dan pendidik, serta menyediakan infrastruktur digital yang moden untuk menyokong pembelajaran abad ke-21.
                  </p>
                </div>
              </div>

              {/* Key Points */}
              <div className="grid sm:grid-cols-2 gap-4">
                {[
                  'Pendidikan Inklusif untuk Semua',
                  'Pemerkasaan Digital STEM',
                  'Kerjasama Global',
                  'Penyelidikan & Pembangunan'
                ].map((point, index) => (
                  <div key={index} className="flex items-center space-x-3">
                    <div className="w-6 h-6 rounded-full bg-blue-500/20 flex items-center justify-center">
                      <div className="w-2 h-2 rounded-full bg-blue-400" />
                    </div>
                    <span className="text-white text-sm font-medium">{point}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className={`relative z-10 px-6 lg:px-12 py-24 transition-all duration-1000 delay-500 ${animatedElements.includes('cta') ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
        <div className="max-w-4xl mx-auto">
          <div className="relative p-12 rounded-3xl overflow-hidden">
            {/* Background Gradient */}
            <div className="absolute inset-0 bg-gradient-to-r from-blue-600 via-violet-600 to-blue-600" />
            <div className="absolute inset-0 bg-grid-pattern opacity-10" />
            
            {/* Animated Border */}
            <div className="absolute inset-[2px] rounded-3xl bg-gradient-to-r from-blue-600 via-violet-600 to-blue-600" />
            <div className="absolute inset-0 rounded-3xl bg-slate-950/90" />
            
            {/* Content */}
            <div className="relative text-center space-y-8">
              <div className="inline-flex items-center justify-center w-16 h-16 bg-white/10 rounded-2xl backdrop-blur-sm">
                <MessageSquare className="w-8 h-8 text-white" />
              </div>
              
              <div>
                <h2 className="text-3xl md:text-4xl font-extrabold text-white mb-4">
                  Berjumpa dengan AI Kami
                </h2>
                <p className="text-blue-100/70 max-w-xl mx-auto">
                  Ada soalan tentang pendidikan, sains, atau teknologi? Chatbot AI kami sedia membantu anda 24/7 dengan maklumat terkini dari Kementerian.
                </p>
              </div>

              <button
                onClick={() => navigate('/login')}
                className="group px-10 py-4 bg-white text-blue-600 font-bold rounded-2xl shadow-xl hover:shadow-2xl hover:scale-105 transition-all duration-300 inline-flex items-center space-x-3"
              >
                <Bot className="w-5 h-5" />
                <span>Mulakan Perbualan</span>
                <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform" />
              </button>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="relative z-10 px-6 lg:px-12 py-12 border-t border-white/10">
        <div className="max-w-7xl mx-auto">
          <div className="flex flex-col md:flex-row items-center justify-between space-y-4 md:space-y-0">
            <div className="flex items-center space-x-4">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-cyan-500 rounded-lg flex items-center justify-center">
                <GraduationCap className="w-5 h-5 text-white" />
              </div>
              <div>
                <p className="text-white font-semibold text-sm">KEMENTERIAN PENDIDIKAN</p>
                <p className="text-blue-400/60 text-xs">Sains, Teknologi & Inovasi Sabah</p>
              </div>
            </div>
            
            <p className="text-blue-400/40 text-sm">
              © 2025 Kementerian Pendidikan, Sains, Teknologi dan Inovasi Negeri Sabah. Hak Cipta Terpelihara.
            </p>
          </div>
        </div>
      </footer>

      {/* Custom Styles */}
      <style>{`
        @keyframes float {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          50% { transform: translateY(-20px) rotate(5deg); }
        }
        @keyframes float-delayed {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          50% { transform: translateY(-15px) rotate(-5deg); }
        }
        @keyframes spin-slow {
          from { transform: rotate(0deg); }
          to { transform: rotate(360deg); }
        }
        .animate-float {
          animation: float 6s ease-in-out infinite;
        }
        .animate-float-delayed {
          animation: float-delayed 8s ease-in-out infinite;
        }
        .animate-spin-slow {
          animation: spin-slow 30s linear infinite;
        }
        .bg-grid-pattern {
          background-image: 
            linear-gradient(rgba(255,255,255,0.03) 1px, transparent 1px),
            linear-gradient(90deg, rgba(255,255,255,0.03) 1px, transparent 1px);
          background-size: 50px 50px;
        }
      `}</style>
    </div>
  )
}

export default Landing