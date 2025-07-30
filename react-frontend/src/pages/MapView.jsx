import { useState, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { ArrowLeft, Home, MapPin, Navigation, Map, Globe } from 'lucide-react'
import PageTemplate from '../components/UI/PageTemplate'
import InteractiveMap from '../components/Maps/InteractiveMap'
import { constituencyData, getPartyColor, formatParty } from '../data/constituencyData'

const MapView = () => {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const [selectedLocation, setSelectedLocation] = useState('meru')
  const [loading, setLoading] = useState(true)
  const [mapProvider, setMapProvider] = useState('openstreetmap') // 'openstreetmap' or 'googlemaps'
  
  // Check if we're on the zone council route
  const isZoneMajlisRoute = window.location.pathname === '/zon_majlis'

  // Get location from URL params or default to telok-gong
  useEffect(() => {
    const locationParam = searchParams.get('location')
    if (locationParam) {
      setSelectedLocation(locationParam)
    }
  }, [searchParams])

  // District mappings with coordinates for Klang districts (matching GeoJSON data)
  const districtMappings = {
    'meru': {
      name: 'Meru, Klang, Selangor, Malaysia',
      lat: 3.193,
      lng: 101.478,
      zoom: 14
    },
    'sementa': {
      name: 'Sementa, Klang, Selangor, Malaysia',
      lat: 3.156,
      lng: 101.419,
      zoom: 14
    },
    'selat-klang': {
      name: 'Selat Klang, Klang, Selangor, Malaysia',
      lat: 3.0442,
      lng: 101.4464,
      zoom: 14
    },
    'bandar-baru-klang': {
      name: 'Bandar Baru Klang, Klang, Selangor, Malaysia',
      lat: 3.0560,
      lng: 101.4304,
      zoom: 14
    },
    'pelabuhan-klang': {
      name: 'Pelabuhan Klang, Klang, Selangor, Malaysia',
      lat: 3.0048,
      lng: 101.3929,
      zoom: 14
    },
    'pandamaran': {
      name: 'Pandamaran, Klang, Selangor, Malaysia',
      lat: 3.0264,
      lng: 101.4264,
      zoom: 14
    },
    'sentosa': {
      name: 'Sentosa, Klang, Selangor, Malaysia',
      lat: 3.0167,
      lng: 101.4167,
      zoom: 14
    },
    'sungai-kandis': {
      name: 'Sungai Kandis, Klang, Selangor, Malaysia',
      lat: 3.0500,
      lng: 101.5000,
      zoom: 14
    }
  }

  const locationNames = {
    'meru': 'Meru',
    'sementa': 'Sementa',
    'selat-klang': 'Selat Klang',
    'bandar-baru-klang': 'Bandar Baru Klang',
    'pelabuhan-klang': 'Pelabuhan Klang',
    'pandamaran': 'Pandamaran',
    'sentosa': 'Sentosa',
    'sungai-kandis': 'Sungai Kandis'
  }

  const districtInfo = districtMappings[selectedLocation] || districtMappings['meru']
  const searchQuery = districtInfo.name
  
  // Generate map URLs based on provider
  const getMapUrl = () => {
    if (mapProvider === 'googlemaps') {
      // Google Maps embed without API key
      return `https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d15955.${Math.floor(Math.random() * 1000)}!2d${districtInfo.lng}!3d${districtInfo.lat}!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2z${encodeURIComponent(districtInfo.name)}!5e0!3m2!1sen!2smy!4v${Date.now()}!5m2!1sen!2smy`
    } else {
      // OpenStreetMap with specific coordinates for the selected district
      const bbox = {
        minLng: districtInfo.lng - 0.01,
        minLat: districtInfo.lat - 0.01,
        maxLng: districtInfo.lng + 0.01,
        maxLat: districtInfo.lat + 0.01
      }
      return `https://www.openstreetmap.org/export/embed.html?bbox=${bbox.minLng},${bbox.minLat},${bbox.maxLng},${bbox.maxLat}&layer=mapnik&marker=${districtInfo.lat},${districtInfo.lng}`
    }
  }
  
  const embedUrl = getMapUrl()

  const handleLocationChange = (newLocation) => {
    setSelectedLocation(newLocation)
    // Update URL without page reload - use current path
    const currentPath = window.location.pathname
    navigate(`${currentPath}?location=${newLocation}`, { replace: true })
  }

  const handleBackToDashboard = () => {
    navigate('/home')
  }

  const handleBackToSummary = () => {
    navigate('/executive-summary')
  }


  return (
    <PageTemplate
      title={isZoneMajlisRoute ? `Ringkasan Zon Ahli Majlis - ${locationNames[selectedLocation]}` : `Peta Kawasan - ${locationNames[selectedLocation]}`}
      subtitle={isZoneMajlisRoute ? "Paparan peta kawasan zon ahli majlis PBT" : "Paparan peta interaktif kawasan PBT"}
      icon={MapPin}
      loading={false}
      breadcrumbs={isZoneMajlisRoute ? [
        { label: 'Dashboard', href: '/home' },
        { label: 'Ringkasan Zon Ahli Majlis' }
      ] : [
        { label: 'Ringkasan Eksekutif', href: '/executive-summary' },
        { label: 'Peta Kawasan' }
      ]}
      actions={
        <div className="flex gap-3 items-center">
          <select
            value={selectedLocation}
            onChange={(e) => handleLocationChange(e.target.value)}
            className="select w-40"
          >
            <option value="meru">Meru</option>
            <option value="sementa">Sementa</option>
            <option value="selat-klang">Selat Klang</option>
            <option value="bandar-baru-klang">Bandar Baru Klang</option>
            <option value="pelabuhan-klang">Pelabuhan Klang</option>
            <option value="pandamaran">Pandamaran</option>
            <option value="sentosa">Sentosa</option>
            <option value="sungai-kandis">Sungai Kandis</option>
          </select>
          
          {/* Map Provider Toggle */}
          <div className="flex items-center bg-slate-100 rounded-lg p-1">
            <button
              onClick={() => setMapProvider('openstreetmap')}
              className={`flex items-center space-x-1 px-3 py-1 rounded-md text-sm font-medium transition-colors ${
                mapProvider === 'openstreetmap'
                  ? 'bg-white text-blue-600 shadow-sm'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
              title="OpenStreetMap"
            >
              <Map className="h-4 w-4" />
              <span className="hidden sm:inline">OSM</span>
            </button>
            <button
              onClick={() => setMapProvider('googlemaps')}
              className={`flex items-center space-x-1 px-3 py-1 rounded-md text-sm font-medium transition-colors ${
                mapProvider === 'googlemaps'
                  ? 'bg-white text-blue-600 shadow-sm'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
              title="Google Maps"
            >
              <Globe className="h-4 w-4" />
              <span className="hidden sm:inline">Google</span>
            </button>
          </div>
          
          <button
            onClick={handleBackToSummary}
            className="btn-secondary whitespace-nowrap min-w-[140px]"
            title="Kembali ke Ringkasan Eksekutif"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Summary
          </button>
          <button
            onClick={handleBackToDashboard}
            className="btn-primary whitespace-nowrap min-w-[120px]"
            title="Kembali ke Dashboard"
          >
            <Home className="h-4 w-4 mr-2" />
            Dashboard
          </button>
        </div>
      }
    >
      {/* Interactive Map Container */}
      <div className="professional-card">
        <div className="card-header">
          <h3 className="card-title">Peta Kawasan {locationNames[selectedLocation]}</h3>
          <p className="card-subtitle">
            Lokasi: {searchQuery} | Provider: {mapProvider === 'openstreetmap' ? 'OpenStreetMap' : 'Google Maps'}
            {mapProvider === 'openstreetmap' && <span className="ml-2 text-blue-600">• Sempadan Daerah Dipaparkan</span>}
          </p>
        </div>
        <div className="card-content p-0">
          <InteractiveMap
            selectedLocation={selectedLocation}
            mapProvider={mapProvider}
            onLocationChange={handleLocationChange}
            districtMappings={districtMappings}
            locationNames={locationNames}
          />
        </div>
      </div>

      {/* Location Information */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-6">
        <div className="professional-card">
          <div className="card-header">
            <h4 className="card-title">Maklumat Kawasan</h4>
          </div>
          <div className="card-content">
            {constituencyData[selectedLocation] ? (
              <div className="space-y-4">
                {/* Constituency Header */}
                <div className="border-b border-gray-200 pb-3">
                  <h5 className="font-bold text-lg text-gray-900">
                    {constituencyData[selectedLocation].code} - {constituencyData[selectedLocation].name}
                  </h5>
                </div>

                {/* Representative Information */}
                <div className="space-y-3">
                  <div>
                    <span className="text-sm font-medium text-slate-600">Ahli Majlis:</span>
                    <p className="text-slate-900 font-semibold">{constituencyData[selectedLocation].representative.name}</p>
                  </div>
                  
                  <div>
                    <span className="text-sm font-medium text-slate-600">Parti:</span>
                    <div className="mt-1">
                      <span
                        className="inline-block px-3 py-1 rounded-full text-white text-sm font-medium"
                        style={{ backgroundColor: getPartyColor(constituencyData[selectedLocation].representative.party) }}
                      >
                        {formatParty(constituencyData[selectedLocation].representative.party)}
                      </span>
                    </div>
                  </div>

                  <div>
                    <span className="text-sm font-medium text-slate-600">Penggal:</span>
                    <p className="text-slate-900">{constituencyData[selectedLocation].term}</p>
                  </div>

                  <div>
                    <span className="text-sm font-medium text-slate-600">Dewan:</span>
                    <p className="text-slate-900">{constituencyData[selectedLocation].representative.assembly}</p>
                  </div>
                </div>

                {/* Area Information */}
                <div className="border-t border-gray-200 pt-3">
                  <div>
                    <span className="text-sm font-medium text-slate-600">Kawasan:</span>
                    <p className="text-slate-900 text-sm mt-1">{constituencyData[selectedLocation].area.description}</p>
                  </div>
                  
                  {constituencyData[selectedLocation].localities.length > 0 && (
                    <div className="mt-3">
                      <span className="text-sm font-medium text-slate-600">Lokaliti Utama:</span>
                      <div className="flex flex-wrap gap-1 mt-2">
                        {constituencyData[selectedLocation].localities.map((locality, index) => (
                          <span
                            key={index}
                            className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded"
                          >
                            {locality}
                          </span>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </div>
            ) : (
              <div className="space-y-3">
                <div>
                  <span className="text-sm font-medium text-slate-600">Kawasan:</span>
                  <p className="text-slate-900">{locationNames[selectedLocation]}</p>
                </div>
                <div>
                  <span className="text-sm font-medium text-slate-600">Daerah:</span>
                  <p className="text-slate-900">Klang</p>
                </div>
                <div>
                  <span className="text-sm font-medium text-slate-600">Negeri:</span>
                  <p className="text-slate-900">Selangor</p>
                </div>
                <div>
                  <span className="text-sm font-medium text-slate-600">Negara:</span>
                  <p className="text-slate-900">Malaysia</p>
                </div>
              </div>
            )}
          </div>
        </div>

        <div className="professional-card">
          <div className="card-header">
            <h4 className="card-title">Statistik Kawasan</h4>
          </div>
          <div className="card-content">
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-sm text-slate-600">Premis Berlesen:</span>
                <span className="font-medium text-green-600">52</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm text-slate-600">Premis Tidak Berlesen:</span>
                <span className="font-medium text-red-600">23</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm text-slate-600">Premis Bercukai:</span>
                <span className="font-medium text-blue-600">68</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm text-slate-600">Kadar Pematuhan:</span>
                <span className="font-medium text-purple-600">69.3%</span>
              </div>
            </div>
          </div>
        </div>

        <div className="professional-card">
          <div className="card-header">
            <h4 className="card-title">Tindakan Cepat</h4>
          </div>
          <div className="card-content">
            <div className="space-y-2">
              <button className="w-full btn-secondary text-left">
                <MapPin className="h-4 w-4 mr-2" />
                Lihat Premis Terdekat
              </button>
              <button className="w-full btn-secondary text-left">
                <Navigation className="h-4 w-4 mr-2" />
                Navigasi ke Lokasi
              </button>
            </div>
          </div>
        </div>
      </div>
    </PageTemplate>
  )
}

export default MapView