import React, { useState } from 'react';
import { Plus, Edit, Trash2, Search, Building2 } from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import api from '../services/api';
import LoadingSpinner from '../components/UI/LoadingSpinner';

const Unit = () => {
  const [searchText, setSearchText] = useState('');
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [selectedUnit, setSelectedUnit] = useState(null);
  const [editingUnit, setEditingUnit] = useState(null);
  const [isNewUnit, setIsNewUnit] = useState(false);

  const queryClient = useQueryClient();

  // Fetch units data
  const { data: units = [], isLoading: unitsLoading } = useQuery({
    queryKey: ['units', searchText],
    queryFn: () => api.get('/api/units').then(res => res.data),
    onError: () => {
      // Mock data for development
      return [
        { 
          unit_id: 1, 
          unit_name: 'Unit Perancangan Strategik', 
          unit_desc: 'Menguruskan perancangan strategik jabatan',
          dept_id: 1,
          dept_name: 'Jabatan Perancangan Bandar',
          div_id: 1,
          div_name: 'Bahagian Perancangan',
          createdAt: new Date() 
        },
        { 
          unit_id: 2, 
          unit_name: 'Unit Pemprosesan Lesen', 
          unit_desc: 'Memproses permohonan lesen perniagaan',
          dept_id: 2,
          dept_name: 'Jabatan Lesen',
          div_id: 2,
          div_name: 'Bahagian Lesen',
          createdAt: new Date() 
        },
      ];
    }
  });

  // Fetch departments
  const { data: departments = [] } = useQuery({
    queryKey: ['departments'],
    queryFn: () => api.get('/api/departments').then(res => res.data),
    onError: () => [
      { dept_id: 1, dept_name: 'Jabatan Perancangan Bandar' },
      { dept_id: 2, dept_name: 'Jabatan Lesen' },
    ]
  });

  // Fetch sections
  const { data: sections = [] } = useQuery({
    queryKey: ['sections'],
    queryFn: () => api.get('/api/sections').then(res => res.data),
    onError: () => [
      { div_id: 1, div_name: 'Bahagian Perancangan', dept_id: 1 },
      { div_id: 2, div_name: 'Bahagian Lesen', dept_id: 2 },
    ]
  });

  // Mutations
  const addUnitMutation = useMutation({
    mutationFn: (unitData) => api.post('/api/units', unitData),
    onSuccess: () => {
      queryClient.invalidateQueries(['units']);
      setShowEditModal(false);
      toast.success('Unit berjaya ditambah');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat menambah unit');
    },
  });

  const updateUnitMutation = useMutation({
    mutationFn: ({ id, unitData }) => api.put(`/api/units/${id}`, unitData),
    onSuccess: () => {
      queryClient.invalidateQueries(['units']);
      setShowEditModal(false);
      toast.success('Unit berjaya dikemaskini');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat mengemaskini unit');
    },
  });

  const deleteUnitMutation = useMutation({
    mutationFn: (id) => api.delete(`/api/units/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries(['units']);
      setShowDeleteModal(false);
      toast.success('Unit berjaya dihapus');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat menghapus unit');
    },
  });

  const handleAddUnit = () => {
    setEditingUnit({
      unit_name: '',
      unit_desc: '',
      dept_id: 0,
      div_id: 0,
    });
    setIsNewUnit(true);
    setShowEditModal(true);
  };

  const handleEditUnit = (unit) => {
    setEditingUnit(unit);
    setIsNewUnit(false);
    setShowEditModal(true);
  };

  const handleDeleteUnit = (unit) => {
    setSelectedUnit(unit);
    setShowDeleteModal(true);
  };

  const handleSaveUnit = (e) => {
    e.preventDefault();
    
    if (!editingUnit.unit_name || editingUnit.unit_name.trim() === '') {
      toast.error('Nama Unit perlu diisi');
      return;
    }
    
    if (!editingUnit.unit_desc || editingUnit.unit_desc.trim() === '') {
      toast.error('Keterangan Unit perlu diisi');
      return;
    }

    if (editingUnit.dept_id === 0) {
      toast.error('Sila pilih Jabatan');
      return;
    }

    if (editingUnit.div_id === 0) {
      toast.error('Sila pilih Seksyen');
      return;
    }

    if (isNewUnit) {
      addUnitMutation.mutate(editingUnit);
    } else {
      updateUnitMutation.mutate({ id: editingUnit.unit_id, unitData: editingUnit });
    }
  };

  const filteredUnits = units.filter(unit =>
    unit.unit_name?.toLowerCase().includes(searchText.toLowerCase()) ||
    unit.unit_desc?.toLowerCase().includes(searchText.toLowerCase()) ||
    unit.dept_name?.toLowerCase().includes(searchText.toLowerCase()) ||
    unit.div_name?.toLowerCase().includes(searchText.toLowerCase())
  );

  const filteredSections = sections.filter(section => 
    section.dept_id === editingUnit?.dept_id
  );

  if (unitsLoading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="p-6">
      {/* Header */}
      <div className="flex items-center mb-6 pb-4 border-b border-gray-200">
        <div className="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center mr-3">
          <Building2 className="w-5 h-5 text-purple-600" />
        </div>
        <h1 className="text-xl font-semibold text-gray-800">Pengurusan Unit</h1>
      </div>

      {/* Controls */}
      <div className="mb-6 flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="flex gap-2">
          <button
            onClick={handleAddUnit}
            className="flex items-center gap-2 px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
          >
            <Plus className="w-4 h-4" />
            Tambah Unit
          </button>
        </div>
        
        <div className="flex gap-2 items-center">
          <div className="relative">
            <Search className="w-4 h-4 absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              placeholder="Cari unit..."
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              className="pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
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
                  Nama Unit
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Jabatan
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Seksyen
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Keterangan
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Tarikh Data
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredUnits.map((unit) => (
                <tr key={unit.unit_id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleEditUnit(unit)}
                        className="text-purple-600 hover:text-purple-900"
                        title="Edit"
                      >
                        <Edit className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => handleDeleteUnit(unit)}
                        className="text-red-600 hover:text-red-900"
                        title="Hapus"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {unit.unit_name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {unit.dept_name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {unit.div_name}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-900">
                    {unit.unit_desc}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {new Date(unit.createdAt).toLocaleDateString('ms-MY')}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Summary */}
        <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
          <div className="flex-1 flex justify-between sm:hidden">
            <span className="text-sm text-gray-700">
              Bil. : <span className="font-medium">{filteredUnits.length}</span>
            </span>
          </div>
          <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
            <div>
              <p className="text-sm text-gray-700">
                Bil. : <span className="font-medium">{filteredUnits.length}</span>
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Edit Modal */}
      {showEditModal && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white">
            <div className="mt-3">
              <h3 className="text-lg font-medium text-gray-900 mb-4">
                {isNewUnit ? 'Tambah Unit Baru' : 'Edit Unit'}
              </h3>
              <form onSubmit={handleSaveUnit} className="space-y-4">
                <div className="grid grid-cols-1 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Nama Unit</label>
                    <input
                      type="text"
                      value={editingUnit?.unit_name || ''}
                      onChange={(e) => setEditingUnit({...editingUnit, unit_name: e.target.value})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-purple-500 focus:border-purple-500"
                      placeholder="Nama unit..."
                      required
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">Jabatan</label>
                    <select
                      value={editingUnit?.dept_id || 0}
                      onChange={(e) => setEditingUnit({...editingUnit, dept_id: parseInt(e.target.value), div_id: 0})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-purple-500 focus:border-purple-500"
                      required
                    >
                      <option value={0}>Pilih Jabatan...</option>
                      {departments.map(dept => (
                        <option key={dept.dept_id} value={dept.dept_id}>{dept.dept_name}</option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">Seksyen</label>
                    <select
                      value={editingUnit?.div_id || 0}
                      onChange={(e) => setEditingUnit({...editingUnit, div_id: parseInt(e.target.value)})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-purple-500 focus:border-purple-500"
                      required
                    >
                      <option value={0}>Pilih Seksyen...</option>
                      {filteredSections.map(section => (
                        <option key={section.div_id} value={section.div_id}>{section.div_name}</option>
                      ))}
                    </select>
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Keterangan</label>
                    <textarea
                      value={editingUnit?.unit_desc || ''}
                      onChange={(e) => setEditingUnit({...editingUnit, unit_desc: e.target.value})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-purple-500 focus:border-purple-500"
                      rows="3"
                      placeholder="Keterangan unit..."
                      required
                    />
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
                    disabled={addUnitMutation.isPending || updateUnitMutation.isPending}
                    className="px-4 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700 disabled:opacity-50"
                  >
                    {addUnitMutation.isPending || updateUnitMutation.isPending ? 'Menyimpan...' : 'Simpan'}
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
                  <strong>{selectedUnit?.unit_name}</strong> adalah rekod yang dipilih untuk dihapuskan.
                  <br />Anda pasti untuk menghapuskan rekod ini?
                </p>
              </div>
              <div className="flex justify-center gap-3 mt-4">
                <button
                  onClick={() => deleteUnitMutation.mutate(selectedUnit.unit_id)}
                  disabled={deleteUnitMutation.isPending}
                  className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50"
                >
                  {deleteUnitMutation.isPending ? 'Menghapus...' : 'Ya'}
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

export default Unit;