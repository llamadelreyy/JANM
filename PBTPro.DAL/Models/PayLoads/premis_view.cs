using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class premis_view
    {
        //public int id { get; set; }
        //public Point? geom { get; set; }
        public string? gkeseluruh { get; set; }
        public double? _latitude { get; set; }
        public double? _longitude { get; set; }
        public string codeid_premis { get; set; } = null!;
        //public int? creator_id { get; set; }
        //public int? modifier_id { get; set; }
        //public DateTime? created_at { get; set; }
        //public DateTime? modified_at { get; set; }
        //public bool? is_deleted { get; set; }
        public List<premis_license_tax_view> premis_license_tax { get; set; }

        public static explicit operator premis_view(mst_premis v)
        {
            return new premis_view
            {
                //id = v.id,
                //geom = v.geom,
                gkeseluruh = v.gkeseluruh,
                _latitude = v._latitude,
                _longitude = v._longitude,
                codeid_premis = v.codeid_premis,
                //creator_id = v.creator_id,
                //modifier_id = v.modifier_id,
                //created_at = v.created_at,
                //modified_at = v.modified_at,
                //is_deleted = v.is_deleted
            };
        }
    }

    public class premis_license_tax_view
    {
        public int license_premis_tax_id { get; set; }
        public string? license_accno { get; set; }
        public string? codeid_premis { get; set; }
        public string? floor_building { get; set; }
        public string? tax_accno { get; set; }
        public int? status_lesen_id { get; set; }
        public int? status_tax_id { get; set; }
        //public int? creator_id { get; set; }
        //public DateTime? created_at { get; set; }
        //public int? modifier_id { get; set; }
        //public DateTime? modified_at { get; set; }
        //public bool? is_deleted { get; set; }

        public mst_licensee? license { get; set; }
        public mst_owner_licensee? license_owner { get; set; }
        public ref_license_status? license_status { get; set; }
        public mst_taxholder? tax { get; set; }
        public mst_owner_premi? tax_owner { get; set; }
        public ref_tax_status? tax_status { get; set; }

        public static explicit operator premis_license_tax_view(mst_license_premis_tax v)
        {
            return new premis_license_tax_view
            {
                license_premis_tax_id = v.license_premis_tax_id,
                license_accno = v.license_accno,
                codeid_premis = v.codeid_premis,
                floor_building = v.floor_building,
                tax_accno = v.tax_accno,
                status_lesen_id = v.status_lesen_id,
                status_tax_id = v.status_tax_id,
                //creator_id = v.creator_id,
                //created_at = v.created_at,
                //modifier_id = v.modifier_id,
                //modified_at = v.modified_at,
                //is_deleted = v.is_deleted
            };
        }
    }

    public class premis_license_view
    {
        public string aras { get; set; }
        public string no_lesen { get; set; }
        public string nama_perniagaan { get; set; }
        public string nama_pemilik { get; set; }
        public string alamat_premis { get; set; }
        public string status_lesen { get; set; }
    }

    public class premis_history_view
    {
        public int gid { get; set; }
        public string no_ic_pemilik { get; set; }
        public string nama_perniagaan { get; set; }
        public string alamat_premis { get; set; }
        public string status_lesen_perniagaan { get; set; }
        public List<string>? gambar_premis { get; set; }

        #region PREMIS
        public string nama_pemilik { get; set; }
        public string no_tel { get; set; }
        public string jenis_premis { get; set; }
        public string lot { get; set; }
        public string no_lesen_premis { get; set; }
        public DateOnly? tempoh_sah_lesen { get; set; }
        public DateOnly? tempoh_sah_cukai { get; set; }
        public string status_cukai_premis { get; set; }
        #endregion

        #region LESEN
        public string no_acc_lesen { get; set; }
        public DateTime? tarikh_daftar { get; set; }
        public string nama_ops { get; set; }
        public string nama_parlimen { get; set; }
        public string nama_dun { get; set; }
        public string nama_zon { get; set; }
        public DateTime CreatedAt { get; set; }
        #endregion


    }

    public class premis_marker
    {
        public string? codeid_premis { get; set; }
        public string? lot { get; set; }
        public string marker_cukai_status { get; set; }
        public string marker_lesen_status { get; set; }
        public string marker_color { get; set; }
        public JsonDocument? geom { get; set; }
    }

    public class premis_marker_web
    {
        public string? codeid_premis { get; set; }
        public string? lot { get; set; }
        public string marker_cukai_status { get; set; }
        public string marker_lesen_status { get; set; }
        public string marker_color { get; set; }
        public JsonDocument? geom { get; set; }
        public int total_lesen_aktif { get; set; }
        public int total_lesen_tamat_tempoh { get; set; }
        public int total_lesen_gantung { get; set; }
        public int total_lesen_tiada_data { get; set; }
        public int total_lesen_tidak_berlesen { get; set; }
        public int total_cukai_dibayar { get; set; }
        public int total_cukai_tertungak { get; set; }
        public int total_cukai_tiada_data { get; set; }
    }

    public class premis_search_license_view
    {
        public int gid { get; set; }
        public string no_ic_pemilik { get; set; }
        public string no_ic_lesen { get; set; }
        public string nama_perniagaan { get; set; }
        public string alamat_premis { get; set; }
        public string alamat_perniagaan { get; set; }
        public List<string>? gambar_premis { get; set; }
        public string aras { get; set; }
        public string no_lesen { get; set; }
        public string status_lesen { get; set; }
        public double? _latitude { get; set; }
        public double? _longitude { get; set; }
        public string nama_pemilik { get; set; }
        public string nama_pemegang_lesen { get; set; }
        public string email_pemilik { get; set; }
        public string no_tel_pemilik { get; set; }
        public string email_perniagaan { get; set; }
        public string no_tel_perniagaan { get; set; }
        public string jenis_premis { get; set; }
        public string lot { get; set; }
        public DateOnly? tempoh_sah_lesen { get; set; }
        public DateOnly? tempoh_sah_cukai { get; set; }
        public string status_cukai { get; set; }
        public string no_acc_lesen { get; set; }
        public DateTime? tarikh_daftar { get; set; }
    }


    public class premis_notice
    {
        public int trn_notice_id { get; set; }
        public string notice_ref_no { get; set; }
        public string section_code { get; set; }
        public string act_code { get; set; }
        public string created_at { get; set; }
        public string modified_at { get; set; }
        public int trnstatus_id { get; set; }
        public string doc_pathurl { get; set; }
        public string doc_name { get; set; }
        public string trnstatus_view { get; set; }

    }

    public class NoticeRoot
    {
        public int total_records { get; set; }
        public List<premis_notice> notice_lists { get; set; }
    }

    public class premis_compound
    {
        public int trn_cmpd_id { get; set; }
        public string cmpd_ref_no { get; set; }
        public string section_code { get; set; }
        public string act_code { get; set; }
        public string created_at { get; set; }
        public string modified_at { get; set; }
        public int trnstatus_id { get; set; }
        public string doc_pathurl { get; set; }
        public string doc_name { get; set; }
        public string trnstatus_view { get; set; }

    }

    public class CompoundRoot
    {
        public int total_records { get; set; }
        public List<premis_compound> compound_lists { get; set; }
    }

    public class premis_confiscation
    {
        public int trn_cfsc_id { get; set; }
        public string cfsc_ref_no { get; set; }
        public string section_code { get; set; }
        public string act_code { get; set; }
        public string created_at { get; set; }
        public string modified_at { get; set; }
        public int trnstatus_id { get; set; }
        public string doc_pathurl { get; set; }
        public string doc_name { get; set; }
        public string trnstatus_view { get; set; }

    }

    public class ConfiscationRoot
    {
        public int total_records { get; set; }
        public List<premis_confiscation> confiscation_lists { get; set; }
    }

    public class premis_inspection
    {
        public int trn_inspect_id { get; set; }
        public string inspect_ref_no { get; set; }
        public string section_code { get; set; }
        public string act_code { get; set; }
        public string created_at { get; set; }
        public string modified_at { get; set; }
        public int trnstatus_id { get; set; }
        public string doc_pathurl { get; set; }
        public string doc_name { get; set; }
        public string trnstatus_view { get; set; }

    }

    public class InspectionRoot
    {
        public int total_records { get; set; }
        public List<premis_inspection> inspection_lists { get; set; }
    }
}
