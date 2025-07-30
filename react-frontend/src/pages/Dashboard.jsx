import { useState, useEffect } from 'react'
import { 
  Shield, 
  Users, 
  FileText, 
  AlertTriangle, 
  CheckCircle, 
  Clock,
  TrendingUp,
  MapPin,
  Calendar,
  Target,
  Activity,
  DollarSign,
  Eye,
  Search,
  Filter
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
  Area,
  ComposedChart
} from 'recharts'
import PageTemplate from '../components/UI/PageTemplate'
import StatsCard from '../components/UI/StatsCard'
import DataTable from '../components/UI/DataTable'

const Dashboard = () => {
  const [loading, setLoading] = useState(true)
  const [selectedFilter, setSelectedFilter] = useState('all')
  const [selectedPeriod, setSelectedPeriod] = useState('monthly')
  const [enforcementStats, setEnforcementStats] = useState({})
  const [chartData, setChartData] = useState({})
  const [recentCases, setRecentCases] = useState([])
  const [officerPerformance, setOfficerPerformance] = useState([])
  const [violationHotspots, setViolationHotspots] = useState([])

  // Mock comprehensive enforcement data
  const mockEnforcementStats = {
    totalCases: 3456,
    activeCases: 469,
    resolvedCases: 2987,
    pendingCases: 156,
    totalOfficers: 45,
    activeOfficers: 42,
    totalInspections: 1234,
    violationsFound: 456,
    compoundsIssued: 2847,
    totalRevenue: 1423500,
    complianceRate: 87.5,
    averageResolutionTime: 5.2 // days
  }

  const mockMonthlyTrend = [
    { 
      month: 'Jan', 
      cases: 245, 
      resolved: 189, 
      pending: 56, 
      inspections: 98, 
      violations: 45, 
      revenue: 122500,
      compliance: 85.2 
    },
    { 
      month: 'Feb', 
      cases: 234, 
      resolved: 198, 
      pending: 36, 
      inspections: 102, 
      violations: 38, 
      revenue: 117000,
      compliance: 86.1 
    },
    { 
      month: 'Mar', 
      cases: 267, 
      resolved: 201, 
      pending: 66, 
      inspections: 115, 
      violations: 52, 
      revenue: 133500,
      compliance: 87.3 
    },
    { 
      month: 'Apr', 
      cases: 223, 
      resolved: 187, 
      pending: 36, 
      inspections: 89, 
      violations: 34, 
      revenue: 111500,
      compliance: 88.1 
    },
    { 
      month: 'May', 
      cases: 289, 
      resolved: 234, 
      pending: 55, 
      inspections: 125, 
      violations: 48, 
      revenue: 144500,
      compliance: 87.8 
    },
    { 
      month: 'Jun', 
      cases: 256, 
      resolved: 198, 
      pending: 58, 
      inspections: 108, 
      violations: 41, 
      revenue: 128000,
      compliance: 88.5 
    }
  ]

  const mockViolationTypes = [
    { type: 'Lesen Perniagaan', count: 856, severity: 'Tinggi', color: '#EF4444' },
    { type: 'Kebersihan Premis', count: 654, severity: 'Sederhana', color: '#F59E0B' },
    { type: 'Bangunan Tanpa Kelulusan', count: 512, severity: 'Tinggi', color: '#EF4444' },
    { type: 'Parkir Tanpa Lesen', count: 445, severity: 'Rendah', color: '#10B981' },
    { type: 'Iklan Tanpa Kelulusan', count: 234, severity: 'Sederhana', color: '#F59E0B' },
    { type: 'Pencemaran Bunyi', count: 189, severity: 'Sederhana', color: '#F59E0B' },
    { type: 'Lain-lain', count: 146, severity: 'Rendah', color: '#6B7280' }
  ]

  const mockRecentCases = [
    {
      id: 'ENF001234',
      type: 'Kompaun',
      violation: 'Lesen Perniagaan Tamat Tempoh',
      location: 'Restoran Seri Wangi, Jalan Merdeka',
      officer: 'Ahmad Bin Ali',
      amount: 500,
      status: 'Selesai',
      priority: 'Tinggi',
      dateCreated: '2024-01-15 14:30',
      dateResolved: '2024-01-15 16:45',
      description: 'Premis beroperasi tanpa lesen perniagaan yang sah'
    },
    {
      id: 'ENF001235',
      type: 'Pemeriksaan',
      violation: 'Kebersihan Premis Makanan',
      location: 'Kedai Makan Pak Ali, Jalan Bangsar',
      officer: 'Siti Aminah Binti Hassan',
      amount: 0,
      status: 'Dalam Proses',
      priority: 'Sederhana',
      dateCreated: '2024-01-15 11:15',
      dateResolved: null,
      description: 'Pemeriksaan rutin kebersihan premis makanan'
    },
    {
      id: 'ENF001236',
      type: 'Notis',
      violation: 'Bangunan Tanpa Kelulusan',
      location: 'Lot 123, Jalan Pudu',
      officer: 'Mohd Farid Bin Ibrahim',
      amount: 0,
      status: 'Tertunggak',
      priority: 'Tinggi',
      dateCreated: '2024-01-14 16:45',
      dateResolved: null,
      description: 'Pembinaan struktur tanpa kelulusan pihak berkuasa'
    },
    {
      id: 'ENF001237',
      type: 'Kompaun',
      violation: 'Parkir Tanpa Lesen',
      location: 'Jalan Bukit Bintang (depan Plaza)',
      officer: 'Lim Ah Chong',
      amount: 300,
      status: 'Selesai',
      priority: 'Rendah',
      dateCreated: '2024-01-14 09:20',
      dateResolved: '2024-01-14 09:25',
      description: 'Kenderaan diparkir tanpa lesen parkir yang sah'
    },
    {
      id: 'ENF001238',
      type: 'Sitaan',
      violation: 'Jualan Tanpa Lesen',
      location: 'Pasar Malam Jalan Cheras',
      officer: 'Rajesh Kumar',
      amount: 0,
      status: 'Dalam Proses',
      priority: 'Sederhana',
      dateCreated: '2024-01-13 13:10',
      dateResolved: null,
      description: 'Sitaan barang jualan tanpa lesen penjaja'
    }
  ]

  const mockOfficerPerformance = [
    { 
      officer: 'Ahmad Bin Ali', 
      cases: 89, 
      resolved: 82, 
      pending: 7, 
      revenue: 44500, 
      avgResolution: 4.2,
      compliance: 92.1,
      efficiency: 95.2
    },
    { 
      officer: 'Siti Aminah Binti Hassan', 
      cases: 76, 
      resolved: 68, 
      pending: 8, 
      revenue: 38000, 
      avgResolution: 5.1,
      compliance: 89.5,
      efficiency: 91.8
    },
    { 
      officer: 'Mohd Farid Bin Ibrahim', 
      cases: 68, 
      resolved: 60, 
      pending: 8, 
      revenue: 34000, 
      avgResolution: 5.8,
      compliance: 87.8,
      efficiency: 88.2
    },
    { 
      officer: 'Lim Ah Chong', 
      cases: 62, 
      resolved: 55, 
      pending: 7, 
      revenue: 31000, 
      avgResolution: 4.9,
      compliance: 86.2,
      efficiency: 89.7
    },
    { 
      officer: 'Rajesh Kumar', 
      cases: 58, 
      resolved: 49, 
      pending: 9, 
      revenue: 29000, 
      avgResolution: 6.2,
      compliance: 85.1,
      efficiency: 84.5
    }
  ]

  const mockViolationHotspots = [
    { area: 'Jalan Merdeka', violations: 45, type: 'Lesen Perniagaan', risk: 'Tinggi' },
    { area: 'Jalan Bangsar', violations: 38, type: 'Kebersihan', risk: 'Sederhana' },
    { area: 'Jalan Pudu', violations: 34, type: 'Bangunan', risk: 'Tinggi' },
    { area: 'Jalan Bukit Bintang', violations: 29, type: 'Parkir', risk: 'Rendah' },
    { area: 'Jalan Cheras', violations: 25, type: 'Jualan', risk: 'Sederhana' }
  ]

  useEffect(() => {
    loadData()
  }, [selectedFilter, selectedPeriod])

  const loadData = async () => {
    setLoading(true)
    try {
      await new Promise(resolve => setTimeout(resolve, 1000))
      setEnforcementStats(mockEnforcementStats)
      setChartData({
        monthly: mockMonthlyTrend,
        violations: mockViolationTypes
      })
      setRecentCases(mockRecentCases)
      setOfficerPerformance(mockOfficerPerformance)
      setViolationHotspots(mockViolationHotspots)
    } catch (error) {
      console.error('Error loading enforcement data:', error)
    } finally {
      setLoading(false)
    }
  }

  const caseColumns = [
    {
      key: 'id',
      label: 'ID Kes',
      render: (value) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'type',
      label: 'Jenis',
      render: (value) => (
        <span className={`badge ${
          value === 'Kompaun' ? 'badge-success' :
          value === 'Pemeriksaan' ? 'badge-primary' :
          value === 'Notis' ? 'badge-warning' :
          value === 'Sitaan' ? 'badge-danger' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'violation',
      label: 'Pelanggaran',
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
      key: 'priority',
      label: 'Keutamaan',
      render: (value) => (
        <span className={`badge ${
          value === 'Tinggi' ? 'badge-danger' :
          value === 'Sederhana' ? 'badge-warning' :
          'badge-success'
        }`}>
          {value}
        </span>
      )
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
    }
  ]

  const officerColumns = [
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
      render: (value, officer) => (
        <div className="text-center">
          <div className="professional-number font-semibold">{value}</div>
          <div className="text-xs text-slate-500">{officer.resolved} selesai</div>
        </div>
      )
    },
    {
      key: 'efficiency',
      label: 'Kecekapan (%)',
      render: (value) => (
        <div className="text-center">
          <div className={`professional-number font-semibold ${
            value >= 90 ? 'text-green-600' :
            value >= 85 ? 'text-blue-600' :
            'text-yellow-600'
          }`}>
            {value}%
          </div>
        </div>
      )
    },
    {
      key: 'avgResolution',
      label: 'Purata Penyelesaian (hari)',
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
      label: 'Pematuhan (%)',
      render: (value) => (
        <div className="text-center professional-number">{value}%</div>
      )
    }
  ]

  const hotspotColumns = [
    {
      key: 'area',
      label: 'Kawasan',
      render: (value) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'violations',
      label: 'Pelanggaran',
      render: (value) => (
        <div className="text-center professional-number font-semibold">{value}</div>
      )
    },
    {
      key: 'type',
      label: 'Jenis Utama'
    },
    {
      key: 'risk',
      label: 'Tahap Risiko',
      render: (value) => (
        <span className={`badge ${
          value === 'Tinggi' ? 'badge-danger' :
          value === 'Sederhana' ? 'badge-warning' :
          'badge-success'
        }`}>
          {value}
        </span>
      )
    }
  ]

  return (
    <PageTemplate
      title="Papan Pemuka Penguatkuasaan"
      subtitle="Sistem pemantauan dan analisis aktiviti penguatkuasaan"
      icon={Shield}
      onRefresh={loadData}
      loading={loading}
      breadcrumbs={[
        { label: 'Papan Pemuka' },
        { label: 'Penguatkuasaan' }
      ]}
      actions={
        <div className="flex gap-3">
          <select
            value={selectedFilter}
            onChange={(e) => setSelectedFilter(e.target.value)}
            className="select"
          >
            <option value="all">Semua Kes</option>
            <option value="active">Kes Aktif</option>
            <option value="resolved">Kes Selesai</option>
            <option value="pending">Kes Tertunggak</option>
          </select>
          <select
            value={selectedPeriod}
            onChange={(e) => setSelectedPeriod(e.target.value)}
            className="select"
          >
            <option value="monthly">Bulanan</option>
            <option value="quarterly">Suku Tahun</option>
            <option value="yearly">Tahunan</option>
          </select>
        </div>
      }
    >
      {/* Primary KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Jumlah Kes"
          value={enforcementStats.totalCases}
          subtitle="Kes penguatkuasaan"
          icon={FileText}
          color="blue"
          trend="up"
          trendValue="+8.2%"
          loading={loading}
        />
        <StatsCard
          title="Kes Aktif"
          value={enforcementStats.activeCases}
          subtitle="Dalam pemprosesan"
          icon={Clock}
          color="yellow"
          loading={loading}
        />
        <StatsCard
          title="Kes Selesai"
          value={enforcementStats.resolvedCases}
          subtitle={`${((enforcementStats.resolvedCases / enforcementStats.totalCases) * 100).toFixed(1)}% kadar penyelesaian`}
          icon={CheckCircle}
          color="green"
          trend="up"
          trendValue="+5.1%"
          loading={loading}
        />
        <StatsCard
          title="Kes Tertunggak"
          value={enforcementStats.pendingCases}
          subtitle="Memerlukan tindakan"
          icon={AlertTriangle}
          color="red"
          trend="down"
          trendValue="-12.3%"
          loading={loading}
        />
      </div>

      {/* Secondary KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Pegawai Aktif"
          value={enforcementStats.activeOfficers}
          subtitle={`daripada ${enforcementStats.totalOfficers} pegawai`}
          icon={Users}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Pemeriksaan"
          value={enforcementStats.totalInspections}
          subtitle="Pemeriksaan dijalankan"
          icon={Search}
          color="purple"
          loading={loading}
        />
        <StatsCard
          title="Hasil Kutipan"
          value={`RM ${(enforcementStats.totalRevenue / 1000).toFixed(0)}K`}
          subtitle="Kutipan kompaun"
          icon={DollarSign}
          color="green"
          trend="up"
          trendValue="+15.7%"
          loading={loading}
        />
        <StatsCard
          title="Kadar Pematuhan"
          value={`${enforcementStats.complianceRate}%`}
          subtitle="Pematuhan peraturan"
          icon={Target}
          color="indigo"
          trend="up"
          trendValue="+2.8%"
          loading={loading}
        />
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Monthly Enforcement Trend */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Trend Penguatkuasaan Bulanan</h3>
            <p className="card-subtitle">Prestasi kes dan penyelesaian</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <ComposedChart data={chartData.monthly}>
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
                    name === 'cases' ? 'Kes Baru' :
                    name === 'resolved' ? 'Kes Selesai' :
                    name === 'pending' ? 'Kes Tertunggak' :
                    name === 'compliance' ? 'Pematuhan (%)' : name
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar dataKey="cases" fill="#3b82f6" name="cases" />
                <Bar dataKey="resolved" fill="#10b981" name="resolved" />
                <Line 
                  type="monotone" 
                  dataKey="compliance" 
                  stroke="#8b5cf6" 
                  strokeWidth={3}
                  dot={{ fill: '#8b5cf6', strokeWidth: 2, r: 4 }}
                />
              </ComposedChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Violation Types */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Jenis Pelanggaran</h3>
            <p className="card-subtitle">Taburan mengikut kategori</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData.violations} layout="horizontal">
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  type="number"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  type="category"
                  dataKey="type"
                  tick={{ fontSize: 11, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  width={120}
                />
                <Tooltip 
                  formatter={(value, name) => [value, 'Bilangan Kes']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar 
                  dataKey="count" 
                  radius={[0, 4, 4, 0]}
                >
                  {chartData.violations?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Revenue and Compliance Trend */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Hasil dan Pematuhan</h3>
            <p className="card-subtitle">Trend kutipan dan kadar pematuhan</p>
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
                  yAxisId="left"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`}
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
                    name === 'revenue' ? `RM ${(value / 1000).toFixed(0)}K` : `${value}%`,
                    name === 'revenue' ? 'Hasil' : 'Pematuhan'
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Area 
                  yAxisId="left"
                  type="monotone" 
                  dataKey="revenue" 
                  stroke="#10b981" 
                  fill="#10b981"
                  fillOpacity={0.6}
                />
                <Line 
                  yAxisId="right"
                  type="monotone" 
                  dataKey="compliance" 
                  stroke="#3b82f6" 
                  strokeWidth={3}
                  dot={{ fill: '#3b82f6', strokeWidth: 2, r: 4 }}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Violation Hotspots */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Kawasan Hotspot</h3>
            <p className="card-subtitle">Kawasan dengan pelanggaran tinggi</p>
          </div>
          <div className="card-content">
            <div className="space-y-3">
              {violationHotspots.map((hotspot, index) => (
                <div key={index} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                  <div>
                    <div className="font-medium text-slate-900">{hotspot.area}</div>
                    <div className="text-sm text-slate-500">{hotspot.type}</div>
                  </div>
                  <div className="text-right">
                    <div className="professional-number font-semibold text-slate-900">
                      {hotspot.violations}
                    </div>
                    <span className={`badge text-xs ${
                      hotspot.risk === 'Tinggi' ? 'badge-danger' :
                      hotspot.risk === 'Sederhana' ? 'badge-warning' :
                      'badge-success'
                    }`}>
                      {hotspot.risk}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Data Tables */}
      <div className="space-y-8">
        {/* Recent Cases */}
        <DataTable
          data={recentCases}
          columns={caseColumns}
          title="Kes Terkini"
          subtitle="Senarai kes penguatkuasaan terbaru"
          loading={loading}
          searchable={true}
          sortable={true}
          actions={false}
        />

        {/* Officer Performance */}
        <DataTable
          data={officerPerformance}
          columns={officerColumns}
          title="Prestasi Pegawai"
          subtitle="Analisis prestasi pegawai penguatkuasa"
          loading={loading}
          searchable={true}
          sortable={true}
          actions={false}
        />
      </div>
    </PageTemplate>
  )
}

export default Dashboard