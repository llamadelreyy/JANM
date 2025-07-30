import React, { useState, useEffect, useRef } from 'react'
import { constituencyData, getPartyColor, formatParty } from '../../data/constituencyData'

const InteractiveMap = ({
  selectedLocation,
  mapProvider,
  onLocationChange,
  districtMappings,
  locationNames
}) => {
  const mapRef = useRef(null)
  const [mapLoaded, setMapLoaded] = useState(false)
  const [geoJsonData, setGeoJsonData] = useState(null)
  const [hoveredDistrict, setHoveredDistrict] = useState(null)
  const [tooltipPosition, setTooltipPosition] = useState({ x: 0, y: 0 })

  // Load GeoJSON data
  useEffect(() => {
    const loadGeoJsonData = async () => {
      try {
        const response = await fetch('/klang_districts.geojson')
        const data = await response.json()
        setGeoJsonData(data)
      } catch (error) {
        console.error('Error loading GeoJSON data:', error)
        // Fallback to embedded data if fetch fails
        setGeoJsonData({
          type: "FeatureCollection",
          features: [
            {
              type: "Feature",
              properties: { Name: "N.42 MERU" },
              geometry: {
                type: "Polygon",
                coordinates: [[
                  [101.469678, 3.193228], [101.472415, 3.192311], [101.473191, 3.191974],
                  [101.473780, 3.191719], [101.476846, 3.19132], [101.479079, 3.191353],
                  [101.479757, 3.19159], [101.480701, 3.192021], [101.482259, 3.19195],
                  [101.482990, 3.192469], [101.483668, 3.192973], [101.483917, 3.193102],
                  [101.484797, 3.192137], [101.485014, 3.1919], [101.485219, 3.191959],
                  [101.485495, 3.192038], [101.485567, 3.192058], [101.485678, 3.192035],
                  [101.486727, 3.191819], [101.487183, 3.192074], [101.487538, 3.192273],
                  [101.488439, 3.19199], [101.488919, 3.191839], [101.488986, 3.191818],
                  [101.469678, 3.193228]
                ]]
              }
            },
            {
              type: "Feature",
              properties: { Name: "N.43 SEMENTA" },
              geometry: {
                type: "Polygon",
                coordinates: [[
                  [101.421611, 3.155972], [101.421248, 3.15597], [101.421067, 3.155965],
                  [101.420702, 3.155962], [101.42034, 3.155961], [101.420300, 3.155961],
                  [101.419975, 3.155961], [101.419613, 3.15596], [101.419436, 3.15596],
                  [101.419252, 3.15596], [101.418890, 3.155959], [101.418882, 3.155959],
                  [101.418528, 3.155959], [101.418166, 3.155958], [101.417805, 3.155958],
                  [101.417446, 3.155958], [101.417084, 3.155957], [101.416723, 3.155957],
                  [101.416361, 3.155956], [101.416024, 3.155955], [101.416008, 3.155955],
                  [101.421611, 3.155972]
                ]]
              }
            }
          ]
        })
      }
    }
    loadGeoJsonData()
  }, [])

  // Extract district boundaries from GeoJSON data
  const getDistrictBoundaries = () => {
    if (!geoJsonData) return {}
    
    const boundaries = {}
    
    geoJsonData.features.forEach(feature => {
      const name = feature.properties.Name
      let districtKey = ''
      
      // Map district names to keys
      if (name.includes('MERU')) districtKey = 'meru'
      else if (name.includes('SEMENTA')) districtKey = 'sementa'
      else if (name.includes('SELAT KLANG')) districtKey = 'selat-klang'
      else if (name.includes('BANDAR BARU KLANG')) districtKey = 'bandar-baru-klang'
      else if (name.includes('PELABUHAN KLANG')) districtKey = 'pelabuhan-klang'
      else if (name.includes('PANDAMARAN')) districtKey = 'pandamaran'
      else if (name.includes('SENTOSA')) districtKey = 'sentosa'
      else if (name.includes('SUNGAI KANDIS')) districtKey = 'sungai-kandis'
      
      if (districtKey && feature.geometry) {
        if (feature.geometry.type === 'Polygon') {
          // Single polygon - use the outer ring
          boundaries[districtKey] = feature.geometry.coordinates[0]
        } else if (feature.geometry.type === 'MultiPolygon') {
          // Multiple polygons - use the largest one
          const polygons = feature.geometry.coordinates
          const largestPolygon = polygons.reduce((largest, current) => {
            return current[0].length > largest[0].length ? current : largest
          }, polygons[0])
          boundaries[districtKey] = largestPolygon[0]
        }
      }
    })
    
    return boundaries
  }

  const districtBoundaries = getDistrictBoundaries()

  const districtInfo = districtMappings[selectedLocation] || districtMappings['telok-gong']

  // Calculate overall bounds from all district boundaries
  const calculateOverallBounds = () => {
    if (!geoJsonData || Object.keys(districtBoundaries).length === 0) {
      return {
        minLng: 101.35,
        maxLng: 101.52,
        minLat: 2.95,
        maxLat: 3.20
      }
    }

    let minLng = Infinity, maxLng = -Infinity
    let minLat = Infinity, maxLat = -Infinity

    Object.values(districtBoundaries).forEach(coordinates => {
      coordinates.forEach(([lng, lat]) => {
        minLng = Math.min(minLng, lng)
        maxLng = Math.max(maxLng, lng)
        minLat = Math.min(minLat, lat)
        maxLat = Math.max(maxLat, lat)
      })
    })

    // Add padding
    const lngPadding = (maxLng - minLng) * 0.1
    const latPadding = (maxLat - minLat) * 0.1

    return {
      minLng: minLng - lngPadding,
      maxLng: maxLng + lngPadding,
      minLat: minLat - latPadding,
      maxLat: maxLat + latPadding
    }
  }

  // Generate map URLs with district boundaries
  const getMapUrl = () => {
    const bounds = calculateOverallBounds()
    const centerLng = (bounds.minLng + bounds.maxLng) / 2
    const centerLat = (bounds.minLat + bounds.maxLat) / 2

    if (mapProvider === 'googlemaps') {
      // Google Maps embed with custom styling
      return `https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d15955.${Math.floor(Math.random() * 1000)}!2d${centerLng}!3d${centerLat}!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2z${encodeURIComponent('Klang Districts')}!5e0!3m2!1sen!2smy!4v${Date.now()}!5m2!1sen!2smy`
    } else {
      // OpenStreetMap with district boundaries
      return `https://www.openstreetmap.org/export/embed.html?bbox=${bounds.minLng},${bounds.minLat},${bounds.maxLng},${bounds.maxLat}&layer=mapnik&marker=${centerLat},${centerLng}`
    }
  }

  // Handle mouse events for tooltip
  const handleMouseEnter = (districtKey, event) => {
    setHoveredDistrict(districtKey)
    const rect = mapRef.current.getBoundingClientRect()
    setTooltipPosition({
      x: event.clientX - rect.left,
      y: event.clientY - rect.top
    })
  }

  const handleMouseMove = (event) => {
    if (hoveredDistrict) {
      const rect = mapRef.current.getBoundingClientRect()
      setTooltipPosition({
        x: event.clientX - rect.left,
        y: event.clientY - rect.top
      })
    }
  }

  const handleMouseLeave = () => {
    setHoveredDistrict(null)
  }

  // Create SVG overlay for district boundaries
  const createDistrictOverlay = () => {
    if (!mapRef.current || !geoJsonData || Object.keys(districtBoundaries).length === 0) return null

    const mapRect = mapRef.current.getBoundingClientRect()
    const mapWidth = mapRect.width
    const mapHeight = mapRect.height

    // Calculate bounds for coordinate conversion
    const bounds = calculateOverallBounds()

    // Convert lat/lng to pixel coordinates
    const coordToPixel = (lng, lat) => {
      const x = ((lng - bounds.minLng) / (bounds.maxLng - bounds.minLng)) * mapWidth
      const y = ((bounds.maxLat - lat) / (bounds.maxLat - bounds.minLat)) * mapHeight
      return [x, y]
    }

    return (
      <svg
        className="absolute inset-0 z-10"
        width={mapWidth}
        height={mapHeight}
        style={{ top: 0, left: 0, pointerEvents: 'none' }}
        onMouseMove={handleMouseMove}
        onMouseLeave={handleMouseLeave}
      >
        {Object.entries(districtBoundaries).map(([districtKey, coordinates]) => {
          const isSelected = districtKey === selectedLocation
          const isHovered = districtKey === hoveredDistrict
          const constituency = constituencyData[districtKey]
          const partyColor = constituency ? getPartyColor(constituency.representative.party) : '#64748B'
          
          const pixelCoords = coordinates.map(([lng, lat]) => coordToPixel(lng, lat))
          const pathData = `M ${pixelCoords.map(([x, y]) => `${x},${y}`).join(' L ')} Z`

          return (
            <g key={districtKey}>
              {/* District boundary */}
              <path
                d={pathData}
                fill={
                  isSelected
                    ? 'rgba(59, 130, 246, 0.4)'
                    : isHovered
                      ? `${partyColor}40`
                      : 'rgba(148, 163, 184, 0.2)'
                }
                stroke={
                  isSelected
                    ? '#3B82F6'
                    : isHovered
                      ? partyColor
                      : '#64748B'
                }
                strokeWidth={isSelected ? 3 : isHovered ? 2.5 : 2}
                strokeDasharray={isSelected ? 'none' : '5,5'}
                className="transition-all duration-300 cursor-pointer"
                style={{ pointerEvents: 'all' }}
                onClick={() => onLocationChange(districtKey)}
                onMouseEnter={(e) => handleMouseEnter(districtKey, e)}
              />
              
              {/* District label */}
              <text
                x={pixelCoords.reduce((sum, [x]) => sum + x, 0) / pixelCoords.length}
                y={pixelCoords.reduce((sum, [, y]) => sum + y, 0) / pixelCoords.length}
                textAnchor="middle"
                dominantBaseline="middle"
                className={`text-xs font-semibold pointer-events-none ${
                  isSelected ? 'fill-blue-700' : isHovered ? 'fill-slate-800' : 'fill-slate-600'
                }`}
                style={{ textShadow: '1px 1px 2px rgba(255,255,255,0.8)' }}
              >
                {locationNames[districtKey]}
              </text>
            </g>
          )
        })}
      </svg>
    )
  }

  // Create tooltip component
  const createTooltip = () => {
    if (!hoveredDistrict || !constituencyData[hoveredDistrict]) return null

    const constituency = constituencyData[hoveredDistrict]
    const partyColor = getPartyColor(constituency.representative.party)

    return (
      <div
        className="absolute z-30 bg-white rounded-lg shadow-xl border border-gray-200 p-4 max-w-sm pointer-events-none"
        style={{
          left: tooltipPosition.x + 10,
          top: tooltipPosition.y - 10,
          transform: tooltipPosition.x > 400 ? 'translateX(-100%)' : 'none'
        }}
      >
        <div className="space-y-3">
          {/* Header */}
          <div className="border-b border-gray-200 pb-2">
            <h3 className="font-bold text-lg text-gray-900">
              {constituency.code} - {constituency.name}
            </h3>
          </div>

          {/* Representative Info */}
          <div className="space-y-2">
            <div>
              <span className="text-sm font-medium text-gray-600">Ahli Majlis:</span>
              <p className="text-gray-900 font-semibold">{constituency.representative.name}</p>
            </div>
            
            <div>
              <span className="text-sm font-medium text-gray-600">Parti:</span>
              <p className="text-gray-900 font-medium">
                {formatParty(constituency.representative.party)}
              </p>
            </div>

            <div>
              <span className="text-sm font-medium text-gray-600">Penggal:</span>
              <p className="text-gray-900">{constituency.term}</p>
            </div>
          </div>

          {/* Area Info */}
          <div className="border-t border-gray-200 pt-2">
            <div>
              <span className="text-sm font-medium text-gray-600">Kawasan:</span>
              <p className="text-gray-900 text-sm">{constituency.area.description}</p>
            </div>
            
            {constituency.localities.length > 0 && (
              <div className="mt-2">
                <span className="text-sm font-medium text-gray-600">Lokaliti Utama:</span>
                <div className="flex flex-wrap gap-1 mt-1">
                  {constituency.localities.slice(0, 3).map((locality, index) => (
                    <span
                      key={index}
                      className="text-xs bg-gray-100 text-gray-700 px-2 py-1 rounded"
                    >
                      {locality}
                    </span>
                  ))}
                  {constituency.localities.length > 3 && (
                    <span className="text-xs text-gray-500">
                      +{constituency.localities.length - 3} lagi
                    </span>
                  )}
                </div>
              </div>
            )}
          </div>

          {/* Compliance Status */}
          {constituency.compliance && (
            <div className="border-t border-gray-200 pt-3">
              <div className="space-y-3">
                <div>
                  <span className="text-sm font-medium text-gray-600">Status Pematuhan Lesen:</span>
                  <div className="mt-1 flex items-center justify-between">
                    <div className="flex items-center space-x-2">
                      <span className="text-sm text-green-600 font-medium">
                        {constituency.compliance.license.compliant} Patuh
                      </span>
                      <span className="text-sm text-red-600 font-medium">
                        {constituency.compliance.license.nonCompliant} Tidak Patuh
                      </span>
                    </div>
                    <span className="text-sm font-bold text-blue-600">
                      {constituency.compliance.license.percentage.toFixed(1)}%
                    </span>
                  </div>
                </div>

                <div>
                  <span className="text-sm font-medium text-gray-600">Status Pematuhan Cukai:</span>
                  <div className="mt-1 flex items-center justify-between">
                    <div className="flex items-center space-x-2">
                      <span className="text-sm text-green-600 font-medium">
                        {constituency.compliance.tax.compliant} Patuh
                      </span>
                      <span className="text-sm text-red-600 font-medium">
                        {constituency.compliance.tax.nonCompliant} Tidak Patuh
                      </span>
                    </div>
                    <span className="text-sm font-bold text-blue-600">
                      {constituency.compliance.tax.percentage.toFixed(1)}%
                    </span>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    )
  }

  useEffect(() => {
    const timer = setTimeout(() => setMapLoaded(true), 1000)
    return () => clearTimeout(timer)
  }, [selectedLocation, mapProvider])

  return (
    <div className="relative w-full h-[600px] rounded-lg overflow-hidden bg-slate-100">
      {/* Base Map */}
      <iframe
        ref={mapRef}
        key={`${mapProvider}-${selectedLocation}`}
        src={getMapUrl()}
        width="100%"
        height="100%"
        style={{ border: 0 }}
        allowFullScreen=""
        loading="lazy"
        referrerPolicy="no-referrer-when-downgrade"
        title={`Peta ${locationNames[selectedLocation]} - ${mapProvider === 'openstreetmap' ? 'OpenStreetMap' : 'Google Maps'}`}
        onLoad={() => setMapLoaded(true)}
        className="relative z-0"
      />

      {/* District Boundaries Overlay */}
      {mapLoaded && mapProvider === 'openstreetmap' && createDistrictOverlay()}

      {/* Constituency Tooltip */}
      {mapLoaded && mapProvider === 'openstreetmap' && createTooltip()}

      {/* District Selection Overlay */}
      <div className="absolute top-4 right-4 z-20">
        <div className="bg-white rounded-lg shadow-lg p-3 max-w-xs">
          <h4 className="font-semibold text-slate-900 mb-2 text-sm">Pilih Kawasan:</h4>
          <div className="grid grid-cols-2 gap-1">
            {Object.entries(locationNames)
              .filter(([key]) => districtBoundaries[key]) // Only show districts with actual coordinates
              .map(([key, name]) => (
                <button
                  key={key}
                  onClick={() => onLocationChange(key)}
                  className={`px-2 py-1 text-xs rounded transition-colors ${
                    selectedLocation === key
                      ? 'bg-blue-600 text-white'
                      : 'bg-slate-100 text-slate-700 hover:bg-slate-200'
                  }`}
                >
                  {name}
                </button>
              ))}
          </div>
        </div>
      </div>

      {/* Legend */}
      <div className="absolute bottom-4 left-4 z-20">
        <div className="bg-white rounded-lg shadow-lg p-3">
          <h4 className="font-semibold text-slate-900 mb-2 text-sm">Legenda:</h4>
          <div className="space-y-1">
            <div className="flex items-center space-x-2">
              <div className="w-4 h-3 bg-blue-400 bg-opacity-50 border-2 border-blue-600 rounded"></div>
              <span className="text-xs text-slate-700">Kawasan Terpilih</span>
            </div>
            <div className="flex items-center space-x-2">
              <div className="w-4 h-3 bg-slate-300 bg-opacity-50 border-2 border-slate-500 border-dashed rounded"></div>
              <span className="text-xs text-slate-700">Kawasan Lain</span>
            </div>
          </div>
        </div>
      </div>

      {/* Loading Overlay */}
      {!mapLoaded && (
        <div className="absolute inset-0 bg-slate-100 flex items-center justify-center z-30">
          <div className="flex flex-col items-center space-y-3">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            <p className="text-sm text-slate-600">Memuatkan peta...</p>
          </div>
        </div>
      )}
    </div>
  )
}

export default InteractiveMap