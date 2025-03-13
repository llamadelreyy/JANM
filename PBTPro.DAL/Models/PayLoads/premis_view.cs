using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public string marker_cukai_status { get; set; }
        public string marker_lesen_status { get; set; }
        public string marker_color { get; set; }
        public JsonDocument? geom { get; set; }
    }
}
