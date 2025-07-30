import { useState, useEffect } from 'react'
import { 
  FileText, 
  TrendingUp, 
  Calendar, 
  Users, 
  MapPin,
  Building,
  AlertTriangle,
  CheckCircle,
  Clock,
  DollarSign,
  Download,
  Filter,
  Search
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
import { toast } from 'sonner'

const LicenseDistribution = () => {
  const [loading, setLoading] = useState(true)
  const [selectedCategory, setSelectedCategory] = useState('all')
  const [selectedStatus, setSelectedStatus] = useState('all')
  const [selectedPeriod, setSelectedPeriod] = useState('monthly')
  const [licenseStats, setLicenseStats] = useState({})
  const [chartData, setChartData] = useState({})
  const [licenseData, setLicenseData] = useState([])
  const [expiringLicenses, setExpiringLicenses] = useState([])

  // Mock comprehensive license data
  const mockLicenseStats = {
    totalLicenses: 8456,
    activeLicenses: 7234,
    expiredLicenses: 892,
    pendingRenewal: 330,
    newApplications: 156,
    totalRevenue: 2450000,
    renewalRate: 85.6,
    complianceRate: 89.3
  }

  const mockMonthlyTrend = [
    { 
      month: 'Jan', 
      issued: 145, 
      renewed: 289, 
      expired: 67, 
      revenue: 245000,
      applications: 89,
      compliance: 87.2 
    },
    { 
      month: 'Feb', 
      issued: 134, 
      renewed: 298, 
      expired: 45, 
      revenue: 267000,
      applications: 76,
      compliance: 88.1 
    },
    { 
      month: 'Mar', 
      issued: 167, 
      renewed: 334, 
      expired: 52, 
      revenue: 289000,
      applications: 98,
      compliance: 89.3 
    },
    { 
      month: 'Apr', 
      issued: 123, 
      renewed: 267, 
      expired: 38, 
      revenue: 234000,
      applications: 67,
      compliance: 90.1 
    },
    { 
      month: 'May', 
      issued: 189, 
      renewed: 345, 
      expired: 61, 
      revenue: 312000,
      applications: 112,
      compliance: 88.7 
    },
    { 
      month: 'Jun', 
      issued: 156, 
      renewed: 298, 
      expired: 44, 
      revenue: 278000,
      applications: 89,
      compliance: 89.5 
    }
  ]

  const mockLicenseCategories = [
    { category: 'Perniagaan Makanan', count: 2456, percentage: 29.1, revenue: 736800, color: '#3B82F6' },
    { category: 'Kedai Runcit', count: 1834, percentage: 21.7, revenue: 550200, color: '#10B981' },
    { category: 'Restoran & Kafe', count: 1567, percentage: 18.5, revenue: 470100, color: '#F59E0B' },
    { category: 'Perkhidmatan', count: 1234, percentage: 14.6, revenue: 370200, color: '#EF4444' },
    { category: 'Hiburan', count: 789, percentage: 9.3, revenue: 236700, color: '#8B5CF6' },
    { category: 'Lain-lain', count: 576, percentage: 6.8, revenue: 172800, color: '#6B7280' }
  ]

  const mockLicenseData = [
    {
      id: 'LIC001234',
      businessName: 'Restoran Seri Wangi',
      ownerName: 'Ahmad Bin Ali',
      category: 'Perniagaan Makanan',
      address: 'No. 123, Jalan Merdeka, KL',
      issueDate: '2023-01-15',
      expiryDate: '2024-01-15',
      status: 'Aktif',
      fee: 300,
      zone: 'Zon Pusat Bandar',
      contact: '03-12345678'
    },
    {
      id: 'LIC001235',
      businessName: 'Kedai Runcit Pak Man',
      ownerName: 'Mohd Sulaiman',
      category: 'Kedai Runcit',
      address: 'No. 456, Jalan Bangsar, KL',
      issueDate: '2023-02-20',
      expiryDate: '2024-02-20',
      status: 'Aktif',
      fee: 250,
      zone: 'Zon Bandar Selatan',
      contact: '03-23456789'
    },
    {
      id: 'LIC001236',
      businessName: 'Kafe Kopi Sedap',
      ownerName: 'Siti Aminah Binti Hassan',
      category: 'Restoran & Kafe',
      address: 'No. 789, Jalan Pudu, KL',
      issueDate: '2023-03-10',
      expiryDate: '2024-03-10',
      status: 'Hampir Tamat',
      fee: 350,
      zone: 'Zon Komersial',
      contact: '03-34567890'
    },
    {
      id: 'LIC001237',
      businessName: 'Salon Kecantikan Seri',
      ownerName: 'Lim Ah Chong',
      category: 'Perkhidmatan',
      address: 'No. 321, Jalan Bukit Bintang, KL',
      issueDate: '2022-12-01',
      expiryDate: '2023-12-01',
      status: 'Tamat Tempoh',
      fee: 400,
      zone: 'Zon Pusat Bandar',
      contact: '03-45678901'
    },
    {
      id: 'LIC001238',
      businessName: 'Pusat Hiburan Gemilang',
      ownerName: 'Rajesh Kumar',
      category: 'Hiburan',
      address: 'No. 654, Jalan Cheras, KL',
      issueDate: '2023-04-15',
      expiryDate: '2024-04-15',
      status: 'Aktif',
      fee: 500,
      zone: 'Zon Bandar Timur',
      contact: '03-56789012'
    }
  ]

  const mockExpiringLicenses = [
    { id: 'LIC001239', businessName: 'Warung Mak Cik', expiryDate: '2024-01-25', daysLeft: 10, category: 'Perniagaan Makanan' },
    { id: 'LIC001240', businessName: 'Kedai Basikal Jaya', expiryDate: '2024-01-28', daysLeft: 13, category: 'Kedai Runcit' },
    { id: 'LIC001241', businessName: 'Restoran Seafood', expiryDate: '2024-02-02', daysLeft: 18, category: 'Restoran & Kafe' },
    { id: 'LIC001242', businessName: 'Spa Relaksasi', expiryDate: '2024-02-05', daysLeft: 21, category: 'Perkhidmatan' },
    { id: 'LIC001243', businessName: 'Karaoke Box', expiryDate: '2024-02-08', daysLeft: 24, category: 'Hiburan' }
  ]

  useEffect(() => {
    loadData()
  }, [selectedCategory, selectedStatus, selectedPeriod])

  const loadData = async () => {
    setLoading(true)
    try {
      await new Promise(resolve => setTimeout(resolve, 1000))
      setLicenseStats(mockLicenseStats)
      setChartData({
        monthly: mockMonthlyTrend,
        categories: mockLicenseCategories
      })
      setLicenseData(mockLicenseData)
      setExpiringLicenses(mockExpiringLicenses)
    } catch (error) {
      console.error('Error loading license data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleExport = () => {
    toast.success('Data lesen sedang dieksport...')
  }

  const licenseColumns = [
    {
      key: 'id',
      label: 'ID Lesen',
      render: (value) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'businessName',
      label: 'Nama Perniagaan',
      render: (value, license) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{license.ownerName}</div>
        </div>
      )
    },
    {
      key: 'category',
      label: 'Kategori',
      render: (value) => (
        <span className={`badge ${
          value === 'Perniagaan Makanan' ? 'badge-primary' :
          value === 'Kedai Runcit' ? 'badge-success' :
          value === 'Restoran & Kafe' ? 'badge-warning' :
          value === 'Perkhidmatan' ? 'badge-secondary' :
          value === 'Hiburan' ? 'badge-danger' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'address',
      label: 'Alamat',
      render: (value, license) => (
        <div>
          <div className="text-sm text-slate-900">{value}</div>
          <div className="text-xs text-slate-500">{license.zone}</div>
        </div>
      )
    },
    {
      key: 'expiryDate',
      label: 'Tarikh Tamat',
      render: (value) => (
        <div className="text-sm text-slate-600">
          {new Date(value).toLocaleDateString('ms-MY')}
        </div>
      )
    },
    {
      key: 'status',
      label: 'Status',
      render: (value) => (
        <span className={`badge ${
          value === 'Aktif' ? 'badge-success' :
          value === 'Hampir Tamat' ? 'badge-warning' :
          value === 'Tamat Tempoh' ? 'badge-danger' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'fee',
      label: 'Yuran (RM)',
      render: (value) => (
        <div className="text-right professional-number">
          {value.toLocaleString('ms-MY')}
        </div>
      )
    }
  ]

  const expiringColumns = [
    {
      key: 'id',
      label: 'ID Lesen'
    },
    {
      key: 'businessName',
      label: 'Nama Perniagaan'
    },
    {
      key: 'category',
      label: 'Kategori'
    },
    {
      key: 'daysLeft',
      label: 'Hari Tinggal',
      render: (value) => (
        <div className="text-center">
          <span className={`professional-number font-semibold ${
            value <= 7 ? 'text-red-600' :
            value <= 14 ? 'text-yellow-600' :
            'text-blue-600'
          }`}>
            {value}
          </span>
        </div>
      )
    },
    {
      key: 'expiryDate',
      label: 'Tarikh Tamat',
      render: (value) => (
        <div className="text-sm text-slate-600">
          {new Date(value).toLocaleDateString('ms-MY')}
        </div>
      )
    }
  ]

  return (
    <PageTemplate
      title="Taburan Lesen"
      subtitle="Analisis dan pemantauan lesen perniagaan dalam kawasan majlis"
      icon={FileText}
      onRefresh={loadData}
      loading={loading}
      breadcrumbs={[
        { label: 'Taburan Data' },
        { label: 'Lesen' }
      ]}
      actions={
        <div className="flex gap-3">
          <select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            className="select"
          >
            <option value="all">Semua Kategori</option>
            <option value="food">Perniagaan Makanan</option>
            <option value="retail">Kedai Runcit</option>
            <option value="restaurant">Restoran & Kafe</option>
            <option value="service">Perkhidmatan</option>
            <option value="entertainment">Hiburan</option>
          </select>
          <select
            value={selectedStatus}
            onChange={(e) => setSelectedStatus(e.target.value)}
            className="select"
          >
            <option value="all">Semua Status</option>
            <option value="active">Aktif</option>
            <option value="expiring">Hampir Tamat</option>
            <option value="expired">Tamat Tempoh</option>
          </select>
          <button onClick={handleExport} className="btn-primary">
            <Download className="h-4 w-4 mr-2" />
            Eksport
          </button>
        </div>
      }
    >
      {/* License Statistics */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Jumlah Lesen"
          value={licenseStats.totalLicenses}
          subtitle="Lesen keseluruhan"
          icon={FileText}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Lesen Aktif"
          value={licenseStats.activeLicenses}
          subtitle={`${((licenseStats.activeLicenses / licenseStats.totalLicenses) * 100).toFixed(1)}% daripada jumlah`}
          icon={CheckCircle}
          color="green"
          trend="up"
          trendValue="+3.2%"
          loading={loading}
        />
        <StatsCard
          title="Lesen Tamat Tempoh"
          value={licenseStats.expiredLicenses}
          subtitle="Memerlukan pembaharuan"
          icon={AlertTriangle}
          color="red"
          trend="down"
          trendValue="-8.1%"
          loading={loading}
        />
        <StatsCard
          title="Permohonan Baru"
          value={licenseStats.newApplications}
          subtitle="Dalam pemprosesan"
          icon={Clock}
          color="yellow"
          loading={loading}
        />
      </div>

      {/* Secondary KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatsCard
          title="Hasil Kutipan"
          value={`RM ${(licenseStats.totalRevenue / 1000000).toFixed(1)}M`}
          subtitle="Kutipan yuran lesen"
          icon={DollarSign}
          color="green"
          trend="up"
          trendValue="+12.5%"
          loading={loading}
        />
        <StatsCard
          title="Kadar Pembaharuan"
          value={`${licenseStats.renewalRate}%`}
          subtitle="Pembaharuan tepat masa"
          icon={TrendingUp}
          color="purple"
          trend="up"
          trendValue="+2.8%"
          loading={loading}
        />
        <StatsCard
          title="Kadar Pematuhan"
          value={`${licenseStats.complianceRate}%`}
          subtitle="Pematuhan peraturan"
          icon={CheckCircle}
          color="indigo"
          loading={loading}
        />
        <StatsCard
          title="Menunggu Pembaharuan"
          value={licenseStats.pendingRenewal}
          subtitle="Dalam tempoh grace"
          icon={Calendar}
          color="yellow"
          loading={loading}
        />
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Monthly License Trend */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Trend Lesen Bulanan</h3>
            <p className="card-subtitle">Pengeluaran dan pembaharuan lesen</p>
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
                    name === 'issued' ? 'Dikeluarkan' :
                    name === 'renewed' ? 'Diperbaharui' :
                    name === 'expired' ? 'Tamat Tempoh' :
                    name === 'compliance' ? 'Pematuhan (%)' : name
                  ]}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar dataKey="issued" fill="#3b82f6" name="issued" />
                <Bar dataKey="renewed" fill="#10b981" name="renewed" />
                <Bar dataKey="expired" fill="#ef4444" name="expired" />
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

        {/* License Categories */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Kategori Lesen</h3>
            <p className="card-subtitle">Taburan mengikut jenis perniagaan</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData.categories}
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  dataKey="percentage"
                  label={({ category, percentage }) => `${category} (${percentage}%)`}
                >
                  {chartData.categories?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip 
                  formatter={(value, name, props) => [
                    `${value}% (${props.payload.count} lesen)`,
                    'Peratusan'
                  ]}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Revenue Trend */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Trend Hasil Kutipan</h3>
            <p className="card-subtitle">Kutipan yuran lesen bulanan</p>
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
                  tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`}
                />
                <Tooltip 
                  formatter={(value) => [`RM ${(value / 1000).toFixed(0)}K`, 'Hasil']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Area 
                  type="monotone" 
                  dataKey="revenue" 
                  stroke="#10b981" 
                  fill="#10b981"
                  fillOpacity={0.6}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Category Revenue */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title">Hasil Mengikut Kategori</h3>
            <p className="card-subtitle">Kutipan yuran setiap kategori</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData.categories} layout="horizontal">
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis 
                  type="number"
                  tick={{ fontSize: 12, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`}
                />
                <YAxis 
                  type="category"
                  dataKey="category"
                  tick={{ fontSize: 11, fill: '#64748b' }}
                  axisLine={{ stroke: '#e2e8f0' }}
                  width={120}
                />
                <Tooltip 
                  formatter={(value) => [`RM ${value.toLocaleString()}`, 'Hasil']}
                  labelStyle={{ color: '#1e293b' }}
                />
                <Bar 
                  dataKey="revenue" 
                  radius={[0, 4, 4, 0]}
                >
                  {chartData.categories?.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Data Tables */}
      <div className="space-y-8">
        {/* Expiring Licenses Alert */}
        <div className="professional-card">
          <div className="card-header">
            <h3 className="card-title flex items-center">
              <AlertTriangle className="h-5 w-5 text-yellow-500 mr-2" />
              Lesen Hampir Tamat Tempoh
            </h3>
            <p className="card-subtitle">Lesen yang akan tamat dalam 30 hari</p>
          </div>
          <div className="card-content">
            <div className="overflow-x-auto">
              <table className="gov-table">
                <thead>
                  <tr>
                    {expiringColumns.map((column) => (
                      <th key={column.key} className="gov-table th">
                        {column.label}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {expiringLicenses.map((license, index) => (
                    <tr key={index} className="gov-table tr">
                      {expiringColumns.map((column) => (
                        <td key={column.key} className="gov-table td">
                          {column.render ? 
                            column.render(license[column.key], license) : 
                            license[column.key]
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

        {/* All Licenses */}
        <DataTable
          data={licenseData}
          columns={licenseColumns}
          title="Senarai Lesen Perniagaan"
          subtitle="Maklumat lengkap semua lesen dalam kawasan majlis"
          loading={loading}
          searchable={true}
          sortable={true}
          actions={false}
        />
      </div>
    </PageTemplate>
  )
}

export default LicenseDistribution