import React, { useState, useEffect } from 'react'
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts'

const AnimatedRingChart = ({
  data,
  title,
  total,
  centerText,
  animationDuration = 1500
}) => {
  const [animatedData, setAnimatedData] = useState([])
  const [isAnimating, setIsAnimating] = useState(true)

  useEffect(() => {
    // Start animation
    setIsAnimating(true)
    
    // Initialize with zero values
    const initialData = data.map(item => ({ ...item, value: 0, animatedValue: 0 }))
    setAnimatedData(initialData)

    // Animate to final values
    const animationSteps = 60
    const stepDuration = animationDuration / animationSteps
    
    let currentStep = 0
    const interval = setInterval(() => {
      currentStep++
      const progress = currentStep / animationSteps
      const easeOutQuart = 1 - Math.pow(1 - progress, 4) // Easing function
      
      const newData = data.map(item => ({
        ...item,
        animatedValue: Math.round(item.value * easeOutQuart)
      }))
      
      setAnimatedData(newData)
      
      if (currentStep >= animationSteps) {
        clearInterval(interval)
        setIsAnimating(false)
        setAnimatedData(data.map(item => ({ ...item, animatedValue: item.value })))
      }
    }, stepDuration)

    return () => clearInterval(interval)
  }, [data, animationDuration])

  const CustomTooltip = ({ active, payload }) => {
    if (active && payload && payload.length) {
      const data = payload[0].payload
      return (
        <div className="bg-white p-3 rounded-lg shadow-xl border border-slate-200 z-50">
          <p className="font-medium text-slate-900">{data.name}</p>
          <p className="text-sm text-slate-600">
            Jumlah: <span className="font-semibold text-slate-900">{data.animatedValue.toLocaleString()}</span>
          </p>
          <p className="text-sm text-slate-600">
            Peratusan: <span className="font-semibold text-slate-900">{((data.animatedValue / total) * 100).toFixed(1)}%</span>
          </p>
        </div>
      )
    }
    return null
  }

  return (
    <div className="w-full h-full">
      {/* Main Card - Fixed dimensions with proper containment */}
      <div className="relative bg-white rounded-2xl shadow-xl border border-slate-200 h-[420px] w-full overflow-hidden transform transition-transform duration-150 hover:scale-[1.02] hover:shadow-2xl">
        {/* Content Container with padding */}
        <div className="p-6 h-full flex flex-col">
          {/* Header - Fixed height */}
          <div className="text-center mb-4 flex-shrink-0">
            <h3 className="text-lg font-bold text-slate-900 mb-1 truncate">{title}</h3>
            <p className="text-sm text-slate-600">Jumlah: {total.toLocaleString()}</p>
          </div>

          {/* Chart Container - Flexible height */}
          <div className="relative flex justify-center items-center flex-1 min-h-0 mb-4">
            <div className="w-[200px] h-[200px]">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={animatedData}
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    innerRadius={55}
                    dataKey="animatedValue"
                    startAngle={90}
                    endAngle={450}
                    animationBegin={0}
                    animationDuration={animationDuration}
                  >
                    {animatedData.map((entry, index) => (
                      <Cell
                        key={`cell-${index}`}
                        fill={entry.color}
                        stroke={entry.color}
                        strokeWidth={2}
                        className="drop-shadow-sm hover:opacity-80 transition-opacity duration-150"
                      />
                    ))}
                  </Pie>
                  <Tooltip content={<CustomTooltip />} />
                </PieChart>
              </ResponsiveContainer>
            </div>

            {/* Center Text */}
            <div className="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
              <div className="text-center bg-white rounded-full p-2 shadow-lg border-2 border-slate-100">
                <div className="text-lg font-bold text-slate-900">{total.toLocaleString()}</div>
                <div className="text-xs text-slate-600 font-medium">{centerText}</div>
              </div>
            </div>
          </div>

          {/* Legend - Fixed height at bottom */}
          <div className="flex-shrink-0 space-y-1 max-h-[120px] overflow-y-auto">
            {animatedData.map((entry, index) => (
              <div key={index} className="flex items-center justify-between p-2 rounded-lg bg-slate-50 hover:bg-slate-100 transition-colors duration-150">
                <div className="flex items-center space-x-2 flex-1 min-w-0">
                  <div
                    className="w-3 h-3 rounded-full shadow-sm flex-shrink-0"
                    style={{ backgroundColor: entry.color }}
                  ></div>
                  <span className="text-xs font-medium text-slate-700 truncate">{entry.name}</span>
                </div>
                <div className="text-right flex-shrink-0 ml-2">
                  <div className="text-xs font-bold text-slate-900">
                    {isAnimating ? (
                      <span className="inline-block animate-pulse">
                        {entry.animatedValue.toLocaleString()}
                      </span>
                    ) : (
                      entry.animatedValue.toLocaleString()
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Loading Animation Overlay */}
        {isAnimating && (
          <div className="absolute inset-0 bg-white bg-opacity-70 rounded-2xl flex items-center justify-center z-10">
            <div className="flex space-x-1">
              <div className="w-2 h-2 bg-blue-500 rounded-full animate-bounce"></div>
              <div className="w-2 h-2 bg-blue-500 rounded-full animate-bounce" style={{ animationDelay: '0.1s' }}></div>
              <div className="w-2 h-2 bg-blue-500 rounded-full animate-bounce" style={{ animationDelay: '0.2s' }}></div>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}

export default AnimatedRingChart