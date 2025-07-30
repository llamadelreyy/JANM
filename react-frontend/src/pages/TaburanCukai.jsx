import { useState, useEffect } from 'react'
import { FileSpreadsheet, TrendingUp, MapPin, Calendar, Download } from 'lucide-react'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, LineChart, Line } from 'recharts'
import PageTemplate from '../components/UI/PageTemplate'
import StatsCard from '../components/UI/StatsCard'
import DataTable from '../components/UI/DataTable'
import { toast } from 'sonner'

const TaburanCukai = () => {
  const [loading, setLoading] = useState(true)
  const [selectedYear, setSelectedYear] = useState('2024')
  const [selectedDistrict, setSelectedDistrict] = useState('all')
  const [stats, setStats] = useState({
    totalAssessment: 0,
    totalCollection: 0,
    collectionRate: 0,
    outstandingAmount: 0
  })
  const [chartData, setChartData] = useState({
    monthly: [],
    byDistrict: [],
    byCategory: [],
    trend: []
  })
  const [tableData, setTableData] = useState([])

  // Mock data
  const mockStats = {
    totalAssessment: 125000000,
    totalCollection: 98750000,
    collectionRate: 79.0,
    outstandingAmount: 26250000
  }

  const mockMonthlyData = [
    { month: 'Jan', assessment: 10500000, collection: 8200000, rate: 78.1 },
    { month: 'Feb', assessment: 10200000, collection: 8100000, rate: 79.4 },
    { month: 'Mar', assessment: 10800000, collection: 8500000, rate: 78.7 },
    { month: 'Apr', assessment: 10300000, collection: 8200000, rate: 79.6 },
    { month: 'May', assessment: 10600000, collection: 8400000, rate: 79.2 },
    { month: 'Jun', assessment: 10400000, collection: 8300000, rate: 79.8 },
    { month: 'Jul', assessment: 10700000, collection: 8500000, rate: 79.4 },
    { month: 'Aug', assessment: 10500000, collection: 8300000, rate: 79.0 },
    { month: 'Sep', assessment: 10300000, collection: 8200000, rate: 79.6 },
    { month: 'Oct', assessment: 10600000, collection: 8400000, rate: 79.2 },
    { month: 'Nov', assessment: 10400000, collection: 8250000, rate: 79.3 },
    { month: 'Dec', assessment: 10700000, collection: 8400000, rate: 78.5 }
  ]

  const mockDistrictData = [
    { district: 'Kuala Lumpur', assessment: 45000000, collection: 36000000, rate: 80.0, properties: 15000 },
    { district: 'Petaling Jaya', assessment: 35000000, collection: 27500000, rate: 78.6, properties: 12000 },
    { district: 'Shah Alam', assessment: 25000000, collection: 19500000, rate: 78.0, properties: 8500 },
    { district: 'Subang Jaya', assessment: 20000000, collection: 15750000, rate: 78.8, properties: 7000 }
  ]

  const mockCategoryData = [
    { category: 'Kediaman', value: 45, amount: 56250000, color: '#3B82F6' },
    { category: 'Komersial', value: 30, amount: 37500000, color: '#10B981' },
    { category: 'Industri', value: 15, amount: 18750000, color: '#F59E0B' },
    { category: 'Pertanian', value: 10, amount: 12500000, color: '#EF4444' }
  ]

  const mockTrendData = [
    { year: '2020', assessment: 95000000, collection: 72000000, rate: 75.8 },
    { year: '2021', assessment: 105000000, collection: 81000000, rate: 77.1 },
    { year: '2022', assessment: 115000000, collection: 89000000, rate: 77.4 },
    { year: '2023', assessment: 120000000, collection: 94000000, rate: 78.3 },
    { year: '2024', assessment: 125000000, collection: 98750000, rate: 79.0 }
  ]

  const mockTableData = [
    {
      id: 1,
      propertyId: 'PBT001234',
      owner: 'Ahmad Bin Ali',
      address: 'No. 123, Jalan Merdeka, KL',
      category: 'Kediaman',
      assessment: 2500,
      paid: 2500,
      outstanding: 0,
      status: 'Selesai'
    },
    {
      id: 2,
      propertyId: 'PBT001235',
      owner: 'Siti Aminah Binti Hassan',
      address: 'No. 456, Jalan Bangsar, KL',
      category: 'Kediaman',
      assessment: 3200,
      paid: 1600,
      outstanding: 1600,
      status: 'Tertunggak'
    },
    {
      id: 3,
      propertyId: 'PBT001236',
      owner: 'Lim Ah Chong',
      address: 'No. 789, Jalan Pudu, KL',
      category: 'Komersial',
      assessment: 8500,
      paid: 8500,
      outstanding: 0,
      status: 'Selesai'
    },
    {
      id: 4,
      propertyId: 'PBT001237',
      owner: 'Rajesh Kumar',
      address: 'No. 321, Jalan Brickfields, KL',
      category: 'Komersial',
      assessment: 12000,
      paid: 6000,
      outstanding: 6000,
      status: 'Tertunggak'
    },
    {
      id: 5,
      propertyId: 'PBT001238',
      owner: 'Fatimah Binti Abdullah',
      address: 'No. 654, Jalan Cheras, KL',
      category: 'Kediaman',
      assessment: 1800,
      paid: 1800,
      outstanding: 0,
      status: 'Selesai'
    }
  ]

  useEffect(() => {
    loadData()
  }, [selectedYear, selectedDistrict])

  const loadData = async () => {
    setLoading(true)
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      setStats(mockStats)
      setChartData({
        monthly: mockMonthlyData,
        byDistrict: mockDistrictData,
        byCategory: mockCategoryData,
        trend: mockTrendData
      })
      setTableData(mockTableData)
    } catch (error) {
      toast.error('Gagal memuat data taksiran')
    } finally {
      setLoading(false)
    }
  }

  const handleExport = () => {
    toast.success('Data sedang dieksport...')
  }

  const tableColumns = [
    {
      key: 'propertyId',
      label: 'ID Harta',
      render: (value) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'owner',
      label: 'Pemilik',
      render: (value, item) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{item.address}</div>
        </div>
      )
    },
    {
      key: 'category',
      label: 'Kategori',
      render: (value) => (
        <span className={`badge ${
          value === 'Kediaman' ? 'badge-primary' :
          value === 'Komersial' ? 'badge-success' :
          value === 'Industri' ? 'badge-warning' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'assessment',
      label: 'Taksiran (RM)',
      render: (value) => (
        <div className="text-right professional-number">
          {value.toLocaleString('ms-MY')}
        </div>
      )
    },
    {
      key: 'paid',
      label: 'Dibayar (RM)',
      render: (value) => (
        <div className="text-right professional-number text-green-600">
          {value.toLocaleString('ms-MY')}
        </div>
      )
    },
    {
      key: 'outstanding',
      label: 'Tertunggak (RM)',
      render: (value) => (
        <div className="text-right professional-number text-red-600">
          {value.toLocaleString('ms-MY')}
        </div>
      )
    },
    {
      key: 'status',
      label: 'Status',
      render: (value) => (
        <span className={`badge ${
          value === 'Selesai' ? 'badge-success' : 'badge-warning'
        }`}>
          {value}
        </span>
      )
    }
  ]

  return (
    <PageTemplate
      title="Taburan Taksiran"
      subtitle="Analisis dan pemantauan kutipan taksiran harta"
      icon={FileSpreadsheet}
      onRefresh={loadData}
      loading={loading}
      breadcrumbs={[
        { label: 'Taburan Data' },
        { label: 'Taksiran' }
      ]}
      actions={
        <div className="flex gap-3">
          <select
            value={selectedYear}
            onChange={(e) => setSelectedYear(e.target.value)}
            className="select"
          >
            <option value="2024">2024</option>
            <option value="2023">2023</option>
            <option value="2022">2022</option>
          </select>
          <select
            value={selectedDistrict}
            onChange={(e) => setSelectedDistrict(e.target.value)}
            className="select"
          >
            <option value="all">Semua Daerah</option>
            <option value="kl">Kuala Lumpur</option>
            <option value="pj">Petaling Jaya</option>
            <option value="sa">Shah Alam</option>
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
          title="Jumlah Taksiran"
          value={`RM ${(stats.totalAssessment / 1000000).toFixed(1)}M`}
          subtitle="Taksiran keseluruhan"
          icon={FileSpreadsheet}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Jumlah Kutipan"
          value={`RM ${(stats.totalCollection / 1000000).toFixed(1)}M`}
          subtitle="Kutipan terkumpul"
          icon={TrendingUp}
          color="green"
          loading={loading}
        />
        <StatsCard
          title="Kadar Kutipan"
          value={`${stats.collectionRate}%`}
          subtitle="Peratusan kutipan"
          icon={Calendar}
          color="purple"
          trend="up"
          trendValue="+2.1%"
          loading={loading}
        />
        <StatsCard
          title="Tertunggak"
          value={`RM ${(stats.outstandingAmount / 1000000).toFixed(1)}M`}
          subtitle="Belum dikutip"
          icon={MapPin}
          color="red"
          loading={loading}
        />
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Monthly Collection Chart */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Kutipan Bulanan {selectedYear}</h3>
            <p className="card-subtitle">Perbandingan taksiran dan kutipan</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData.monthly}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  dataKey="month" 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                />
                <YAxis 
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  tickFormatter={(value) => `${(value / 1000000).toFixed(0)}M`}
                />
                <Tooltip 
                  formatter={(value, name) => [
                    `RM ${(value / 1000000).toFixed(1)}M`,
                    name === 'assessment' ? 'Taksiran' : 'Kutipan'
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar dataKey="assessment" fill="#94a3b8" name="assessment" />
                <Bar dataKey="collection" fill="#3b82f6" name="collection" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Category Distribution */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Taburan Mengikut Kategori</h3>
            <p className="card-subtitle">Peratusan taksiran mengikut jenis harta</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.byCategory}
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  dataKey="value"
                  label={({ category, value }) => `${category} (${value}%)`}
                >
                  {chartData.byCategory.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip 
                  formatter={(value, name, props) => [
                    `${value}% (RM ${(props.payload.amount / 1000000).toFixed(1)}M)`,
                    'Peratusan'
                  ]}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* District Performance */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Prestasi Mengikut Daerah</h3>
            <p className="card-subtitle">Kadar kutipan setiap daerah</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData.byDistrict} layout="horizontal">
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  type="number"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  tickFormatter={(value) => `${value}%`}
                />
                <YAxis 
                  type="category"
                  dataKey="district"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  width={100}
                />
                <Tooltip 
                  formatter={(value) => [`${value}%`, 'Kadar Kutipan']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar dataKey="rate" fill="#10b981" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Trend Analysis */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Trend Kutipan 5 Tahun</h3>
            <p className="card-subtitle">Perkembangan kadar kutipan</p>
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
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  domain={['dataMin - 2', 'dataMax + 2']}
                  tickFormatter={(value) => `${value}%`}
                />
                <Tooltip 
                  formatter={(value) => [`${value}%`, 'Kadar Kutipan']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Line 
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

      {/* Data Table */}
      <DataTable
        data={tableData}
        columns={tableColumns}
        title="Senarai Taksiran Harta"
        subtitle="Maklumat terperinci taksiran dan kutipan"
        loading={loading}
        searchable={true}
        sortable={true}
        actions={false}
      />
    </PageTemplate>
  )
}

export default TaburanCukai