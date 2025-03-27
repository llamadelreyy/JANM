using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class trn_compound_view
    {
        public int id_kompaun { get; set; }
        public string? no_rujukan { get; set; }
        public int? lesen_id { get; set; }
        public string? no_lesen { get; set; }
        public string? nama_perniagaan { get; set; }
        public string? nama_pemilik { get; set; }
        public string? nama_pegawai { get; set; }
        public double? amaun { get; set; }
        public int? status_bayaran_id { get; set; }
        public string? status_bayaran { get; set; }
        public string? ssm_no { get; set; }
        public string? alamat_perniagaan { get; set; }
        public string? akta_kesalahan { get; set; }
        public string? kod_kesalahan { get; set; }
        public string? kod_seksyen { get; set; }
        public string? kod_uuk { get; set; }
        public string? arahan { get; set; }
        public string? lokasi_kesalahan { get; set; }
        public string? nama_saksi { get; set; }
        public string? cara_serahan { get; set; }
        public double? amaun_dibayar { get; set; }
        public string? no_resit_bayaran { get; set; }
        public DateTime? TarikhData { get; set; }
        public string? nama_dokumen { get; set; }
        public string? pautan_dokumen { get; set; }
        public List<string> imej_kompaun { get; set; }
        public bool is_cukai { get; set; }
        public string? no_cukai { get; set; }
    }

    public class trn_notices_view
    {
        public int id_notis { get; set; }
        public string? no_rujukan { get; set; }
        public int? lesen_id { get; set; }
        public string? no_lesen { get; set; }
        public string? nama_perniagaan { get; set; }
        public string? nama_pemilik { get; set; }       
        public string? nama_pegawai { get; set; }
        public DateTime? tarikh_tamat { get; set; }
        public int? tempoh_notis_id { get; set; }
        public string? tempoh_notis { get; set; }
        public int? status_notis_id { get; set; }
        public string? status_notis { get; set; }
        public string? ssm_no { get; set; }
        public string? alamat_perniagaan { get; set; }
        public string? akta_kesalahan { get; set; }
        public string? kod_kesalahan { get; set; }
        public string? kod_seksyen { get; set; }
        public string? kod_uuk { get; set; }
        public string? arahan { get; set; }
        public string? lokasi_kesalahan { get; set; }
        public string? nama_saksi { get; set; }
        public string? cara_serahan { get; set; }
        public DateTime? TarikhData { get; set; }
        public string? nama_dokumen { get; set; }
        public string? pautan_dokumen { get; set; }
        public string? no_cukai { get; set; }
        public List<string> imej_notis { get; set; }
        public bool is_cukai { get; set; }

    }

    public class trn_inspect_view
    {
        public int id_nota { get; set; }
        public string? no_rujukan { get; set; }
        public int? lesen_id { get; set; }
        public string? no_lesen { get; set; }
        public string? nama_perniagaan { get; set; }
        public string? nama_pemilik { get; set; }
        public string? nama_pegawai { get; set; }
        public string? ssm_no { get; set; }
        public string? alamat_perniagaan { get; set; }
        public string? arahan { get; set; }
        public string? lokasi_kesalahan { get; set; }
        public string? nama_saksi { get; set; }
        public DateTime? TarikhData { get; set; }
        public string? nama_dokumen { get; set; }
        public string? pautan_dokumen { get; set; }
        public string? no_cukai { get; set; }
        public List<string> imej_nota { get; set; }
        public int? id_jabatan { get; set; }
        public string? nama_jabatan { get; set; }
        public int status_nota_id { get; set; }
        public string? status_nota { get; set; }
        public decimal? inspect_longitude { get; set; }     
        public decimal? inspect_latitude { get; set; }
        public bool is_cukai { get; set; }
    }

    public class trn_cfsc_view
    {
        public int id_sita { get; set; }
        public string? no_rujukan { get; set; }
        public int? lesen_id { get; set; }
        public string? no_lesen { get; set; }
        public string? nama_perniagaan { get; set; }
        public string? nama_pemilik { get; set; }
        public string? nama_pegawai { get; set; }
        public string? ssm_no { get; set; }
        public string? alamat_perniagaan { get; set; }
        public string? arahan { get; set; }
        public string? lokasi_kesalahan { get; set; }
        public DateTime? TarikhData { get; set; }
        public string? nama_dokumen { get; set; }
        public string? pautan_dokumen { get; set; }
        public string? no_cukai { get; set; }
        public List<string> imej_sita { get; set; }
        public string? nama_jabatan { get; set; }
        public int status_sita_id { get; set; }
        public string? status_sita { get; set; }
        public decimal? inspect_longitude { get; set; }
        public decimal? inspect_latitude { get; set; }
        public bool is_cukai { get; set; }
        public string? akta_kesalahan { get; set; }
        public string? kod_kesalahan { get; set; }
        public string? kod_seksyen { get; set; }
        public string? kod_uuk { get; set; }
        public int? inv_id { get; set; }
        public int? inv_type_id { get; set; }
        public int? scenario_id { get; set; }
        public string? nama_scenario { get; set; }
        public string? nama_inventori { get; set; }
        public int? jenis_inventori { get; set; }
        public string? nama_inv_type { get; set; }
    }
}
