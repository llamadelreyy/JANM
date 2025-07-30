import { useState, useEffect } from 'react'
import { 
  FileBarChart, 
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
  Package
} from 'lucide-react'
import {
  ResponsiveContainer,
  Tooltip,
  Brush,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  RadialBarChart,
  RadialBar,
  Legend,
  Cell,
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie
} from 'recharts'
import PageTemplate from '../components/UI/PageTemplate'
import StatsCard from '../components/UI/StatsCard'
import DataTable from '../components/UI/DataTable'
import { toast } from 'sonner'

const GraphConfiscation = () => {
  const [loading, setLoading] = useState(true)
  const [selectedPeriod, setSelectedPeriod] = useState('monthly')
  const [selectedCategory, setSelectedCategory] = useState('all')
  const [stats, setStats] = useState({})
  const [chartData, setChartData] = useState({})

  // Mock comprehensive confiscation data
  const mockStats = {
    totalConfiscations: 789,
    itemsConfiscated: 2456,
    totalValue: 1234500,
    returnedItems: 1567,
    destroyedItems: 456,
    pendingItems: 433,
    averageValue: 1564,
    recoveryRate: 63.8
  }

  // Radial bar chart data for confiscation categories
  const mockRadialData = [
    { category: 'Makanan Rosak', value: 35, count: 276, fill: '#EF4444' },
    { category: 'Barang Tiruan', value: 28, count: 221, fill: '#F59E0B' },
    { category: 'Lesen Tamat', value: 20, count: 158, fill: '#3B82F6' },
    { category: 'Barang Haram', value: 12, count: 95, fill: '#10B981' },
    { category: 'Lain-lain', value: 5, count: 39, fill: '#8B5CF6' }
  ]

  // Sankey-style flow data for confiscation process
  const mockFlowData = [
    { stage: 'Sitaan Dibuat', value: 789, next: 'Penilaian' },
    { stage: 'Penilaian', value: 789, next: 'Keputusan' },
    { stage: 'Dikembalikan', value: 315, next: 'Selesai' },
    { stage: 'Dimusnahkan', value: 237, next: 'Selesai' },
    { stage: 'Dijual Lelongan', value: 158, next: 'Selesai' },
    { stage: 'Dalam Proses', value: 79, next: 'Pending' }
  ]

  // Area chart with brush for detailed timeline
  const mockTimelineData = [
    { date: '2024-01-01', confiscations: 12, value: 45000, items: 67 },
    { date: '2024-01-02', confiscations: 8, value: 32000, items: 45 },
    { date: '2024-01-03', confiscations: 15, value: 67000, items: 89 },
    { date: '2024-01-04', confiscations: 6, value: 23000, items: 34 },
    { date: '2024-01-05', confiscations: 18, value: 78000, items: 123 },
    { date: '2024-01-06', confiscations: 11, value: 45000, items: 67 },
    { date: '2024-01-07', confiscations: 9, value: 38000, items: 56 },
    { date: '2024-01-08', confiscations: 14, value: 56000, items: 78 },
    { date: '2024-01-09', confiscations: 7, value: 29000, items: 43 },
    { date: '2024-01-10', confiscations: 16, value: 67000, items: 89 },
    { date: '2024-01-11', confiscations: 13, value: 52000, items: 76 },
    { date: '2024-01-12', confiscations: 10, value: 41000, items: 58 },
    { date: '2024-01-13', confiscations: 19, value: 82000, items: 134 },
    { date: '2024-01-14', confiscations: 5, value: 21000, items: 32 },
    { date: '2024-01-15', confiscations: 17, value: 73000, items: 98 }
  ]

  // Multi-line chart for trends
  const mockTrendData = [
    { month: 'Jan', confiscations: 89, returned: 56, destroyed: 23, sold: 10 },
    { month: 'Feb', confiscations: 76, returned: 48, destroyed: 18, sold: 10 },
    { month: 'Mar', confiscations: 95, returned: 62, destroyed: 21, sold: 12 },
    { month: 'Apr', confiscations: 67, returned: 43, destroyed: 15, sold: 9 },
    { month: 'May', confiscations: 112, returned: 71, destroyed: 28, sold: 13 },
    { month: 'Jun', confiscations: 84, returned: 54, destroyed: 19, sold: 11 }
  ]

  // Pie chart for item disposition
  const mockDispositionData = [
    { name: 'Dikembalikan', value: 1567, percentage: 63.8, color: '#10B981' },
    { name: 'Dimusnahkan', value: 456, percentage: 18.6, color: '#EF4444' },
    { name: 'Dijual Lelongan', value: 234, percentage: 9.5, color: '#F59E0B' },
    { name: 'Dalam Proses', value: 199, percentage: 8.1, color: '#6B7280' }
  ]

  const mockConfiscationData = [
    {
      id: 'CNF001234',
      itemType: 'Makanan Rosak',
      description: 'Daging ayam tidak segar',
      location: 'Pasar Borong Selayang',
      officer: 'Ahmad Bin Ali',
      confiscationDate: '2024-01-15',
      estimatedValue: 2500,
      quantity: '50 kg',
      status: 'Dimusnahkan',
      disposalDate: '2024-01-16'
    },
    {
      id: 'CNF001235',
      itemType: 'Barang Tiruan',
      description: 'Jam tangan tiruan jenama terkenal',
      location: 'Pasar Malam Jalan TAR',
      officer: 'Siti Aminah',
      confiscationDate: '2024-01-14',
      estimatedValue: 15000,
      quantity: '25 unit',
      status: 'Dalam Proses',
      disposalDate: null
    },
    {
      id: 'CNF001236',
      itemType: 'Lesen Tamat',
      description: 'Barang jualan tanpa lesen sah',
      location: 'Kedai Runcit Jalan Pudu',
      officer: 'Mohd Farid',
      confiscationDate: '2024-01-13',
      estimatedValue: 3200,
      quantity: '100 item',
      status: 'Dikembalikan',
      disposalDate: '2024-01-20'
    }
  ]

  useEffect(() => {
    loadData()
  }, [selectedPeriod, selectedCategory])

  const loadData = async () => {
    setLoading(true)
    try {
      await new Promise(resolve => setTimeout(resolve, 1000))
      setStats(mockStats)
      setChartData({
        radial: mockRadialData,
        timeline: mockTimelineData,
        trends: mockTrendData,
        disposition: mockDispositionData
      })
    } catch (error) {
      console.error('Error loading confiscation data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleExport = () => {
    toast.success('Graf sitaan sedang dieksport...')
  }

  const confiscationColumns = [
    {
      key: 'id',
      label: 'ID Sitaan',
      render: (value) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'itemType',
      label: 'Jenis Barang',
      render: (value, item) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{item.description}</div>
        </div>
      )
    },
    {
      key: 'location',
      label: 'Lokasi Sitaan'
    },
    {
      key: 'officer',
      label: 'Pegawai'
    },
    {
      key: 'quantity',
      label: 'Kuantiti',
      render: (value) => (
        <div className="text-center font-medium">{value}</div>
      )
    },
    {
      key: 'estimatedValue',
      label: 'Nilai Anggaran (RM)',
      render: (value) => (
        <div className="text-right professional-number">
          {value.toLocaleString('ms-MY')}
        </div>
      )
    },
    {
      key: 'status',
      label: 'Status',
      render: (value) => (
        <span className={`badge ${
          value === 'Dikembalikan' ? 'badge-success' :
          value === 'Dimusnahkan' ? 'badge-danger' :
          value === 'Dijual Lelongan' ? 'badge-warning' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'confiscationDate',
      label: 'Tarikh Sitaan',
      render: (value) => (
        <div className="text-sm text-slate-600">
          {new Date(value).toLocaleDateString('ms-MY')}
        </div>
      )
    }
  ]

  return (
    <PageTemplate
      title="Graf Sitaan"
      subtitle="Analisis visual sitaan barang dan tindakan susulan"
      icon={FileBarChart}
      onRefresh={loadData}
      loading={loading}
      breadcrumbs={[
        { label: 'Graf Statistik' },
        { label: 'Graf Sitaan' }
      ]}
      actions={
        <div className="flex gap-3">
          <select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            className="select"
          >
            <option value="all">Semua Kategori</option>
            <option value="food">Makanan</option>
            <option value="counterfeit">Barang Tiruan</option>
            <option value="expired">Lesen Tamat</option>
            <option value="illegal">Barang Haram</option>
          </select>
          <select
            value={selectedPeriod}
            onChange={(e) => setSelectedPeriod(e.target.value)}
            className="select"
          >
            <option value="daily">Harian</option>
            <option value="monthly">Bulanan</option>
            <option value="quarterly">Suku Tahun</option>
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
          title="Jumlah Sitaan"
          value={stats.totalConfiscations}
          subtitle="Kes sitaan dijalankan"
          icon={FileBarChart}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Item Disita"
          value={stats.itemsConfiscated}
          subtitle="Jumlah barang disita"
          icon={Package}
          color="purple"
          loading={loading}
        />
        <StatsCard
          title="Nilai Anggaran"
          value={`RM ${(stats.totalValue / 1000).toFixed(0)}K`}
          subtitle="Nilai keseluruhan"
          icon={Target}
          color="green"
          trend="up"
          trendValue="+15.3%"
          loading={loading}
        />
        <StatsCard
          title="Kadar Pemulihan"
          value={`${stats.recoveryRate}%`}
          subtitle="Barang dikembalikan"
          icon={CheckCircle}
          color="indigo"
          trend="up"
          trendValue="+3.7%"
          loading={loading}
        />
      </div>

      {/* Advanced Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Radial Bar Chart - Categories */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Kategori Sitaan</h3>
            <p className="card-subtitle">Taburan radial mengikut jenis</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <RadialBarChart 
                cx="50%" 
                cy="50%" 
                innerRadius="20%" 
                outerRadius="80%" 
                data={chartData.radial}
              >
                <RadialBar 
                  dataKey="value" 
                  cornerRadius={10} 
                  label={{ position: 'insideStart', fill: '#fff' }}
                >
                  {chartData.radial?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.fill} />
                  ))}
                </RadialBar>
                <Legend 
                  iconSize={10} 
                  layout="horizontal" 
                  verticalAlign="bottom" 
                  align="center"
                />
                <Tooltip 
                  formatter={(value, name) => [`${value}% (${chartData.radial?.find(d => d.value === value)?.count || 0} kes)`, 'Peratusan']}
                />
              </RadialBarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Disposition Pie Chart */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Pelupusan Barang Sitaan</h3>
            <p className="card-subtitle">Status tindakan terhadap barang</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.disposition}
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  dataKey="value"
                  label={({ name, percentage }) => `${name} (${percentage}%)`}
                >
                  {chartData.disposition?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip 
                  formatter={(value, name, props) => [
                    `${value} item (${props.payload.percentage}%)`,
                    'Bilangan'
                  ]}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Multi-line Trend Chart */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Trend Tindakan Bulanan</h3>
            <p className="card-subtitle">Perbandingan tindakan terhadap sitaan</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={chartData.trends}>
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
                    name === 'confiscations' ? 'Sitaan' :
                    name === 'returned' ? 'Dikembalikan' :
                    name === 'destroyed' ? 'Dimusnahkan' :
                    'Dijual'
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Line 
                  type="monotone" 
                  dataKey="confiscations" 
                  stroke="#3b82f6" 
                  strokeWidth={3}
                  dot={{ fill: '#3b82f6', strokeWidth: 2, r: 4 }}
                />
                <Line 
                  type="monotone" 
                  dataKey="returned" 
                  stroke="#10b981" 
                  strokeWidth={2}
                  dot={{ fill: '#10b981', strokeWidth: 2, r: 3 }}
                />
                <Line 
                  type="monotone" 
                  dataKey="destroyed" 
                  stroke="#ef4444" 
                  strokeWidth={2}
                  dot={{ fill: '#ef4444', strokeWidth: 2, r: 3 }}
                />
                <Line 
                  type="monotone" 
                  dataKey="sold" 
                  stroke="#f59e0b" 
                  strokeWidth={2}
                  dot={{ fill: '#f59e0b', strokeWidth: 2, r: 3 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Value Distribution Bar Chart */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Nilai Sitaan Mengikut Kategori</h3>
            <p className="card-subtitle">Analisis nilai anggaran barang</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData.radial}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  dataKey="category"
                  tick={{ fontSize: 11, fill: '#64748b' }}
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
                  formatter={(value, name) => [`${value} kes`, 'Bilangan']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar 
                  dataKey="count" 
                  radius={[4, 4, 0, 0]}
                >
                  {chartData.radial?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.fill} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Timeline Chart with Brush */}
      <div className="professional-card mb-8">
        <div className="card-header">
          <h3 className="card-title">Timeline Sitaan Harian</h3>
          <p className="card-subtitle">Analisis terperinci dengan brush selection</p>
        </div>
        <div className="card-content">
          <ResponsiveContainer width="100%" height={400}>
            <AreaChart data={chartData.timeline}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis 
                dataKey="date"
                tick={{ fontSize: 12, fill: '#64748b' }}
                axisLine={{ stroke: '#e2e8f0' }}
                tickFormatter={(value) => new Date(value).toLocaleDateString('ms-MY', { month: 'short', day: 'numeric' })}
              />
              <YAxis 
                tick={{ fontSize: 12, fill: '#64748b' }}
                axisLine={{ stroke: '#e2e8f0' }}
              />
              <Tooltip 
                formatter={(value, name) => [
                  name === 'value' ? `RM ${value.toLocaleString()}` : value,
                  name === 'confiscations' ? 'Sitaan' :
                  name === 'value' ? 'Nilai' : 'Item'
                ]}
                labelFormatter={(value) => new Date(value).toLocaleDateString('ms-MY')}
                labelStyle={{ color: '#1e293b' }}
              />
              <Area 
                type="monotone" 
                dataKey="confiscations" 
                stroke="#8b5cf6" 
                fill="#8b5cf6"
                fillOpacity={0.6}
              />
              <Brush 
                dataKey="date" 
                height={30} 
                stroke="#8b5cf6"
                tickFormatter={(value) => new Date(value).toLocaleDateString('ms-MY', { month: 'short', day: 'numeric' })}
              />
            </AreaChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Confiscation Data Table */}
      <DataTable
        data={mockConfiscationData}
        columns={confiscationColumns}
        title="Rekod Sitaan Terkini"
        subtitle="Senarai sitaan yang telah dijalankan"
        loading={loading}
        searchable={true}
        sortable={true}
        actions={false}
      />
    </PageTemplate>
  )
}

export default GraphConfiscation