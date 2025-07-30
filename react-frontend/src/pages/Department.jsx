import React, { useState } from 'react';
import { Plus, Edit, Trash2, Search, Building } from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import api from '../services/api';
import LoadingSpinner from '../components/UI/LoadingSpinner';

const Department = () => {
  const [searchText, setSearchText] = useState('');
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [selectedDepartment, setSelectedDepartment] = useState(null);
  const [editingDepartment, setEditingDepartment] = useState(null);
  const [isNewDepartment, setIsNewDepartment] = useState(false);

  const queryClient = useQueryClient();

  // Fetch departments data
  const { data: departments = [], isLoading } = useQuery({
    queryKey: ['departments', searchText],
    queryFn: () => api.get('/api/departments').then(res => res.data),
    onError: () => {
      // Mock data for development
      return [
        { dept_id: 1, dept_name: 'Jabatan Perancangan Bandar', dept_desc: 'Menguruskan perancangan dan pembangunan bandar', createdAt: new Date() },
        { dept_id: 2, dept_name: 'Jabatan Lesen', dept_desc: 'Menguruskan permohonan dan pengeluaran lesen', createdAt: new Date() },
        { dept_id: 3, dept_name: 'Jabatan Cukai', dept_desc: 'Menguruskan kutipan cukai dan taksiran', createdAt: new Date() },
      ];
    }
  });

  // Mutations
  const addDepartmentMutation = useMutation({
    mutationFn: (deptData) => api.post('/api/departments', deptData),
    onSuccess: () => {
      queryClient.invalidateQueries(['departments']);
      setShowEditModal(false);
      toast.success('Jabatan berjaya ditambah');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat menambah jabatan');
    },
  });

  const updateDepartmentMutation = useMutation({
    mutationFn: ({ id, deptData }) => api.put(`/api/departments/${id}`, deptData),
    onSuccess: () => {
      queryClient.invalidateQueries(['departments']);
      setShowEditModal(false);
      toast.success('Jabatan berjaya dikemaskini');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat mengemaskini jabatan');
    },
  });

  const deleteDepartmentMutation = useMutation({
    mutationFn: (id) => api.delete(`/api/departments/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries(['departments']);
      setShowDeleteModal(false);
      toast.success('Jabatan berjaya dihapus');
    },
    onError: (error) => {
      toast.error(error.response?.data?.message || 'Ralat menghapus jabatan');
    },
  });

  const handleAddDepartment = () => {
    setEditingDepartment({
      dept_name: '',
      dept_desc: '',
    });
    setIsNewDepartment(true);
    setShowEditModal(true);
  };

  const handleEditDepartment = (department) => {
    setEditingDepartment(department);
    setIsNewDepartment(false);
    setShowEditModal(true);
  };

  const handleDeleteDepartment = (department) => {
    setSelectedDepartment(department);
    setShowDeleteModal(true);
  };

  const handleSaveDepartment = (e) => {
    e.preventDefault();
    
    if (!editingDepartment.dept_name || editingDepartment.dept_name.trim() === '') {
      toast.error('Nama Jabatan perlu diisi');
      return;
    }
    
    if (!editingDepartment.dept_desc || editingDepartment.dept_desc.trim() === '') {
      toast.error('Keterangan Jabatan perlu diisi');
      return;
    }

    if (isNewDepartment) {
      addDepartmentMutation.mutate(editingDepartment);
    } else {
      updateDepartmentMutation.mutate({ id: editingDepartment.dept_id, deptData: editingDepartment });
    }
  };

  const filteredDepartments = departments.filter(dept =>
    dept.dept_name?.toLowerCase().includes(searchText.toLowerCase()) ||
    dept.dept_desc?.toLowerCase().includes(searchText.toLowerCase())
  );

  if (isLoading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="p-6">
      {/* Header */}
      <div className="flex items-center mb-6 pb-4 border-b border-gray-200">
        <div className="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center mr-3">
          <Building className="w-5 h-5 text-blue-600" />
        </div>
        <h1 className="text-xl font-semibold text-gray-800">Pengurusan Jabatan</h1>
      </div>

      {/* Controls */}
      <div className="mb-6 flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="flex gap-2">
          <button
            onClick={handleAddDepartment}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            <Plus className="w-4 h-4" />
            Tambah Jabatan
          </button>
        </div>
        
        <div className="flex gap-2 items-center">
          <div className="relative">
            <Search className="w-4 h-4 absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              placeholder="Cari jabatan..."
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
                  Nama Jabatan
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
              {filteredDepartments.map((department) => (
                <tr key={department.dept_id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleEditDepartment(department)}
                        className="text-blue-600 hover:text-blue-900"
                        title="Edit"
                      >
                        <Edit className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => handleDeleteDepartment(department)}
                        className="text-red-600 hover:text-red-900"
                        title="Hapus"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {department.dept_name}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-900">
                    {department.dept_desc}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {new Date(department.createdAt).toLocaleDateString('ms-MY')}
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
              Bil. : <span className="font-medium">{filteredDepartments.length}</span>
            </span>
          </div>
          <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
            <div>
              <p className="text-sm text-gray-700">
                Bil. : <span className="font-medium">{filteredDepartments.length}</span>
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
                {isNewDepartment ? 'Tambah Jabatan Baru' : 'Edit Jabatan'}
              </h3>
              <form onSubmit={handleSaveDepartment} className="space-y-4">
                <div className="grid grid-cols-1 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Nama Jabatan</label>
                    <input
                      type="text"
                      value={editingDepartment?.dept_name || ''}
                      onChange={(e) => setEditingDepartment({...editingDepartment, dept_name: e.target.value})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Nama jabatan..."
                      required
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Keterangan</label>
                    <textarea
                      value={editingDepartment?.dept_desc || ''}
                      onChange={(e) => setEditingDepartment({...editingDepartment, dept_desc: e.target.value})}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500"
                      rows="3"
                      placeholder="Keterangan jabatan..."
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
                    disabled={addDepartmentMutation.isPending || updateDepartmentMutation.isPending}
                    className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
                  >
                    {addDepartmentMutation.isPending || updateDepartmentMutation.isPending ? 'Menyimpan...' : 'Simpan'}
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
                  <strong>{selectedDepartment?.dept_name}</strong> adalah rekod yang dipilih untuk dihapuskan.
                  <br />Anda pasti untuk menghapuskan rekod ini?
                </p>
              </div>
              <div className="flex justify-center gap-3 mt-4">
                <button
                  onClick={() => deleteDepartmentMutation.mutate(selectedDepartment.dept_id)}
                  disabled={deleteDepartmentMutation.isPending}
                  className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50"
                >
                  {deleteDepartmentMutation.isPending ? 'Menghapus...' : 'Ya'}
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

export default Department;