import { useState, useEffect } from 'react'
import { 
  Building, 
  MapPin, 
  Users, 
  TrendingUp, 
  AlertTriangle, 
  CheckCircle,
  FileText,
  DollarSign,
  Calendar,
  Target,
  Activity,
  BarChart3
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
  RadarChart,
  PolarGrid,
  PolarAngleAxis,
  PolarRadiusAxis,
  Radar
} from 'recharts'
import PageTemplate from '../components/UI/PageTemplate'
import StatsCard from '../components/UI/StatsCard'
import DataTable from '../components/UI/DataTable'

const ZonMajlis = () => {
  const [loading, setLoading] = useState(true)
  const [selectedZone, setSelectedZone] = useState('all')
  const [selectedPeriod, setSelectedPeriod] = useState('monthly')
  const [zoneStats, setZoneStats] = useState({})
  const [chartData, setChartData] = useState({})
  const [zonePerformance, setZonePerformance] = useState([])
  const [zoneComparison, setZoneComparison] = useState([])

  // Mock comprehensive zone data
  const mockZoneStats = {
    totalZones: 12,
    activeZones: 11,
    totalPopulation: 1850000,
    totalArea: 243.65, // km²
    averageCompliance: 86.7,
    totalRevenue: 2450000,
    totalCases: 3456,
    resolvedCases: 2987
  }

  const mockZonePerformance = [
    {
      id: 1,
      zoneName: 'Zon Pusat Bandar',
      area: 15.2,
      population: 285000,
      cases: 456,
      resolved: 398,
      revenue: 456000,
      compliance: 87.3,
      officers: 8,
      inspections: 234,
      violations: 58,
      status: 'Aktif'
    },
    {
      id: 2,
      zoneName: 'Zon Bandar Utara',
      area: 22.8,
      population: 195000,
      cases: 312,
      resolved: 278,
      revenue: 312000,
      compliance: 89.1,
      officers: 6,
      inspections: 189,
      violations: 34,
      status: 'Aktif'
    },
    {
      id: 3,
      zoneName: 'Zon Bandar Selatan',
      area: 18.5,
      population: 167000,
      cases: 289,
      resolved: 245,
      revenue: 289000,
      compliance: 84.8,
      officers: 5,
      inspections: 156,
      violations: 44,
      status: 'Aktif'
    },
    {
      id: 4,
      zoneName: 'Zon Bandar Timur',
      area: 25.1,
      population: 203000,
      cases: 378,
      resolved: 334,
      revenue: 378000,
      compliance: 88.4,
      officers: 7,
      inspections: 201,
      violations: 44,
      status: 'Aktif'
    },
    {
      id: 5,
      zoneName: 'Zon Bandar Barat',
      area: 19.7,
      population: 178000,
      cases: 298,
      resolved: 267,
      revenue: 298000,
      compliance: 89.6,
      officers: 6,
      inspections: 167,
      violations: 31,
      status: 'Aktif'
    },
    {
      id: 6,
      zoneName: 'Zon Perindustrian',
      area: 35.4,
      population: 89000,
      cases: 567,
      resolved: 489,
      revenue: 567000,
      compliance: 86.2,
      officers: 9,
      inspections: 298,
      violations: 78,
      status: 'Aktif'
    },
    {
      id: 7,
      zoneName: 'Zon Perumahan A',
      area: 28.3,
      population: 234000,
      cases: 234,
      resolved: 212,
      revenue: 234000,
      compliance: 90.6,
      officers: 5,
      inspections: 134,
      violations: 22,
      status: 'Aktif'
    },
    {
      id: 8,
      zoneName: 'Zon Perumahan B',
      area: 31.2,
      population: 198000,
      cases: 198,
      resolved: 178,
      revenue: 198000,
      compliance: 89.9,
      officers: 4,
      inspections: 112,
      violations: 20,
      status: 'Aktif'
    },
    {
      id: 9,
      zoneName: 'Zon Komersial',
      area: 12.8,
      population: 145000,
      cases: 445,
      resolved: 378,
      revenue: 445000,
      compliance: 84.9,
      officers: 8,
      inspections: 234,
      violations: 67,
      status: 'Aktif'
    },
    {
      id: 10,
      zoneName: 'Zon Pelabuhan',
      area: 8.9,
      population: 67000,
      cases: 189,
      resolved: 167,
      revenue: 189000,
      compliance: 88.4,
      officers: 4,
      inspections: 98,
      violations: 22,
      status: 'Aktif'
    },
    {
      id: 11,
      zoneName: 'Zon Lapangan Terbang',
      area: 15.6,
      population: 45000,
      cases: 123,
      resolved: 109,
      revenue: 123000,
      compliance: 88.6,
      officers: 3,
      inspections: 67,
      violations: 14,
      status: 'Aktif'
    },
    {
      id: 12,
      zoneName: 'Zon Pembangunan',
      area: 10.2,
      population: 44000,
      cases: 67,
      resolved: 45,
      revenue: 67000,
      compliance: 67.2,
      officers: 2,
      inspections: 34,
      violations: 22,
      status: 'Dalam Pembangunan'
    }
  ]

  const mockMonthlyComparison = [
    { month: 'Jan', pusat: 89, utara: 87, selatan: 85, timur: 88, barat: 90 },
    { month: 'Feb', pusat: 88, utara: 89, selatan: 86, timur: 89, barat: 91 },
    { month: 'Mar', pusat: 87, utara: 90, selatan: 85, timur: 88, barat: 90 },
    { month: 'Apr', pusat: 88, utara: 88, selatan: 84, timur: 89, barat: 89 },
    { month: 'May', pusat: 87, utara: 89, selatan: 85, timur: 88, barat: 90 },
    { month: 'Jun', pusat: 87, utara: 89, selatan: 85, timur: 88, barat: 90 }
  ]

  const mockZoneTypes = [
    { type: 'Bandar', count: 5, percentage: 41.7, color: '#3B82F6' },
    { type: 'Perumahan', count: 2, percentage: 16.7, color: '#10B981' },
    { type: 'Perindustrian', count: 1, percentage: 8.3, color: '#F59E0B' },
    { type: 'Komersial', count: 1, percentage: 8.3, color: '#EF4444' },
    { type: 'Infrastruktur', count: 2, percentage: 16.7, color: '#8B5CF6' },
    { type: 'Pembangunan', count: 1, percentage: 8.3, color: '#6B7280' }
  ]

  const mockRadarData = [
    { subject: 'Pematuhan', A: 87, B: 89, fullMark: 100 },
    { subject: 'Hasil', A: 85, B: 88, fullMark: 100 },
    { subject: 'Pemeriksaan', A: 90, B: 85, fullMark: 100 },
    { subject: 'Penyelesaian', A: 88, B: 92, fullMark: 100 },
    { subject: 'Kebersihan', A: 86, B: 87, fullMark: 100 },
    { subject: 'Keselamatan', A: 89, B: 90, fullMark: 100 }
  ]

  useEffect(() => {
    loadData()
  }, [selectedZone, selectedPeriod])

  const loadData = async () => {
    setLoading(true)
    try {
      await new Promise(resolve => setTimeout(resolve, 1000))
      setZoneStats(mockZoneStats)
      setChartData({
        monthly: mockMonthlyComparison,
        types: mockZoneTypes,
        radar: mockRadarData
      })
      setZonePerformance(mockZonePerformance)
    } catch (error) {
      console.error('Error loading zone data:', error)
    } finally {
      setLoading(false)
    }
  }

  const zoneColumns = [
    {
      key: 'zoneName',
      label: 'Nama Zon',
      render: (value, zone) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{zone.area} km² • {zone.population.toLocaleString()} penduduk</div>
        </div>
      )
    },
    {
      key: 'cases',
      label: 'Kes',
      render: (value, zone) => (
        <div className="text-center">
          <div className="professional-number font-semibold">{value}</div>
          <div className="text-xs text-slate-500">{zone.resolved} selesai</div>
        </div>
      )
    },
    {
      key: 'compliance',
      label: 'Pematuhan (%)',
      render: (value) => (
        <div className="text-center">
          <div className={`professional-number font-semibold ${
            value >= 90 ? 'text-green-600' :
            value >= 85 ? 'text-blue-600' :
            value >= 80 ? 'text-yellow-600' : 'text-red-600'
          }`}>
            {value}%
          </div>
        </div>
      )
    },
    {
      key: 'revenue',
      label: 'Hasil (RM)',
      render: (value) => (
        <div className="text-right professional-number text-green-600">
          {(value / 1000).toFixed(0)}K
        </div>
      )
    },
    {
      key: 'officers',
      label: 'Pegawai',
      render: (value) => (
        <div className="text-center professional-number">{value}</div>
      )
    },
    {
      key: 'violations',
      label: 'Pelanggaran',
      render: (value) => (
        <div className="text-center">
          <span className={`professional-number ${
            value <= 20 ? 'text-green-600' :
            value <= 40 ? 'text-yellow-600' : 'text-red-600'
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
          value === 'Aktif' ? 'badge-success' : 'badge-warning'
        }`}>
          {value}
        </span>
      )
    }
  ]

  return (
    <PageTemplate
      title="Ringkasan Zon Ahli Majlis"
      subtitle="Analisis prestasi dan pemantauan zon-zon dalam kawasan majlis"
      icon={Building}
      onRefresh={loadData}
      loading={loading}
      breadcrumbs={[
        { label: 'Papan Pemuka' },
        { label: 'Ringkasan Zon Ahli Majlis' }
      ]}
      actions={
        <div className="flex gap-3">
          <select
            value={selectedZone}
            onChange={(e) => setSelectedZone(e.target.value)}
            className="select"
          >
            <option value="all">Semua Zon</option>
            <option value="bandar">Zon Bandar</option>
            <option value="perumahan">Zon Perumahan</option>
            <option value="perindustrian">Zon Perindustrian</option>
            <option value="komersial">Zon Komersial</option>
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
      {/* Zone Overview Statistics */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Jumlah Zon"
          value={zoneStats.totalZones}
          subtitle="Zon dalam kawasan majlis"
          icon={Building}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Zon Aktif"
          value={zoneStats.activeZones}
          subtitle="Zon beroperasi penuh"
          icon={CheckCircle}
          color="green"
          loading={loading}
        />
        <StatsCard
          title="Jumlah Penduduk"
          value={`${(zoneStats.totalPopulation / 1000000).toFixed(1)}M`}
          subtitle="Penduduk keseluruhan"
          icon={Users}
          color="purple"
          loading={loading}
        />
        <StatsCard
          title="Keluasan Kawasan"
          value={`${zoneStats.totalArea} km²`}
          subtitle="Kawasan pentadbiran"
          icon={MapPin}
          color="indigo"
          loading={loading}
        />
      </div>

      {/* Performance Metrics */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Purata Pematuhan"
          value={`${zoneStats.averageCompliance}%`}
          subtitle="Kadar pematuhan zon"
          icon={Target}
          color="green"
          trend="up"
          trendValue="+1.8%"
          loading={loading}
        />
        <StatsCard
          title="Hasil Kutipan"
          value={`RM ${(zoneStats.totalRevenue / 1000000).toFixed(1)}M`}
          subtitle="Kutipan semua zon"
          icon={DollarSign}
          color="green"
          trend="up"
          trendValue="+5.2%"
          loading={loading}
        />
        <StatsCard
          title="Jumlah Kes"
          value={zoneStats.totalCases}
          subtitle="Kes penguatkuasaan"
          icon={FileText}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Kes Selesai"
          value={zoneStats.resolvedCases}
          subtitle={`${((zoneStats.resolvedCases / zoneStats.totalCases) * 100).toFixed(1)}% kadar penyelesaian`}
          icon={CheckCircle}
          color="purple"
          trend="up"
          trendValue="+3.1%"
          loading={loading}
        />
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Zone Compliance Comparison */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Perbandingan Pematuhan Zon</h3>
            <p className="card-subtitle">Trend pematuhan 5 zon utama</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={chartData.monthly}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  dataKey="month" 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  domain={[80, 95]}
                  tickFormatter={(value) => `${value}%`}
                />
                <Tooltip 
                  formatter={(value, name) => [
                    `${value}%`,
                    name === 'pusat' ? 'Zon Pusat' :
                    name === 'utara' ? 'Zon Utara' :
                    name === 'selatan' ? 'Zon Selatan' :
                    name === 'timur' ? 'Zon Timur' : 'Zon Barat'
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Line type="monotone" dataKey="pusat" stroke="#3b82f6" strokeWidth={2} />
                <Line type="monotone" dataKey="utara" stroke="#10b981" strokeWidth={2} />
                <Line type="monotone" dataKey="selatan" stroke="#f59e0b" strokeWidth={2} />
                <Line type="monotone" dataKey="timur" stroke="#ef4444" strokeWidth={2} />
                <Line type="monotone" dataKey="barat" stroke="#8b5cf6" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Zone Types Distribution */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Taburan Jenis Zon</h3>
            <p className="card-subtitle">Kategori zon dalam kawasan majlis</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.types}
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  dataKey="percentage"
                  label={({ type, percentage }) => `${type} (${percentage}%)`}
                >
                  {chartData.types?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip 
                  formatter={(value, name, props) => [
                    `${value}% (${props.payload.count} zon)`,
                    'Peratusan'
                  ]}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Zone Performance Radar */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Analisis Prestasi Zon</h3>
            <p className="card-subtitle">Perbandingan prestasi mengikut metrik</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <RadarChart data={chartData.radar}>
                <PolarGrid stroke="#e2e8f0" />
                <PolarAngleAxis 
                  dataKey="subject" 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                />
                <PolarRadiusAxis 
                  angle={90} 
                  domain={[0, 100]}
                  tick={{ fontSize: 10, fill: '#64748b' }}
                />
                <Radar
                  name="Zon A"
                  dataKey="A"
                  stroke="#3b82f6"
                  fill="#3b82f6"
                  fillOpacity={0.3}
                  strokeWidth={2}
                />
                <Radar
                  name="Zon B"
                  dataKey="B"
                  stroke="#10b981"
                  fill="#10b981"
                  fillOpacity={0.3}
                  strokeWidth={2}
                />
                <Tooltip />
              </RadarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Zone Revenue Comparison */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Hasil Kutipan Mengikut Zon</h3>
            <p className="card-subtitle">Perbandingan hasil kutipan bulanan</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={zonePerformance.slice(0, 6)}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  dataKey="zoneName"
                  tick={{ fontSize: 10, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  angle={-45}
                  textAnchor="end"
                  height={80}
                />
                <YAxis 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`}
                />
                <Tooltip 
                  formatter={(value) => [`RM ${value.toLocaleString()}`, 'Hasil']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar 
                  dataKey="revenue" 
                  fill="#10b981"
                  radius={[4, 4, 0, 0]}
                />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Zone Performance Table */}
      <DataTable
        data={zonePerformance}
        columns={zoneColumns}
        title="Prestasi Terperinci Zon"
        subtitle="Maklumat lengkap prestasi setiap zon dalam kawasan majlis"
        loading={loading}
        searchable={true}
        sortable={true}
        actions={false}
      />
    </PageTemplate>
  )
}

export default ZonMajlis