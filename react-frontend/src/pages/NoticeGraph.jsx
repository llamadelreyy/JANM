import React, { useState, useEffect } from 'react';
import { ArrowLeft } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import LoadingSpinner from '../components/UI/LoadingSpinner';

const NoticeGraph = () => {
  const navigate = useNavigate();
  const currentYear = new Date().getFullYear();

  // Fetch notice graph data
  const { data: chartData = [], isLoading } = useQuery({
    queryKey: ['notice-graph', currentYear],
    queryFn: async () => {
      const response = await api.get('/api/Dashboard/GetTotalNoticeGraph');
      
      // Define month names mapping
      const monthNames = {
        1: 'JAN', 2: 'FEB', 3: 'MAC',
        4: 'APR', 5: 'MEI', 6: 'JUN',
        7: 'JUL', 8: 'OGO', 9: 'SEP',
        10: 'OKT', 11: 'NOV', 12: 'DIS'
      };

      // Transform the data
      const transformedData = response.data.map(item => ({
        month: monthNames[item.month],
        total: item.count,
        monthNumber: item.month
      }));

      // Ensure all 12 months are present
      const completeData = [];
      for (let i = 1; i <= 12; i++) {
        const existingData = transformedData.find(item => item.monthNumber === i);
        completeData.push({
          month: monthNames[i],
          total: existingData ? existingData.total : 0,
          monthNumber: i
        });
      }

      return completeData;
    },
  });

  const handleGoBack = () => {
    navigate('/home');
  };

  // Custom tooltip for the chart
  const CustomTooltip = ({ active, payload, label }) => {
    if (active && payload && payload.length) {
      return (
        <div className="bg-white p-3 border border-gray-300 rounded-lg shadow-lg">
          <p className="font-medium">{`${label} : ${payload[0].value.toLocaleString()}`}</p>
        </div>
      );
    }
    return null;
  };

  if (isLoading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="p-6">
      {/* Header with Navigation */}
      <div className="flex items-center mb-6 pb-4 border-b border-gray-200">
        <button
          onClick={handleGoBack}
          className="flex items-center text-blue-600 hover:text-blue-800 mr-4"
        >
          <ArrowLeft className="w-5 h-5 mr-1" />
        </button>
        <div className="flex items-center">
          <span className="text-blue-600 hover:text-blue-800 cursor-pointer" onClick={handleGoBack}>
            Ringkasan Eksekutif
          </span>
          <span className="mx-2 text-gray-400">/</span>
          <span className="text-gray-600">Perincian Notis</span>
        </div>
      </div>

      {/* Chart Container */}
      <div className="bg-white rounded-lg shadow-lg p-6">
        <div className="mb-6">
          <h2 className="text-xl font-semibold text-gray-800 text-center">
            Jumlah Notis Mengikut Bulan Pada Tahun {currentYear}
          </h2>
        </div>

        <div className="w-full professional-chart" style={{ height: '500px' }}>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart
              data={chartData}
              margin={{
                top: 20,
                right: 30,
                left: 20,
                bottom: 5,
              }}
              barCategoryGap="20%"
            >
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis
                dataKey="month"
                axisLine={false}
                tickLine={false}
                tick={{ fontSize: 12, fill: '#64748b', fontFamily: 'Inter', fontWeight: 500 }}
              />
              <YAxis
                axisLine={false}
                tickLine={false}
                tick={{ fontSize: 12, fill: '#64748b', fontFamily: 'Inter', fontWeight: 500 }}
                tickFormatter={(value) => value.toLocaleString()}
              />
              <Tooltip
                content={<CustomTooltip />}
                cursor={{ fill: 'rgba(0, 0, 0, 0.05)' }}
              />
              <Bar
                dataKey="total"
                fill="#3b82f6"
                radius={[6, 6, 0, 0]}
                maxBarSize={50}
              />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Summary Statistics */}
        <div className="mt-6 grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="bg-gradient-to-br from-green-50 to-green-100 p-6 rounded-xl border border-green-200 shadow-sm">
            <div className="text-sm text-green-700 font-semibold uppercase tracking-wide">Jumlah Notis</div>
            <div className="text-3xl professional-number text-green-800 mt-2">
              {chartData.reduce((sum, item) => sum + item.total, 0).toLocaleString()}
            </div>
          </div>
          
          <div className="bg-gradient-to-br from-blue-50 to-blue-100 p-6 rounded-xl border border-blue-200 shadow-sm">
            <div className="text-sm text-blue-700 font-semibold uppercase tracking-wide">Purata Bulanan</div>
            <div className="text-3xl professional-number text-blue-800 mt-2">
              {Math.round(chartData.reduce((sum, item) => sum + item.total, 0) / 12).toLocaleString()}
            </div>
          </div>
          
          <div className="bg-gradient-to-br from-purple-50 to-purple-100 p-6 rounded-xl border border-purple-200 shadow-sm">
            <div className="text-sm text-purple-700 font-semibold uppercase tracking-wide">Bulan Tertinggi</div>
            <div className="text-3xl professional-number text-purple-800 mt-2">
              {chartData.reduce((max, item) => item.total > max.total ? item : max, chartData[0])?.month || '-'}
            </div>
          </div>
        </div>

        {/* Monthly Breakdown Table */}
        <div className="mt-8">
          <h3 className="text-lg font-semibold text-gray-800 mb-4">Pecahan Mengikut Bulan</h3>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Bulan
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Jumlah Notis
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Peratus (%)
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {chartData.map((item, index) => {
                  const totalNotices = chartData.reduce((sum, data) => sum + data.total, 0);
                  const percentage = totalNotices > 0 ? ((item.total / totalNotices) * 100).toFixed(1) : 0;
                  
                  return (
                    <tr key={index} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        {item.month}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {item.total.toLocaleString()}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {percentage}%
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NoticeGraph;