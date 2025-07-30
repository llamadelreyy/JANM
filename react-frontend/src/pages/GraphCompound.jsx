import { useState, useEffect } from 'react'
import { FileSpreadsheet, TrendingUp, Calendar, Users, Download, Filter } from 'lucide-react'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, LineChart, Line, PieChart, Pie, Cell, AreaChart, Area } from 'recharts'
import PageTemplate from '../components/UI/PageTemplate'
import StatsCard from '../components/UI/StatsCard'
import { toast } from 'sonner'

const GraphCompound = () => {
  const [loading, setLoading] = useState(true)
  const [selectedYear, setSelectedYear] = useState('2024')
  const [selectedPeriod, setSelectedPeriod] = useState('monthly')
  const [stats, setStats] = useState({
    totalCompounds: 0,
    totalAmount: 0,
    paidCompounds: 0,
    pendingCompounds: 0
  })
  const [chartData, setChartData] = useState({
    monthly: [],
    byType: [],
    byStatus: [],
    trend: [],
    byOfficer: []
  })

  // Mock data
  const mockStats = {
    totalCompounds: 2847,
    totalAmount: 1423500,
    paidCompounds: 2156,
    pendingCompounds: 691
  }

  const mockMonthlyData = [
    { month: 'Jan', issued: 245, paid: 189, amount: 122500, pending: 56 },
    { month: 'Feb', issued: 234, paid: 198, amount: 117000, pending: 36 },
    { month: 'Mar', issued: 267, paid: 201, amount: 133500, pending: 66 },
    { month: 'Apr', issued: 223, paid: 187, amount: 111500, pending: 36 },
    { month: 'May', issued: 289, paid: 234, amount: 144500, pending: 55 },
    { month: 'Jun', issued: 256, paid: 198, amount: 128000, pending: 58 },
    { month: 'Jul', issued: 278, paid: 221, amount: 139000, pending: 57 },
    { month: 'Aug', issued: 234, paid: 189, amount: 117000, pending: 45 },
    { month: 'Sep', issued: 245, paid: 201, amount: 122500, pending: 44 },
    { month: 'Oct', issued: 267, paid: 234, amount: 133500, pending: 33 },
    { month: 'Nov', issued: 289, paid: 256, amount: 144500, pending: 33 },
    { month: 'Dec', issued: 220, paid: 248, amount: 110000, pending: 72 }
  ]

  const mockTypeData = [
    { type: 'Lesen Perniagaan', count: 856, amount: 428000, percentage: 30.1, color: '#3B82F6' },
    { type: 'Kebersihan', count: 654, amount: 327000, percentage: 23.0, color: '#10B981' },
    { type: 'Bangunan', count: 512, amount: 256000, percentage: 18.0, color: '#F59E0B' },
    { type: 'Parkir', count: 445, amount: 222500, percentage: 15.6, color: '#EF4444' },
    { type: 'Iklan', count: 234, amount: 117000, percentage: 8.2, color: '#8B5CF6' },
    { type: 'Lain-lain', count: 146, amount: 73000, percentage: 5.1, color: '#6B7280' }
  ]

  const mockStatusData = [
    { status: 'Dibayar', count: 2156, percentage: 75.7, color: '#10B981' },
    { status: 'Tertunggak', count: 691, percentage: 24.3, color: '#EF4444' }
  ]

  const mockTrendData = [
    { year: '2020', compounds: 2234, amount: 1117000, rate: 72.5 },
    { year: '2021', compounds: 2456, amount: 1228000, rate: 74.2 },
    { year: '2022', compounds: 2634, amount: 1317000, rate: 75.8 },
    { year: '2023', compounds: 2789, amount: 1394500, rate: 76.4 },
    { year: '2024', compounds: 2847, amount: 1423500, rate: 75.7 }
  ]

  const mockOfficerData = [
    { officer: 'Ahmad Bin Ali', compounds: 234, amount: 117000, rate: 78.2 },
    { officer: 'Siti Aminah', compounds: 198, amount: 99000, rate: 76.8 },
    { officer: 'Mohd Farid', compounds: 187, amount: 93500, rate: 75.4 },
    { officer: 'Lim Ah Chong', compounds: 176, amount: 88000, rate: 74.1 },
    { officer: 'Rajesh Kumar', compounds: 165, amount: 82500, rate: 73.3 }
  ]

  useEffect(() => {
    loadData()
  }, [selectedYear, selectedPeriod])

  const loadData = async () => {
    setLoading(true)
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      setStats(mockStats)
      setChartData({
        monthly: mockMonthlyData,
        byType: mockTypeData,
        byStatus: mockStatusData,
        trend: mockTrendData,
        byOfficer: mockOfficerData
      })
    } catch (error) {
      toast.error('Gagal memuat data kompaun')
    } finally {
      setLoading(false)
    }
  }

  const handleExport = () => {
    toast.success('Graf sedang dieksport...')
  }

  const formatCurrency = (value) => {
    return `RM ${(value / 1000).toFixed(0)}K`
  }

  return (
    <PageTemplate
      title="Graf Kompaun"
      subtitle="Analisis statistik dan trend kompaun penguatkuasaan"
      icon={FileSpreadsheet}
      onRefresh={loadData}
      loading={loading}
      breadcrumbs={[
        { label: 'Graf Statistik' },
        { label: 'Graf Kompaun' }
      ]}
      actions={
        <div className="flex gap-3">
          <select
            value={selectedPeriod}
            onChange={(e) => setSelectedPeriod(e.target.value)}
            className="select"
          >
            <option value="monthly">Bulanan</option>
            <option value="quarterly">Suku Tahun</option>
            <option value="yearly">Tahunan</option>
          </select>
          <select
            value={selectedYear}
            onChange={(e) => setSelectedYear(e.target.value)}
            className="select"
          >
            <option value="2024">2024</option>
            <option value="2023">2023</option>
            <option value="2022">2022</option>
          </select>
          <button onClick={handleExport} className="btn-primary">
            <Download className="h-4 w-4 mr-2" />
            Eksport
          </button>
        </div>
      }
    >
      {/* Statistics Cards */}
      <div className="professional-stats mb-8">
        <StatsCard
          title="Jumlah Kompaun"
          value={stats.totalCompounds}
          subtitle="Kompaun dikeluarkan"
          icon={FileSpreadsheet}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Jumlah Nilai"
          value={formatCurrency(stats.totalAmount)}
          subtitle="Nilai keseluruhan"
          icon={TrendingUp}
          color="green"
          loading={loading}
        />
        <StatsCard
          title="Kompaun Dibayar"
          value={stats.paidCompounds}
          subtitle={`${((stats.paidCompounds / stats.totalCompounds) * 100).toFixed(1)}% kadar bayaran`}
          icon={Calendar}
          color="purple"
          trend="up"
          trendValue="+3.2%"
          loading={loading}
        />
        <StatsCard
          title="Kompaun Tertunggak"
          value={stats.pendingCompounds}
          subtitle="Belum dibayar"
          icon={Users}
          color="red"
          loading={loading}
        />
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Monthly Compounds Chart */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Kompaun Bulanan {selectedYear}</h3>
            <p className="card-subtitle">Trend pengeluaran dan pembayaran kompaun</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <AreaChart data={chartData.monthly}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  dataKey="month" 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <Tooltip 
                  formatter={(value, name) => [
                    value,
                    name === 'issued' ? 'Dikeluarkan' : 
                    name === 'paid' ? 'Dibayar' : 'Tertunggak'
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Area 
                  type="monotone" 
                  dataKey="issued" 
                  stackId="1"
                  stroke="#3b82f6" 
                  fill="#3b82f6"
                  fillOpacity={0.6}
                />
                <Area 
                  type="monotone" 
                  dataKey="paid" 
                  stackId="2"
                  stroke="#10b981" 
                  fill="#10b981"
                  fillOpacity={0.6}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Compound by Type */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Kompaun Mengikut Jenis</h3>
            <p className="card-subtitle">Taburan kompaun berdasarkan kategori</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.byType}
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  dataKey="percentage"
                  label={({ type, percentage }) => `${type} (${percentage}%)`}
                >
                  {chartData.byType.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip 
                  formatter={(value, name, props) => [
                    `${value}% (${props.payload.count} kompaun)`,
                    'Peratusan'
                  ]}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Payment Status */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Status Pembayaran</h3>
            <p className="card-subtitle">Kadar pembayaran kompaun</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData.byStatus}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  dataKey="status"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <Tooltip 
                  formatter={(value, name) => [value, 'Bilangan']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar 
                  dataKey="count" 
                  fill={(entry) => entry.color}
                  radius={[4, 4, 0, 0]}
                >
                  {chartData.byStatus.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* 5-Year Trend */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Trend 5 Tahun</h3>
            <p className="card-subtitle">Perkembangan kompaun dan kadar bayaran</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={chartData.trend}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  dataKey="year"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  yAxisId="left"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  yAxisId="right"
                  orientation="right"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  tickFormatter={(value) => `${value}%`}
                />
                <Tooltip 
                  formatter={(value, name) => [
                    name === 'compounds' ? value :
                    name === 'amount' ? formatCurrency(value) :
                    `${value}%`,
                    name === 'compounds' ? 'Bilangan' :
                    name === 'amount' ? 'Nilai' : 'Kadar Bayaran'
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Line 
                  yAxisId="left"
                  type="monotone" 
                  dataKey="compounds" 
                  stroke="#3b82f6" 
                  strokeWidth={3}
                  dot={{ fill: '#3b82f6', strokeWidth: 2, r: 4 }}
                />
                <Line 
                  yAxisId="right"
                  type="monotone" 
                  dataKey="rate" 
                  stroke="#8b5cf6" 
                  strokeWidth={3}
                  dot={{ fill: '#8b5cf6', strokeWidth: 2, r: 4 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Officer Performance */}
      <div className="professional-card">
        <div className="card-header">
          <h3 className="card-title">Prestasi Pegawai Penguatkuasa</h3>
          <p className="card-subtitle">Top 5 pegawai berdasarkan bilangan kompaun</p>
        </div>
        <div className="card-content">
          <ResponsiveContainer width="100%" height={400}>
            <BarChart data={chartData.byOfficer} layout="horizontal">
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis 
                type="number"
                tick={{ fontSize: 12, fill: '#64748b' }}
                axisLine={{ stroke: '#e2e8f0' }}
              />
              <YAxis 
                type="category"
                dataKey="officer"
                tick={{ fontSize: 12, fill: '#64748b' }}
                axisLine={{ stroke: '#e2e8f0' }}
                width={120}
              />
              <Tooltip 
                formatter={(value, name) => [
                  value,
                  name === 'compounds' ? 'Bilangan Kompaun' :
                  name === 'amount' ? 'Nilai (RM)' : 'Kadar Bayaran (%)'
                ]}
                labelStyle={{ color: '#1e293b' }}
              />
              <Bar dataKey="compounds" fill="#3b82f6" radius={[0, 4, 4, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </PageTemplate>
  )
}

export default GraphCompound