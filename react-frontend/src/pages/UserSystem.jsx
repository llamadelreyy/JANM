import { useState, useEffect } from 'react'
import { Users, Plus, Shield, Mail, Calendar, Search } from 'lucide-react'
import PageTemplate from '../components/UI/PageTemplate'
import DataTable from '../components/UI/DataTable'
import FormModal from '../components/UI/FormModal'
import StatsCard from '../components/UI/StatsCard'
import { toast } from 'sonner'

const UserSystem = () => {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editingUser, setEditingUser] = useState(null)
  const [stats, setStats] = useState({
    totalUsers: 0,
    activeUsers: 0,
    inactiveUsers: 0,
    adminUsers: 0
  })

  // Mock data for demonstration
  const mockUsers = [
    {
      id: 1,
      username: 'admin',
      email: 'admin@pbt.gov.my',
      fullName: 'Administrator Sistem',
      role: 'Administrator',
      status: 'Aktif',
      lastLogin: '2024-01-15 09:30:00',
      createdDate: '2023-01-01',
      department: 'IT'
    },
    {
      id: 2,
      username: 'penguatkuasa1',
      email: 'penguatkuasa1@pbt.gov.my',
      fullName: 'Ahmad Bin Ali',
      role: 'Penguatkuasa',
      status: 'Aktif',
      lastLogin: '2024-01-14 16:45:00',
      createdDate: '2023-02-15',
      department: 'Penguatkuasaan'
    },
    {
      id: 3,
      username: 'supervisor1',
      email: 'supervisor1@pbt.gov.my',
      fullName: 'Siti Aminah Binti Hassan',
      role: 'Penyelia',
      status: 'Aktif',
      lastLogin: '2024-01-14 14:20:00',
      createdDate: '2023-03-10',
      department: 'Penguatkuasaan'
    },
    {
      id: 4,
      username: 'clerk1',
      email: 'clerk1@pbt.gov.my',
      fullName: 'Mohd Farid Bin Ibrahim',
      role: 'Kerani',
      status: 'Tidak Aktif',
      lastLogin: '2024-01-10 11:15:00',
      createdDate: '2023-04-20',
      department: 'Pentadbiran'
    },
    {
      id: 5,
      username: 'manager1',
      email: 'manager1@pbt.gov.my',
      fullName: 'Dato\' Rahman Bin Abdullah',
      role: 'Pengurus',
      status: 'Aktif',
      lastLogin: '2024-01-15 08:00:00',
      createdDate: '2023-01-15',
      department: 'Pengurusan'
    }
  ]

  useEffect(() => {
    loadUsers()
  }, [])

  const loadUsers = async () => {
    setLoading(true)
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      setUsers(mockUsers)
      
      // Calculate stats
      const totalUsers = mockUsers.length
      const activeUsers = mockUsers.filter(u => u.status === 'Aktif').length
      const inactiveUsers = totalUsers - activeUsers
      const adminUsers = mockUsers.filter(u => u.role === 'Administrator').length
      
      setStats({
        totalUsers,
        activeUsers,
        inactiveUsers,
        adminUsers
      })
    } catch (error) {
      toast.error('Gagal memuat data pengguna')
    } finally {
      setLoading(false)
    }
  }

  const handleAddUser = () => {
    setEditingUser(null)
    setShowModal(true)
  }

  const handleEditUser = (user) => {
    setEditingUser(user)
    setShowModal(true)
  }

  const handleDeleteUser = async (user) => {
    if (window.confirm(`Adakah anda pasti untuk memadam pengguna "${user.fullName}"?`)) {
      try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500))
        setUsers(prev => prev.filter(u => u.id !== user.id))
        toast.success('Pengguna berjaya dipadam')
      } catch (error) {
        toast.error('Gagal memadam pengguna')
      }
    }
  }

  const handleSubmitUser = async (formData) => {
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      if (editingUser) {
        // Update existing user
        setUsers(prev => prev.map(u => 
          u.id === editingUser.id 
            ? { ...u, ...formData, id: editingUser.id }
            : u
        ))
        toast.success('Pengguna berjaya dikemaskini')
      } else {
        // Add new user
        const newUser = {
          ...formData,
          id: Date.now(),
          createdDate: new Date().toISOString().split('T')[0],
          lastLogin: '-'
        }
        setUsers(prev => [...prev, newUser])
        toast.success('Pengguna berjaya ditambah')
      }
      
      setShowModal(false)
      loadUsers() // Refresh stats
    } catch (error) {
      toast.error('Gagal menyimpan pengguna')
    }
  }

  const userFormFields = [
    {
      name: 'username',
      label: 'Nama Pengguna',
      type: 'text',
      required: true,
      placeholder: 'Masukkan nama pengguna'
    },
    {
      name: 'email',
      label: 'Emel',
      type: 'email',
      required: true,
      placeholder: 'Masukkan alamat emel'
    },
    {
      name: 'fullName',
      label: 'Nama Penuh',
      type: 'text',
      required: true,
      placeholder: 'Masukkan nama penuh'
    },
    {
      name: 'role',
      label: 'Peranan',
      type: 'select',
      required: true,
      options: [
        { value: 'Administrator', label: 'Administrator' },
        { value: 'Pengurus', label: 'Pengurus' },
        { value: 'Penyelia', label: 'Penyelia' },
        { value: 'Penguatkuasa', label: 'Penguatkuasa' },
        { value: 'Kerani', label: 'Kerani' }
      ]
    },
    {
      name: 'department',
      label: 'Jabatan',
      type: 'select',
      required: true,
      options: [
        { value: 'IT', label: 'Teknologi Maklumat' },
        { value: 'Penguatkuasaan', label: 'Penguatkuasaan' },
        { value: 'Pentadbiran', label: 'Pentadbiran' },
        { value: 'Pengurusan', label: 'Pengurusan' },
        { value: 'Kewangan', label: 'Kewangan' }
      ]
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
      key: 'username',
      label: 'Nama Pengguna',
      render: (value, user) => (
        <div className="font-medium text-slate-900">{value}</div>
      )
    },
    {
      key: 'fullName',
      label: 'Nama Penuh',
      render: (value, user) => (
        <div>
          <div className="font-medium text-slate-900">{value}</div>
          <div className="text-sm text-slate-500">{user.email}</div>
        </div>
      )
    },
    {
      key: 'role',
      label: 'Peranan',
      render: (value) => (
        <span className={`badge ${
          value === 'Administrator' ? 'badge-danger' :
          value === 'Pengurus' ? 'badge-primary' :
          value === 'Penyelia' ? 'badge-warning' :
          'badge-secondary'
        }`}>
          {value}
        </span>
      )
    },
    {
      key: 'department',
      label: 'Jabatan'
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
      key: 'lastLogin',
      label: 'Login Terakhir',
      render: (value) => (
        <div className="text-sm text-slate-600">
          {value === '-' ? 'Belum login' : new Date(value).toLocaleString('ms-MY')}
        </div>
      )
    }
  ]

  return (
    <PageTemplate
      title="Pengguna Sistem"
      subtitle="Pengurusan pengguna dan akses sistem"
      icon={Users}
      onRefresh={loadUsers}
      loading={loading}
      breadcrumbs={[
        { label: 'Tetapan', href: '/settings' },
        { label: 'Pengurusan Pengguna' },
        { label: 'Pengguna Sistem' }
      ]}
    >
      {/* Statistics Cards */}
      <div className="professional-stats mb-8">
        <StatsCard
          title="Jumlah Pengguna"
          value={stats.totalUsers}
          icon={Users}
          color="blue"
          loading={loading}
        />
        <StatsCard
          title="Pengguna Aktif"
          value={stats.activeUsers}
          icon={Shield}
          color="green"
          loading={loading}
        />
        <StatsCard
          title="Pengguna Tidak Aktif"
          value={stats.inactiveUsers}
          icon={Users}
          color="yellow"
          loading={loading}
        />
        <StatsCard
          title="Administrator"
          value={stats.adminUsers}
          icon={Shield}
          color="red"
          loading={loading}
        />
      </div>

      {/* Users Table */}
      <DataTable
        data={users}
        columns={tableColumns}
        title="Senarai Pengguna"
        subtitle="Pengurusan pengguna sistem Sistem Pemantauan Lesen - MBDK"
        onAdd={handleAddUser}
        onEdit={handleEditUser}
        onDelete={handleDeleteUser}
        loading={loading}
        searchable={true}
        sortable={true}
      />

      {/* User Form Modal */}
      <FormModal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        onSubmit={handleSubmitUser}
        title={editingUser ? 'Kemaskini Pengguna' : 'Tambah Pengguna Baru'}
        fields={userFormFields}
        initialData={editingUser || {}}
        submitText={editingUser ? 'Kemaskini' : 'Tambah'}
      />
    </PageTemplate>
  )
}

export default UserSystem