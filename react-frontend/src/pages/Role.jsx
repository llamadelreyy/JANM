import { useState, useEffect } from 'react'
import { Shield, Plus, Users, Settings, Lock } from 'lucide-react'
import PageTemplate from '../components/UI/PageTemplate'
import DataTable from '../components/UI/DataTable'
import FormModal from '../components/UI/FormModal'
import StatsCard from '../components/UI/StatsCard'
import { toast } from 'sonner'

const Role = () => {
  const [roles, setRoles] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editingRole, setEditingRole] = useState(null)
  const [stats, setStats] = useState({
    totalRoles: 0,
    activeRoles: 0,
    systemRoles: 0,
    customRoles: 0
  })

  // Mock data for demonstration
  const mockRoles = [
    {
      id: 1,
      name: 'Administrator',
      description: 'Akses penuh kepada semua fungsi sistem',
      type: 'Sistem',
      status: 'Aktif',
      userCount: 2,
      permissions: ['CREATE', 'READ', 'UPDATE', 'DELETE', 'ADMIN'],
      createdDate: '2023-01-01',
      lastModified: '2024-01-10'
    },
    {
      id: 2,
      name: 'Pengurus',
      description: 'Akses pengurusan dan laporan',
      type: 'Sistem',
      status: 'Aktif',
      userCount: 3,
      permissions: ['READ', 'UPDATE', 'REPORT'],
      createdDate: '2023-01-01',
      lastModified: '2024-01-05'
    },
    {
      id: 3,
      name: 'Penyelia',
      description: 'Akses penyelia untuk penguatkuasaan',
      type: 'Sistem',
      status: 'Aktif',
      userCount: 5,
      permissions: ['READ', 'UPDATE', 'APPROVE'],
      createdDate: '2023-01-01',
      lastModified: '2023-12-20'
    },
    {
      id: 4,
      name: 'Penguatkuasa',
      description: 'Akses untuk aktiviti penguatkuasaan',
      type: 'Sistem',
      status: 'Aktif',
      userCount: 15,
      permissions: ['READ', 'CREATE', 'UPDATE'],
      createdDate: '2023-01-01',
      lastModified: '2023-11-15'
    },
    {
      id: 5,
      name: 'Kerani',
      description: 'Akses asas untuk kemasukan data',
      type: 'Sistem',
      status: 'Aktif',
      userCount: 8,
      permissions: ['READ', 'CREATE'],
      createdDate: '2023-01-01',
      lastModified: '2023-10-30'
    },
    {
      id: 6,
      name: 'Audit',
      description: 'Akses khusus untuk audit dan pemantauan',
      type: 'Kustom',
      status: 'Aktif',
      userCount: 2,
      permissions: ['READ', 'AUDIT'],
      createdDate: '2023-06-15',
      lastModified: '2023-12-01'
    }
  ]

  const permissionOptions = [
    { value: 'CREATE', label: 'Cipta (Create)' },
    { value: 'READ', label: 'Baca (Read)' },
    { value: 'UPDATE', label: 'Kemaskini (Update)' },
    { value: 'DELETE', label: 'Padam (Delete)' },
    { value: 'ADMIN', label: 'Pentadbiran (Admin)' },
    { value: 'REPORT', label: 'Laporan (Report)' },
    { value: 'APPROVE', label: 'Kelulusan (Approve)' },
    { value: 'AUDIT', label: 'Audit' }
  ]

  useEffect(() => {
    loadRoles()
  }, [])

  const loadRoles = async () => {
    setLoading(true)
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      setRoles(mockRoles)
      
      // Calculate stats
      const totalRoles = mockRoles.length
      const activeRoles = mockRoles.filter(r => r.status === 'Aktif').length
      const systemRoles = mockRoles.filter(r => r.type === 'Sistem').length
      const customRoles = mockRoles.filter(r => r.type === 'Kustom').length
      
      setStats({
        totalRoles,
        activeRoles,
        systemRoles,
        customRoles
      })
    } catch (error) {
      toast.error('Gagal memuat data peranan')
    } finally {
      setLoading(false)
    }
  }

  const handleAddRole = () => {
    setEditingRole(null)
    setShowModal(true)
  }

  const handleEditRole = (role) => {
    setEditingRole(role)
    setShowModal(true)
  }

  const handleDeleteRole = async (role) => {
    if (role.type === 'Sistem') {
      toast.error('Peranan sistem tidak boleh dipadam')
      return
    }

    if (role.userCount > 0) {
      toast.error('Tidak boleh memadam peranan yang masih digunakan oleh pengguna')
      return
    }

    if (window.confirm(`Adakah anda pasti untuk memadam peranan "${role.name}"?`)) {
      try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500))
        setRoles(prev => prev.filter(r => r.id !== role.id))
        toast.success('Peranan berjaya dipadam')
        loadRoles() // Refresh stats
      } catch (error) {
        toast.error('Gagal memadam peranan')
      }
    }
  }

  const handleSubmitRole = async (formData) => {
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      if (editingRole) {
        // Update existing role
        setRoles(prev => prev.map(r => 
          r.id === editingRole.id 
            ? { 
                ...r, 
                ...formData, 
                id: editingRole.id,
                lastModified: new Date().toISOString().split('T')[0]
              }
            : r
        ))
        toast.success('Peranan berjaya dikemaskini')
      } else {
        // Add new role
        const newRole = {
          ...formData,
          id: Date.now(),
          type: 'Kustom',
          userCount: 0,
          createdDate: new Date().toISOString().split('T')[0],
          lastModified: new Date().toISOString().split('T')[0]
        }
        setRoles(prev => [...prev, newRole])
        toast.success('Peranan berjaya ditambah')
      }
      
      setShowModal(false)
      loadRoles() // Refresh stats
    } catch (error) {
      toast.error('Gagal menyimpan peranan')
    }
  }

  const roleFormFields = [
    {
      name: 'name',
      label: 'Nama Peranan',
      type: 'text',
      required: true,
      placeholder: 'Masukkan nama peranan'
    },
    {
      name: 'description',
      label: 'Penerangan',
      type: 'textarea',
      required: true,
      placeholder: 'Masukkan penerangan peranan',
      rows: 3
    },
    {
      name: 'status',
      label: 'Status',
      type: 'select',
      required: true,
      options: [
        { value: 'Aktif', label: 'Aktif' },
        { value: 'Tidak Aktif', label: 'Tidak Aktif' }
      ]
    }
  ]

  const tableColumns = [
    {
      key: 'name',
      label: 'Nama Peranan',
      render: (value, role) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{role.description}</div>
        </div>
      )
    },
    {
      key: 'type',
      label: 'Jenis',
      render: (value) => (
        <span className={`badge ${
          value === 'Sistem' ? 'badge-primary' : 'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'userCount',
      label: 'Bilangan Pengguna',
      render: (value) => (
        <div className="text-center">
          <span className="professional-number">{value}</span>
        </div>
      )
    },
    {
      key: 'permissions',
      label: 'Kebenaran',
      render: (value) => (
        <div className="flex flex-wrap gap-1">
          {value.slice(0, 3).map(permission => (
            <span key={permission} className="badge badge-secondary text-xs">
              {permission}
            </span>
          ))}
          {value.length > 3 && (
            <span className="badge badge-secondary text-xs">
              +{value.length - 3}
            </span>
          )}
        </div>
      )
    },
    {
      key: 'status',
      label: 'Status',
      render: (value) => (
        <span className={`badge ${
          value === 'Aktif' ? 'badge-success' : 'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'lastModified',
      label: 'Kemaskini Terakhir',
      render: (value) => (
        <div className="text-sm text-slate-600">
          {new Date(value).toLocaleDateString('ms-MY')}
        </div>
      )
    }
  ]

  return (
    <PageTemplate
      title="Peranan Sistem"
      subtitle="Pengurusan peranan dan kebenaran akses"
      icon={Shield}
      onRefresh={loadRoles}
      loading={loading}
      breadcrumbs={[
        { label: 'Tetapan', href: '/settings' },
        { label: 'Pengurusan Pengguna' },
        { label: 'Peranan Sistem' }
      ]}
    >
      {/* Statistics Cards */}
      <div className="professional-stats mb-8">
        <StatsCard
          title="Jumlah Peranan"
          value={stats.totalRoles}
          icon={Shield}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Peranan Aktif"
          value={stats.activeRoles}
          icon={Users}
          color="green"
          loading={loading}
        />
        <StatsCard
          title="Peranan Sistem"
          value={stats.systemRoles}
          icon={Settings}
          color="purple"
          loading={loading}
        />
        <StatsCard
          title="Peranan Kustom"
          value={stats.customRoles}
          icon={Lock}
          color="indigo"
          loading={loading}
        />
      </div>

      {/* Roles Table */}
      <DataTable
        data={roles}
        columns={tableColumns}
        title="Senarai Peranan"
        subtitle="Pengurusan peranan dan kebenaran sistem"
        onAdd={handleAddRole}
        onEdit={handleEditRole}
        onDelete={handleDeleteRole}
        loading={loading}
        searchable={true}
        sortable={true}
      />

      {/* Role Form Modal */}
      <FormModal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        onSubmit={handleSubmitRole}
        title={editingRole ? 'Kemaskini Peranan' : 'Tambah Peranan Baru'}
        fields={roleFormFields}
        initialData={editingRole || {}}
        submitText={editingRole ? 'Kemaskini' : 'Tambah'}
      />
    </PageTemplate>
  )
}

export default Role