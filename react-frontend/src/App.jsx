import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './stores/authStore'
import Layout from './components/Layout/Layout'
import ProtectedRoute from './components/Auth/ProtectedRoute'

// Import all pages
import {
  Login,
  Dashboard,
  ExecutiveSummary,
  MapView,
  ZonMajlis,
  UserSystem,
  Role,
  UserRole,
  UserAccess,
  Department,
  Unit,
  State,
  District,
  Town,
  SetupNotis,
  SetupKompaun,
  SetupNotaPemeriksaan,
  SetupBorang,
  TaburanLesen,
  TaburanCukai,
  GraphNotice,
  GraphCompound,
  GraphInspection,
  GraphConfiscation,
  ReportKompaun,
  ReportNotis,
  ReportHarian,
  UserProfile,
  UserPassword,
  AIChat
} from './pages/index.jsx'

function App() {
  const { isAuthenticated } = useAuthStore()

  return (
    <div className="min-h-screen bg-gray-50">
      <Routes>
        {/* Public routes */}
        <Route
          path="/"
          element={
            isAuthenticated ? <Navigate to="/ai-chat" replace /> : <Login />
          }
        />
        <Route
          path="/login"
          element={
            isAuthenticated ? <Navigate to="/ai-chat" replace /> : <Login />
          }
        />
        
        {/* Protected routes */}
        <Route 
          path="/*" 
          element={
            <ProtectedRoute>
              <Layout>
                <Routes>
                  {/* Dashboard Routes */}
                  <Route path="/home" element={<ExecutiveSummary />} />
                  <Route path="/executive-summary" element={<ExecutiveSummary />} />
                  <Route path="/map" element={<MapView />} />
                  <Route path="/dashboard" element={<Dashboard />} />
                  <Route path="/zon_majlis" element={<MapView />} />
                  <Route path="/ai-chat" element={<AIChat />} />
                  
                  {/* Data Distribution Routes */}
                  <Route path="/taburan_lesen" element={<TaburanLesen />} />
                  <Route path="/taburan_cukai" element={<TaburanCukai />} />
                  
                  {/* Graph Routes */}
                  <Route path="/graphnotice" element={<GraphNotice />} />
                  <Route path="/graphcompound" element={<GraphCompound />} />
                  <Route path="/graphinspection" element={<GraphInspection />} />
                  <Route path="/graphconfiscation" element={<GraphConfiscation />} />
                  
                  {/* Settings Routes */}
                  <Route path="/user_system" element={<UserSystem />} />
                  <Route path="/role" element={<Role />} />
                  <Route path="/user_role" element={<UserRole />} />
                  <Route path="/user_access" element={<UserAccess />} />
                  <Route path="/senarai_jabatan" element={<Department />} />
                  <Route path="/unit" element={<Unit />} />
                  <Route path="/negeri" element={<State />} />
                  <Route path="/daerah" element={<District />} />
                  <Route path="/mukim" element={<Town />} />
                  <Route path="/setup_notis" element={<SetupNotis />} />
                  <Route path="/setup_kompaun" element={<SetupKompaun />} />
                  <Route path="/setup_nota_pemeriksaan" element={<SetupNotaPemeriksaan />} />
                  <Route path="/setupBorang" element={<SetupBorang />} />
                  
                  {/* Report Routes */}
                  <Route path="/kompaun_lesen" element={<ReportKompaun />} />
                  <Route path="/lapor_notis" element={<ReportNotis />} />
                  <Route path="/rptweatherforecast" element={<ReportHarian />} />
                  
                  {/* User Profile Routes */}
                  <Route path="/user_profile" element={<UserProfile />} />
                  <Route path="/user_password" element={<UserPassword />} />
                  
                  {/* Catch all route */}
                  <Route path="*" element={<Navigate to="/ai-chat" replace />} />
                </Routes>
              </Layout>
            </ProtectedRoute>
          } 
        />
      </Routes>
    </div>
  )
}

export default App