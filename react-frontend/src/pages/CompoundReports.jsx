import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Search, FileText, History, Download } from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import api from '../services/api';
import LoadingSpinner from '../components/UI/LoadingSpinner';

const CompoundReports = () => {
  const [searchText, setSearchText] = useState('');
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [selectedCompound, setSelectedCompound] = useState(null);
  const [editingCompound, setEditingCompound] = useState(null);
  const [isNewCompound, setIsNewCompound] = useState(false);

  const queryClient = useQueryClient();

  // Fetch compounds data
  const { data: compounds = [], isLoading: compoundsLoading } = useQuery({
    queryKey: ['compounds', searchText, pageIndex, pageSize],
    queryFn: () => api.get('/api/compounds').then(res => res.data),
  });

  // Fetch transaction status
  const { data: transactionStatus = [] } = useQuery({
    queryKey: ['transaction-status'],
    queryFn: () => api.get('/api/transaction-status').then(res => res.data),
  });

  // Mutations
  const updateCompoundMutation = useMutation({
    mutationFn: ({ id, compoundData }) => api.put(`/api/compounds/${id}`, compoundData),
    onSuccess: () => {
      queryClient.invalidateQueries(['compounds']);
      setShowEditModal(false);
      toast.success('Kompaun berjaya dikemaskini');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat mengemaskini kompaun');
    },
  });

  const deleteCompoundMutation = useMutation({
    mutationFn: (id) => api.delete(`/api/compounds/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries(['compounds']);
      setShowDeleteModal(false);
      toast.success('Kompaun berjaya dihapus');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat menghapus kompaun');
    },
  });

  const handleAddCompound = () => {
    setEditingCompound({
      noRujukan: '',
      noLesen: '',
      namaPemilik: '',
      namaPegawai: '',
      amaun: 0,
      statusBayaranId: 0,
    });
    setIsNewCompound(true);
    setShowEditModal(true);
  };

  const handleEditCompound = (compound) => {
    setEditingCompound(compound);
    setIsNewCompound(false);
    setShowEditModal(true);
  };

  const handleDeleteCompound = (compound) => {
    setSelectedCompound(compound);
    setShowDeleteModal(true);
  };

  const handleViewHistory = (compound) => {
    // Navigate to history page
    window.open(`/viewreportperuntukan?norujukan=${compound.noRujukan}`, '_blank');
  };

  const handleDownloadReport = (compound) => {
    // Navigate to report page
    window.open(`/reportkompaunlesen?norujukan=${compound.noRujukan}`, '_blank');
  };

  const handleSaveCompound = (e) => {
    e.preventDefault();
    
    // Validation
    if (editingCompound.statusBayaranId === 0) {
      toast.error('Status tidak sah. Medan perlu diisi.');
      return;
    }

    updateCompoundMutation.mutate({ 
      id: editingCompound.idKompaun, 
      compoundData: editingCompound 
    });
  };

  const handleExportXLSX = async () => {
    try {
      const response = await api.get('/api/compounds/export/xlsx', {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', 'ExportResult.xlsx');
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      
      toast.success('Export XLSX berjaya');
    } catch (error) {
      toast.error('Ralat export XLSX');
    }
  };

  const handleExportXLS = async () => {
    try {
      const response = await api.get('/api/compounds/export/xls', {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', 'ExportResult.xls');
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      
      toast.success('Export XLS berjaya');
    } catch (error) {
      toast.error('Ralat export XLS');
    }
  };

  const handleExportCSV = async () => {
    try {
      const response = await api.get('/api/compounds/export/csv', {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', 'ExportResult.csv');
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      
      toast.success('Export CSV berjaya');
    } catch (error) {
      toast.error('Ralat export CSV');
    }
  };

  const getStatusBadge = (status, statusId) => {
    const statusClasses = {
      1: 'bg-green-100 text-green-800', // Baru
      2: 'bg-yellow-100 text-yellow-800', // Dalam Tindakan
      3: 'bg-gray-100 text-gray-800', // Ditutup
      4: 'bg-orange-100 text-orange-800', // Amaran
      5: 'bg-blue-100 text-blue-800', // Telah Dibayar
      6: 'bg-purple-100 text-purple-800', // Disemak
      7: 'bg-indigo-100 text-indigo-800', // Dipindah
      8: 'bg-red-100 text-red-800', // Dibatalkan
    };

    const className = statusClasses[statusId] || 'bg-gray-100 text-gray-800';
    
    return (
      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${className}`}>
        {status?.toUpperCase()}
      </span>
    );
  };

  const filteredCompounds = compounds.filter(compound =>
    compound.noRujukan?.toLowerCase().includes(searchText.toLowerCase()) ||
    compound.noLesen?.toLowerCase().includes(searchText.toLowerCase()) ||
    compound.namaPemilik?.toLowerCase().includes(searchText.toLowerCase()) ||
    compound.namaPerniagaan?.toLowerCase().includes(searchText.toLowerCase())
  );

  // Check if status is editable (not closed, paid, or cancelled)
  const isStatusEditable = (statusId) => {
    return statusId !== 3 && statusId !== 5 && statusId !== 8;
  };

  if (compoundsLoading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="p-6">
      {/* Header */}
      <div className="flex items-center mb-6 pb-4 border-b border-gray-200">
        <img src="/images/icons-small/document.png" alt="" className="w-6 h-6 mr-3" />
        <h1 className="text-xl font-semibold text-gray-800">Pelaporan - Laporan Kompaun</h1>
      </div>

      {/* Controls */}
      <div className="mb-6 flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="flex gap-2">
          <button
            onClick={handleAddCompound}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            <Plus className="w-4 h-4" />
            Tambah
          </button>
        </div>
        
        <div className="flex gap-2 items-center">
          <div className="relative">
            <Search className="w-4 h-4 absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              placeholder="Masukkan kata carian..."
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              className="pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
      </div>

      {/* Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Tindakan
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Bil.
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  No Rujukan
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  No Lesen
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Nama Pemilik
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Nama Pegawai
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Amaun (RM)
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status Bayaran
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Tarikh Data
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredCompounds.map((compound) => (
                <tr 
                  key={compound.idKompaun} 
                  className={`hover:bg-gray-50 ${compound.statusBayaranId === 1 ? 'bg-green-50' : ''}`}
                >
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleEditCompound(compound)}
                        className="text-blue-600 hover:text-blue-900"
                        title="Edit"
                      >
                        <Edit className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => handleDeleteCompound(compound)}
                        className="text-red-600 hover:text-red-900"
                        title="Hapus"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => handleViewHistory(compound)}
                        className="text-purple-600 hover:text-purple-900"
                        title="Sejarah kompaun"
                      >
                        <History className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => handleDownloadReport(compound)}
                        className="text-green-600 hover:text-green-900"
                        title="Muat turun laporan"
                      >
                        <Download className="w-4 h-4" />
                      </button>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {compound.idKompaun}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {compound.noRujukan}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {compound.noLesen}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {compound.namaPemilik}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {compound.namaPegawai}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 text-right">
                    {new Intl.NumberFormat('ms-MY', {
                      style: 'currency',
                      currency: 'MYR'
                    }).format(compound.amaun)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {getStatusBadge(compound.statusBayaran, compound.statusBayaranId)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {new Date(compound.tarikhData).toLocaleDateString('ms-MY')}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
          <div className="flex-1 flex justify-between sm:hidden">
            <button
              onClick={() => setPageIndex(Math.max(0, pageIndex - 1))}
              disabled={pageIndex === 0}
              className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
            >
              Sebelum
            </button>
            <button
              onClick={() => setPageIndex(pageIndex + 1)}
              className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
            >
              Seterus
            </button>
          </div>
          <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
            <div>
              <p className="text-sm text-gray-700">
                Bil. : <span className="font-medium">{filteredCompounds.length}</span>
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Export Buttons */}
      <div className="mt-6 flex gap-2">
        <button
          onClick={handleExportXLSX}
          className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
        >
          Export XLSX
        </button>
        <button
          onClick={handleExportXLS}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          Export XLS
        </button>
        <button
          onClick={handleExportCSV}
          className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
        >
          Export CSV
        </button>
      </div>

      {/* Edit Modal */}
      {showEditModal && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white max-h-[90vh] overflow-y-auto">
            <div className="mt-3">
              <h3 className="text-lg font-medium text-gray-900 mb-4">
                {isNewCompound ? 'Tambah Kompaun Baru' : 'Edit Kompaun'}
              </h3>
              
              {/* Display compound details */}
              {!isNewCompound && editingCompound && (
                <div className="mb-6 p-4 bg-gray-50 rounded-lg">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                    <div><strong>No Rujukan:</strong> {editingCompound.noRujukan}</div>
                    <div><strong>Tarikh:</strong> {new Date(editingCompound.tarikhData).toLocaleDateString('ms-MY')}</div>
                    <div><strong>No Lesen:</strong> {editingCompound.noLesen || '-'}</div>
                    <div><strong>No Daftar Syarikat:</strong> {editingCompound.ssmNo || '-'}</div>
                    <div><strong>Nama Pemilik:</strong> {editingCompound.namaPemilik}</div>
                    <div><strong>Nama Perniagaan:</strong> {editingCompound.namaPerniagaan || '-'}</div>
                    <div className="md:col-span-2"><strong>Alamat Perniagaan:</strong> {editingCompound.alamatPerniagaan}</div>
                    <div className="md:col-span-2"><strong>Akta Kesalahan:</strong> {editingCompound.aktaKesalahan || '-'}</div>
                    <div className="md:col-span-2"><strong>Kod Kesalahan:</strong> {editingCompound.kodKesalahan || '-'}</div>
                    <div><strong>Nama Pegawai:</strong> {editingCompound.namaPegawai}</div>
                    <div><strong>Lokasi Kesalahan:</strong> {editingCompound.lokasiKesalahan}</div>
                  </div>
                </div>
              )}

              <form onSubmit={handleSaveCompound} className="space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Amaun Kompaun (RM)</label>
                    <input
                      type="number"
                      step="0.01"
                      value={editingCompound?.amaun || 0}
                      onChange={(e) => setEditingCompound({...editingCompound, amaun: parseFloat(e.target.value)})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500"
                      disabled={!isNewCompound}
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Status Bayaran</label>
                    <select
                      value={editingCompound?.statusBayaranId || 0}
                      onChange={(e) => setEditingCompound({...editingCompound, statusBayaranId: parseInt(e.target.value)})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500"
                      disabled={!isNewCompound && !isStatusEditable(editingCompound?.statusBayaranId)}
                      required
                    >
                      <option value={0}>Pilih Status Bayaran...</option>
                      {transactionStatus.map(status => (
                        <option key={status.statusId} value={status.statusId}>
                          {status.statusName}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
                
                <div className="flex justify-end gap-3 pt-4">
                  <button
                    type="button"
                    onClick={() => setShowEditModal(false)}
                    className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                  >
                    Batal
                  </button>
                  <button
                    type="submit"
                    disabled={updateCompoundMutation.isPending}
                    className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
                  >
                    {updateCompoundMutation.isPending ? 'Menyimpan...' : 'Simpan'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {showDeleteModal && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
            <div className="mt-3 text-center">
              <h3 className="text-lg font-medium text-gray-900">Hapus rekod</h3>
              <div className="mt-2 px-7 py-3">
                <p className="text-sm text-gray-500">
                  <strong>{selectedCompound?.noRujukan}</strong> adalah rekod yang dipilih untuk dihapuskan.
                  <br />Anda pasti untuk menghapuskan rekod ini?
                </p>
              </div>
              <div className="flex justify-center gap-3 mt-4">
                <button
                  onClick={() => deleteCompoundMutation.mutate(selectedCompound.idKompaun)}
                  disabled={deleteCompoundMutation.isPending}
                  className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50"
                >
                  {deleteCompoundMutation.isPending ? 'Menghapus...' : 'Ya'}
                </button>
                <button
                  onClick={() => setShowDeleteModal(false)}
                  className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                >
                  Tidak
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CompoundReports;