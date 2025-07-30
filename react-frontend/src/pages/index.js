// Export all pages for easier imports
export { default as Login } from './Login'
export { default as Dashboard } from './Dashboard'
export { default as ExecutiveSummary } from './ExecutiveSummary'
export { default as ZonMajlis } from './ZonMajlis'
export { default as UserSystem } from './UserSystem'

// Placeholder pages - these will be created as simple templates
import PageTemplate from '../components/UI/PageTemplate'
import { Shield, Users, Building, MapPin, FileText, BarChart3, User, Settings } from 'lucide-react'

// Role Management
export const Role = () => (
  <PageTemplate title="Peranan Sistem" subtitle="Pengurusan peranan dan kebenaran" icon={Shield}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan peranan sistem akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// User Role Management
export const UserRole = () => (
  <PageTemplate title="Pengguna & Peranan" subtitle="Pengurusan hubungan pengguna dan peranan" icon={Users}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan pengguna dan peranan akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// User Access Management
export const UserAccess = () => (
  <PageTemplate title="Akses Peranan" subtitle="Pengurusan akses dan kebenaran peranan" icon={Shield}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan akses peranan akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// Department Management
export const Department = () => (
  <PageTemplate title="Jabatan" subtitle="Pengurusan senarai jabatan" icon={Building}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan jabatan akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// Unit Management
export const Unit = () => (
  <PageTemplate title="Unit" subtitle="Pengurusan senarai unit" icon={Building}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan unit akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// State Management
export const State = () => (
  <PageTemplate title="Negeri" subtitle="Pengurusan senarai negeri" icon={MapPin}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan negeri akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// District Management
export const District = () => (
  <PageTemplate title="Daerah" subtitle="Pengurusan senarai daerah" icon={MapPin}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan daerah akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// Town Management
export const Town = () => (
  <PageTemplate title="Mukim" subtitle="Pengurusan senarai mukim" icon={MapPin}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman pengurusan mukim akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// Setup Pages
export const SetupNotis = () => (
  <PageTemplate title="Setup Notis" subtitle="Konfigurasi notis sistem" icon={FileText}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman setup notis akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const SetupKompaun = () => (
  <PageTemplate title="Setup Kompaun" subtitle="Konfigurasi kompaun sistem" icon={FileText}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman setup kompaun akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const SetupNotaPemeriksaan = () => (
  <PageTemplate title="Setup Nota Pemeriksaan" subtitle="Konfigurasi nota pemeriksaan" icon={FileText}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman setup nota pemeriksaan akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const SetupBorang = () => (
  <PageTemplate title="Setup Medan Borang" subtitle="Konfigurasi medan borang" icon={FileText}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman setup medan borang akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// Data Distribution Pages
export const TaburanLesen = () => (
  <PageTemplate title="Taburan Lesen" subtitle="Paparan taburan data lesen" icon={BarChart3}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman taburan lesen akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const TaburanCukai = () => (
  <PageTemplate title="Taburan Taksiran" subtitle="Paparan taburan data taksiran" icon={BarChart3}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman taburan taksiran akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// Graph Pages
export const GraphNotice = () => (
  <PageTemplate title="Graf Notis" subtitle="Statistik dan graf notis" icon={BarChart3}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman graf notis akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const GraphCompound = () => (
  <PageTemplate title="Graf Kompaun" subtitle="Statistik dan graf kompaun" icon={BarChart3}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman graf kompaun akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const GraphInspection = () => (
  <PageTemplate title="Graf Nota Pemeriksaan" subtitle="Statistik dan graf nota pemeriksaan" icon={BarChart3}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman graf nota pemeriksaan akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const GraphConfiscation = () => (
  <PageTemplate title="Graf Sitaan" subtitle="Statistik dan graf sitaan" icon={BarChart3}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman graf sitaan akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// Report Pages
export const ReportKompaun = () => (
  <PageTemplate title="Laporan Kompaun" subtitle="Laporan kompaun lesen" icon={FileText}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman laporan kompaun akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const ReportNotis = () => (
  <PageTemplate title="Laporan Notis" subtitle="Laporan notis sistem" icon={FileText}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman laporan notis akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const ReportHarian = () => (
  <PageTemplate title="Laporan Harian" subtitle="Laporan harian sistem" icon={FileText}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman laporan harian akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

// User Profile Pages
export const UserProfile = () => (
  <PageTemplate title="Profil Pengguna" subtitle="Kemaskini maklumat profil" icon={User}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman profil pengguna akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)

export const UserPassword = () => (
  <PageTemplate title="Tukar Katalaluan" subtitle="Kemaskini katalaluan pengguna" icon={Settings}>
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <p className="text-gray-600">Halaman tukar katalaluan akan dibangunkan di sini.</p>
    </div>
  </PageTemplate>
)