import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  TrendingUp,
  Users,
  FileText,
  AlertTriangle,
  CheckCircle,
  Clock,
  MapPin,
  Calendar,
  Target,
  Award,
  Activity,
  DollarSign,
  Map
} from 'lucide-react'
import { 
  BarChart, 
  Bar, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  LineChart,
  Line,
  PieChart,
  Pie,
  Cell,
  AreaChart,
  Area
} from 'recharts'
import PageTemplate from '../components/UI/PageTemplate'
import StatsCard from '../components/UI/StatsCard'
import DataTable from '../components/UI/DataTable'
import AnimatedRingChart from '../components/Charts/AnimatedRingChart'

const ExecutiveSummary = () => {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(true)
  const [selectedLocation, setSelectedLocation] = useState('telok-gong')
  const [stats, setStats] = useState({})
  const [chartData, setChartData] = useState({})
  const [recentActivities, setRecentActivities] = useState([])
  const [topPerformers, setTopPerformers] = useState([])

  // Real data from PBT system - Updated according to requirements
  const mockStats = {
    totalPremises: 752, // Bilangan Premis
    taxedPremises: 672, // Bercukai
    noTaxInfo: 80, // Tidak Bercukai (as specified by user)
    totalLicenses: 883, // Status Lesen total
    licensedPremises: 414, // Berlesen
    unlicensedPremises: 469, // Tidak Berlesen
    taxComplianceRate: ((737 / 755) * 100).toFixed(1),
    licenseComplianceRate: ((414 / 883) * 100).toFixed(1)
  }

  const mockMonthlyTrend = [
    { month: 'Jan', enforcement: 245, revenue: 122500, compliance: 85.2, inspections: 98 },
    { month: 'Feb', enforcement: 234, revenue: 117000, compliance: 86.1, inspections: 102 },
    { month: 'Mar', enforcement: 267, revenue: 133500, compliance: 87.3, inspections: 115 },
    { month: 'Apr', enforcement: 223, revenue: 111500, compliance: 88.1, inspections: 89 },
    { month: 'May', enforcement: 289, revenue: 144500, compliance: 87.8, inspections: 125 },
    { month: 'Jun', enforcement: 256, revenue: 128000, compliance: 88.5, inspections: 108 }
  ]

  // Data for the 3 animated ring charts
  const premisesData = [
    { name: 'Total Premis', value: 752, color: '#6366F1' }
  ]

  const licenseData = [
    {
      name: 'Berlesen',
      value: 414,
      color: '#10B981',
      percentage: ((414 / 883) * 100).toFixed(1),
      count: 414
    },
    {
      name: 'Tidak Berlesen',
      value: 469,
      color: '#EF4444',
      percentage: ((469 / 883) * 100).toFixed(1),
      count: 469
    }
  ]

  const taxData = [
    {
      name: 'Bercukai',
      value: 672,
      color: '#3B82F6',
      percentage: ((672 / 752) * 100).toFixed(1),
      count: 672
    },
    {
      name: 'Tidak Bercukai',
      value: 80,
      color: '#F59E0B',
      percentage: ((80 / 752) * 100).toFixed(1),
      count: 80
    }
  ]

  const mockRecentActivities = [
    {
      id: 1,
      type: 'Kompaun',
      description: 'Kompaun lesen perniagaan - Restoran Seri Wangi',
      officer: 'Ahmad Bin Ali',
      amount: 500,
      status: 'Selesai',
      date: '2024-01-15 14:30',
      location: 'Jalan Merdeka, KL'
    },
    {
      id: 2,
      type: 'Pemeriksaan',
      description: 'Pemeriksaan kebersihan premis makanan',
      officer: 'Siti Aminah',
      amount: 0,
      status: 'Dalam Proses',
      date: '2024-01-15 11:15',
      location: 'Jalan Bangsar, KL'
    },
    {
      id: 3,
      type: 'Notis',
      description: 'Notis penutupan sementara - Kedai Runcit ABC',
      officer: 'Mohd Farid',
      amount: 0,
      status: 'Tertunggak',
      date: '2024-01-14 16:45',
      location: 'Jalan Pudu, KL'
    },
    {
      id: 4,
      type: 'Kompaun',
      description: 'Kompaun parkir tanpa lesen',
      officer: 'Lim Ah Chong',
      amount: 300,
      status: 'Selesai',
      date: '2024-01-14 09:20',
      location: 'Jalan Bukit Bintang, KL'
    },
    {
      id: 5,
      type: 'Sitaan',
      description: 'Sitaan barang jualan tanpa lesen',
      officer: 'Rajesh Kumar',
      amount: 0,
      status: 'Dalam Proses',
      date: '2024-01-13 13:10',
      location: 'Jalan Cheras, KL'
    }
  ]

  const mockTopPerformers = [
    { officer: 'Ahmad Bin Ali', cases: 89, revenue: 44500, compliance: 92.1, rank: 1 },
    { officer: 'Siti Aminah', cases: 76, revenue: 38000, compliance: 89.5, rank: 2 },
    { officer: 'Mohd Farid', cases: 68, revenue: 34000, compliance: 87.8, rank: 3 },
    { officer: 'Lim Ah Chong', cases: 62, revenue: 31000, compliance: 86.2, rank: 4 },
    { officer: 'Rajesh Kumar', cases: 58, revenue: 29000, compliance: 85.1, rank: 5 }
  ]

  useEffect(() => {
    loadData()
  }, [selectedLocation])

  const loadData = async () => {
    setLoading(true)
    try {
      await new Promise(resolve => setTimeout(resolve, 1000))
      setStats(mockStats)
      setChartData({
        monthly: mockMonthlyTrend,
        premises: premisesData,
        license: licenseData,
        tax: taxData
      })
      setRecentActivities(mockRecentActivities)
      setTopPerformers(mockTopPerformers)
    } catch (error) {
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleViewMap = () => {
    // Navigate to internal map page with selected location
    navigate(`/map?location=${selectedLocation}`)
  }

  const activityColumns = [
    {
      key: 'type',
      label: 'Jenis',
      render: (value) => (
        <span className={`badge ${
          value === 'Kompaun' ? 'badge-success' :
          value === 'Pemeriksaan' ? 'badge-primary' :
          value === 'Notis' ? 'badge-warning' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'description',
      label: 'Penerangan',
      render: (value, item) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{item.location}</div>
        </div>
      )
    },
    {
      key: 'officer',
      label: 'Pegawai'
    },
    {
      key: 'amount',
      label: 'Jumlah (RM)',
      render: (value) => (
        <div className="text-right professional-number">
          {value > 0 ? value.toLocaleString('ms-MY') : '-'}
        </div>
      )
    },
    {
      key: 'status',
      label: 'Status',
      render: (value) => (
        <span className={`badge ${
          value === 'Selesai' ? 'badge-success' :
          value === 'Dalam Proses' ? 'badge-primary' :
          'badge-warning'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'date',
      label: 'Tarikh',
      render: (value) => (
        <div className="text-sm text-slate-600">
          {new Date(value).toLocaleString('ms-MY')}
        </div>
      )
    }
  ]

  const performerColumns = [
    {
      key: 'rank',
      label: 'Kedudukan',
      render: (value) => (
        <div className="flex items-center">
          <div className={`w-6 h-6 rounded-full flex items-center justify-center text-xs font-bold text-white ${
            value === 1 ? 'bg-yellow-500' :
            value === 2 ? 'bg-gray-400' :
            value === 3 ? 'bg-amber-600' :
            'bg-slate-400'
          }`}>
            {value}
          </div>
        </div>
      )
    },
    {
      key: 'officer',
      label: 'Pegawai',
      render: (value) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'cases',
      label: 'Kes',
      render: (value) => (
        <div className="text-center professional-number">{value}</div>
      )
    },
    {
      key: 'revenue',
      label: 'Hasil (RM)',
      render: (value) => (
        <div className="text-right professional-number text-green-600">
          {value.toLocaleString('ms-MY')}
        </div>
      )
    },
    {
      key: 'compliance',
      label: 'Kadar Pematuhan (%)',
      render: (value) => (
        <div className="text-center">
          <span className="professional-number">{value}%</span>
        </div>
      )
    }
  ]

  return (
    <PageTemplate
      title="Ringkasan Eksekutif"
      subtitle="Papan pemuka utama sistem penguatkuasaan PBT"
      icon={TrendingUp}
      onRefresh={loadData}
      loading={loading}
      actions={
        <div className="flex gap-3 items-center">
          <select
            value={selectedLocation}
            onChange={(e) => setSelectedLocation(e.target.value)}
            className="select w-40"
          >
            <option value="telok-gong">Telok Gong</option>
            <option value="selat-klang">Selat Klang</option>
            <option value="bandar-baru-klang">Bandar Baru Klang</option>
            <option value="pelabuhan-klang">Pelabuhan Klang</option>
            <option value="pandamaran">Pandamaran</option>
            <option value="sentosa">Sentosa</option>
            <option value="sungai-kandis">Sungai Kandis</option>
            <option value="kota-kemuning">Kota Kemuning</option>
          </select>
          <button
            onClick={handleViewMap}
            className="btn-primary whitespace-nowrap min-w-[120px]"
            title="Lihat peta kawasan terpilih"
          >
            <Map className="h-4 w-4 mr-2" />
            View Map
          </button>
        </div>
      }
    >
      {/* Main KPI Boxes - 3 Animated Ring Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 mb-8">
        {/* Box 1: Bilangan Premis */}
        <div className="w-full">
          <AnimatedRingChart
            data={chartData.premises || premisesData}
            title="Bilangan Premis"
            total={752}
            centerText="Premis"
            animationDuration={2000}
          />
        </div>

        {/* Box 2: Status Lesen */}
        <div className="w-full">
          <AnimatedRingChart
            data={chartData.license || licenseData}
            title="Status Lesen"
            total={883}
            centerText="Lesen"
            animationDuration={2000}
          />
        </div>

        {/* Box 3: Status Cukai */}
        <div className="w-full">
          <AnimatedRingChart
            data={chartData.tax || taxData}
            title="Status Cukai"
            total={752}
            centerText="Cukai"
            animationDuration={2000}
          />
        </div>
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Monthly Trend */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Trend Penguatkuasaan Bulanan</h3>
            <p className="card-subtitle">Prestasi penguatkuasaan dan kutipan</p>
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
                    name === 'enforcement' ? value : 
                    name === 'revenue' ? `RM ${(value / 1000).toFixed(0)}K` :
                    name === 'compliance' ? `${value}%` : value,
                    name === 'enforcement' ? 'Kes' :
                    name === 'revenue' ? 'Hasil' :
                    name === 'compliance' ? 'Pematuhan' : 'Pemeriksaan'
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Area 
                  type="monotone" 
                  dataKey="enforcement" 
                  stackId="1"
                  stroke="#3b82f6" 
                  fill="#3b82f6"
                  fillOpacity={0.6}
                />
                <Area 
                  type="monotone" 
                  dataKey="inspections" 
                  stackId="2"
                  stroke="#10b981" 
                  fill="#10b981"
                  fillOpacity={0.6}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* License Compliance Status */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Status Pematuhan Lesen</h3>
            <p className="card-subtitle">Taburan status lesen premis</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.license}
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  dataKey="value"
                  label={({ name, percentage }) => `${name} (${percentage}%)`}
                >
                  {chartData.license?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip
                  formatter={(value, name, props) => [
                    `${value}% (${props.payload.count} premis)`,
                    'Peratusan'
                  ]}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Tax Compliance Chart */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Status Pematuhan Cukai</h3>
            <p className="card-subtitle">Taburan maklumat cukai premis</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.tax}
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  dataKey="value"
                  label={({ name, percentage }) => `${name} (${percentage}%)`}
                >
                  {chartData.tax?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip
                  formatter={(value, name, props) => [
                    `${value}% (${props.payload.count} premis)`,
                    'Peratusan'
                  ]}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Additional Statistics Card */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Ringkasan Pematuhan</h3>
            <p className="card-subtitle">Statistik keseluruhan sistem</p>
          </div>
          <div className="card-content">
            <div className="space-y-4">
              <div className="flex justify-between items-center p-3 bg-blue-50 rounded-lg">
                <span className="text-sm font-medium text-blue-700">Kadar Pematuhan Lesen</span>
                <span className="text-lg font-bold text-blue-600">{stats.licenseComplianceRate}%</span>
              </div>
              <div className="flex justify-between items-center p-3 bg-green-50 rounded-lg">
                <span className="text-sm font-medium text-green-700">Kadar Pematuhan Cukai</span>
                <span className="text-lg font-bold text-green-600">{stats.taxComplianceRate}%</span>
              </div>
              <div className="flex justify-between items-center p-3 bg-purple-50 rounded-lg">
                <span className="text-sm font-medium text-purple-700">Jumlah Premis</span>
                <span className="text-lg font-bold text-purple-600">{stats.totalPremises}</span>
              </div>
              <div className="flex justify-between items-center p-3 bg-orange-50 rounded-lg">
                <span className="text-sm font-medium text-orange-700">Jumlah Lesen</span>
                <span className="text-lg font-bold text-orange-600">{stats.totalLicenses}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Data Tables */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Recent Activities */}
        <div className="lg:col-span-2">
          <DataTable
            data={recentActivities}
            columns={activityColumns}
            title="Aktiviti Terkini"
            subtitle="Senarai aktiviti penguatkuasaan terbaru"
            loading={loading}
            searchable={true}
            sortable={true}
            actions={false}
          />
        </div>
      </div>

      {/* Top Performers */}
      <div className="professional-card">
        <div className="card-header">
          <h3 className="card-title">Prestasi Pegawai Terbaik</h3>
          <p className="card-subtitle">Ranking pegawai berdasarkan prestasi bulanan</p>
        </div>
        <div className="card-content">
          <div className="overflow-x-auto">
            <table className="gov-table">
              <thead>
                <tr>
                  {performerColumns.map((column) => (
                    <th key={column.key} className="gov-table th">
                      {column.label}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {topPerformers.map((performer, index) => (
                  <tr key={index} className="gov-table tr">
                    {performerColumns.map((column) => (
                      <td key={column.key} className="gov-table td">
                        {column.render ? 
                          column.render(performer[column.key], performer) : 
                          performer[column.key]
                        }
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </PageTemplate>
  )
}

export default ExecutiveSummary