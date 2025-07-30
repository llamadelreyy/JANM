import { useState, useEffect } from 'react'
import { 
  ClipboardList, 
  TrendingUp, 
  Calendar, 
  Users, 
  MapPin,
  Building,
  AlertTriangle,
  CheckCircle,
  Clock,
  Target,
  Download,
  Filter
} from 'lucide-react'
import {
  RadarChart,
  PolarGrid,
  PolarAngleAxis,
  PolarRadiusAxis,
  Radar,
  ResponsiveContainer,
  ScatterChart,
  Scatter,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  BarChart,
  Cell,
  PieChart,
  Pie,
  LabelList,
  ComposedChart,
  Bar,
  Line,
  Legend
} from 'recharts'
import PageTemplate from '../components/UI/PageTemplate'
import StatsCard from '../components/UI/StatsCard'
import DataTable from '../components/UI/DataTable'
import { toast } from 'sonner'

const GraphInspection = () => {
  const [loading, setLoading] = useState(true)
  const [selectedPeriod, setSelectedPeriod] = useState('monthly')
  const [selectedType, setSelectedType] = useState('all')
  const [stats, setStats] = useState({})
  const [chartData, setChartData] = useState({})

  // Mock comprehensive inspection data
  const mockStats = {
    totalInspections: 1234,
    completedInspections: 1089,
    pendingInspections: 145,
    violationsFound: 456,
    complianceRate: 88.3,
    averageScore: 7.8,
    criticalViolations: 67,
    minorViolations: 389
  }

  // Radar chart data for inspection categories
  const mockRadarData = [
    { category: 'Kebersihan', score: 85, maxScore: 100, violations: 45 },
    { category: 'Keselamatan', score: 92, maxScore: 100, violations: 23 },
    { category: 'Struktur', score: 78, maxScore: 100, violations: 67 },
    { category: 'Lesen', score: 88, maxScore: 100, violations: 34 },
    { category: 'Alam Sekitar', score: 82, maxScore: 100, violations: 56 },
    { category: 'Kesihatan', score: 90, maxScore: 100, violations: 28 }
  ]

  // Scatter plot data for inspection score vs violations
  const mockScatterData = [
    { score: 9.2, violations: 2, inspections: 45, zone: 'Zon A' },
    { score: 8.5, violations: 5, inspections: 67, zone: 'Zon B' },
    { score: 7.8, violations: 8, inspections: 89, zone: 'Zon C' },
    { score: 8.9, violations: 3, inspections: 56, zone: 'Zon D' },
    { score: 7.2, violations: 12, inspections: 78, zone: 'Zon E' },
    { score: 8.1, violations: 7, inspections: 92, zone: 'Zon F' },
    { score: 9.0, violations: 4, inspections: 34, zone: 'Zon G' },
    { score: 6.8, violations: 15, inspections: 123, zone: 'Zon H' },
    { score: 8.7, violations: 6, inspections: 45, zone: 'Zon I' },
    { score: 7.5, violations: 9, inspections: 67, zone: 'Zon J' }
  ]

  // Bar chart data for violation distribution
  const mockViolationBarData = [
    { name: 'Kebersihan Premis', violations: 156, fill: '#EF4444' },
    { name: 'Keselamatan Struktur', violations: 89, fill: '#F59E0B' },
    { name: 'Lesen Tidak Sah', violations: 67, fill: '#3B82F6' },
    { name: 'Pencemaran Alam', violations: 78, fill: '#10B981' },
    { name: 'Kesihatan Makanan', violations: 45, fill: '#8B5CF6' },
    { name: 'Parkir Tidak Sah', violations: 34, fill: '#6B7280' }
  ]

  // Pie chart data for inspection process
  const mockProcessPieData = [
    { name: 'Permohonan Pemeriksaan', value: 1500, fill: '#3B82F6' },
    { name: 'Pemeriksaan Dijadualkan', value: 1234, fill: '#10B981' },
    { name: 'Pemeriksaan Selesai', value: 1089, fill: '#F59E0B' },
    { name: 'Laporan Dikeluarkan', value: 987, fill: '#EF4444' },
    { name: 'Tindakan Susulan', value: 456, fill: '#8B5CF6' }
  ]

  // Monthly trend with multiple metrics
  const mockMonthlyTrend = [
    { month: 'Jan', inspections: 98, violations: 45, score: 8.2, compliance: 87.5 },
    { month: 'Feb', inspections: 102, violations: 38, score: 8.5, compliance: 89.1 },
    { month: 'Mar', inspections: 115, violations: 52, score: 7.8, compliance: 85.3 },
    { month: 'Apr', inspections: 89, violations: 34, score: 8.7, compliance: 90.2 },
    { month: 'May', inspections: 125, violations: 48, score: 8.1, compliance: 88.7 },
    { month: 'Jun', inspections: 108, violations: 41, score: 8.4, compliance: 89.5 }
  ]

  const mockInspectionData = [
    {
      id: 'INS001234',
      premiseName: 'Restoran Seri Wangi',
      address: 'No. 123, Jalan Merdeka, KL',
      inspectionDate: '2024-01-15',
      inspector: 'Ahmad Bin Ali',
      score: 8.5,
      violations: 3,
      status: 'Selesai',
      category: 'Makanan',
      nextInspection: '2024-07-15'
    },
    {
      id: 'INS001235',
      premiseName: 'Kedai Runcit Pak Man',
      address: 'No. 456, Jalan Bangsar, KL',
      inspectionDate: '2024-01-14',
      inspector: 'Siti Aminah',
      score: 7.2,
      violations: 8,
      status: 'Tindakan Diperlukan',
      category: 'Runcit',
      nextInspection: '2024-04-14'
    },
    {
      id: 'INS001236',
      premiseName: 'Pejabat Korporat ABC',
      address: 'No. 789, Jalan Pudu, KL',
      inspectionDate: '2024-01-13',
      inspector: 'Mohd Farid',
      score: 9.1,
      violations: 1,
      status: 'Lulus',
      category: 'Pejabat',
      nextInspection: '2025-01-13'
    }
  ]

  useEffect(() => {
    loadData()
  }, [selectedPeriod, selectedType])

  const loadData = async () => {
    setLoading(true)
    try {
      await new Promise(resolve => setTimeout(resolve, 1000))
      setStats(mockStats)
      setChartData({
        radar: mockRadarData,
        scatter: mockScatterData,
        violationBar: mockViolationBarData,
        processPie: mockProcessPieData,
        monthly: mockMonthlyTrend
      })
    } catch (error) {
      console.error('Error loading inspection data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleExport = () => {
    toast.success('Graf pemeriksaan sedang dieksport...')
  }

  const inspectionColumns = [
    {
      key: 'id',
      label: 'ID Pemeriksaan',
      render: (value) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'premiseName',
      label: 'Nama Premis',
      render: (value, item) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{item.address}</div>
        </div>
      )
    },
    {
      key: 'inspector',
      label: 'Pemeriksa'
    },
    {
      key: 'score',
      label: 'Skor',
      render: (value) => (
        <div className="text-center">
          <span className={`professional-number font-semibold ${
            value >= 8.5 ? 'text-green-600' :
            value >= 7.0 ? 'text-yellow-600' :
            'text-red-600'
          }`}>
            {value}/10
          </span>
        </div>
      )
    },
    {
      key: 'violations',
      label: 'Pelanggaran',
      render: (value) => (
        <div className="text-center">
          <span className={`professional-number ${
            value === 0 ? 'text-green-600' :
            value <= 3 ? 'text-yellow-600' :
            'text-red-600'
          }`}>
            {value}
          </span>
        </div>
      )
    },
    {
      key: 'status',
      label: 'Status',
      render: (value) => (
        <span className={`badge ${
          value === 'Lulus' ? 'badge-success' :
          value === 'Selesai' ? 'badge-primary' :
          value === 'Tindakan Diperlukan' ? 'badge-warning' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'nextInspection',
      label: 'Pemeriksaan Seterusnya',
      render: (value) => (
        <div className="text-sm text-slate-600">
          {new Date(value).toLocaleDateString('ms-MY')}
        </div>
      )
    }
  ]

  return (
    <PageTemplate
      title="Graf Nota Pemeriksaan"
      subtitle="Analisis visual prestasi dan trend pemeriksaan premis"
      icon={ClipboardList}
      onRefresh={loadData}
      loading={loading}
      breadcrumbs={[
        { label: 'Graf Statistik' },
        { label: 'Graf Nota Pemeriksaan' }
      ]}
      actions={
        <div className="flex gap-3">
          <select
            value={selectedType}
            onChange={(e) => setSelectedType(e.target.value)}
            className="select"
          >
            <option value="all">Semua Jenis</option>
            <option value="food">Makanan</option>
            <option value="retail">Runcit</option>
            <option value="office">Pejabat</option>
            <option value="industrial">Industri</option>
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
          <button onClick={handleExport} className="btn-primary">
            <Download className="h-4 w-4 mr-2" />
            Eksport
          </button>
        </div>
      }
    >
      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Jumlah Pemeriksaan"
          value={stats.totalInspections}
          subtitle="Pemeriksaan dijalankan"
          icon={ClipboardList}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Pemeriksaan Selesai"
          value={stats.completedInspections}
          subtitle={`${((stats.completedInspections / stats.totalInspections) * 100).toFixed(1)}% kadar siap`}
          icon={CheckCircle}
          color="green"
          trend="up"
          trendValue="+5.2%"
          loading={loading}
        />
        <StatsCard
          title="Purata Skor"
          value={`${stats.averageScore}/10`}
          subtitle="Skor pematuhan"
          icon={Target}
          color="purple"
          trend="up"
          trendValue="+0.3"
          loading={loading}
        />
        <StatsCard
          title="Pelanggaran Dijumpai"
          value={stats.violationsFound}
          subtitle="Memerlukan tindakan"
          icon={AlertTriangle}
          color="red"
          trend="down"
          trendValue="-8.7%"
          loading={loading}
        />
      </div>

      {/* Advanced Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Radar Chart - Inspection Categories Performance */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Prestasi Mengikut Kategori</h3>
            <p className="card-subtitle">Analisis radar skor pemeriksaan</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <RadarChart data={chartData.radar}>
                <PolarGrid stroke="#e2e8f0" />
                <PolarAngleAxis 
                  dataKey="category" 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                />
                <PolarRadiusAxis 
                  angle={90} 
                  domain={[0, 100]}
                  tick={{ fontSize: 10, fill: '#64748b' }}
                />
                <Radar
                  name="Skor"
                  dataKey="score"
                  stroke="#3b82f6"
                  fill="#3b82f6"
                  fillOpacity={0.3}
                  strokeWidth={2}
                />
                <Tooltip 
                  formatter={(value, name) => [`${value}%`, 'Skor Pematuhan']}
                />
              </RadarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Scatter Plot - Score vs Violations */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Korelasi Skor vs Pelanggaran</h3>
            <p className="card-subtitle">Analisis scatter plot mengikut zon</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <ScatterChart data={chartData.scatter}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  type="number" 
                  dataKey="score" 
                  name="Skor"
                  domain={[6, 10]}
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  type="number" 
                  dataKey="violations" 
                  name="Pelanggaran"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <Tooltip 
                  cursor={{ strokeDasharray: '3 3' }}
                  formatter={(value, name) => [
                    value,
                    name === 'score' ? 'Skor' : 'Pelanggaran'
                  ]}
                />
                <Scatter 
                  dataKey="inspections" 
                  fill="#8b5cf6"
                  stroke="#7c3aed"
                  strokeWidth={2}
                />
              </ScatterChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Bar Chart - Violation Distribution */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Taburan Jenis Pelanggaran</h3>
            <p className="card-subtitle">Carta bar visualisasi pelanggaran</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart
                data={chartData.violationBar}
                margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis
                  dataKey="name"
                  tick={{ fontSize: 10, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  angle={-45}
                  textAnchor="end"
                  height={80}
                />
                <YAxis
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <Tooltip
                  formatter={(value, name) => [`${value} kes`, 'Pelanggaran']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar dataKey="violations" radius={[4, 4, 0, 0]}>
                  {chartData.violationBar?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.fill} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Pie Chart - Inspection Process */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Proses Pemeriksaan</h3>
            <p className="card-subtitle">Carta pai aliran kerja</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.processPie}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {chartData.processPie?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.fill} />
                  ))}
                </Pie>
                <Tooltip
                  formatter={(value, name) => [`${value} kes`, 'Bilangan']}
                />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Monthly Trend Analysis */}
      <div className="professional-card mb-8">
        <div className="card-header">
          <h3 className="card-title">Trend Bulanan Pemeriksaan</h3>
          <p className="card-subtitle">Analisis prestasi dan skor pematuhan</p>
        </div>
        <div className="card-content">
          <ResponsiveContainer width="100%" height={400}>
            <ComposedChart data={chartData.monthly}>
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
              />
              <YAxis 
                yAxisId="right"
                orientation="right"
                tick={{ fontSize: 12, fill: '#64748b' }}
                axisLine={{ stroke: '#e2e8f0' }}
                domain={[6, 10]}
              />
              <Tooltip 
                formatter={(value, name) => [
                  name === 'score' ? `${value}/10` : value,
                  name === 'inspections' ? 'Pemeriksaan' :
                  name === 'violations' ? 'Pelanggaran' :
                  name === 'score' ? 'Purata Skor' :
                  'Pematuhan (%)'
                ]}
                labelStyle={{ color: '#1e293b' }}
              />
              <Bar yAxisId="left" dataKey="inspections" fill="#3b82f6" name="inspections" />
              <Bar yAxisId="left" dataKey="violations" fill="#ef4444" name="violations" />
              <Line 
                yAxisId="right"
                type="monotone" 
                dataKey="score" 
                stroke="#10b981" 
                strokeWidth={3}
                dot={{ fill: '#10b981', strokeWidth: 2, r: 4 }}
                name="score"
              />
            </ComposedChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Inspection Data Table */}
      <DataTable
        data={mockInspectionData}
        columns={inspectionColumns}
        title="Rekod Pemeriksaan Terkini"
        subtitle="Senarai pemeriksaan yang telah dijalankan"
        loading={loading}
        searchable={true}
        sortable={true}
        actions={false}
      />
    </PageTemplate>
  )
}

export default GraphInspection