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
        public int gid { get; set; }
        public string lot { get; set; }
        public string no_lesen { get; set; }
        public string no_cukai { get; set; }
        public string status_lesen { get; set; }
        public string status_cukai { get; set; }
        public string nama_perniagaan { get; set; }
        public string nama_pemilik { get; set; }
        public string alamat_premis1 { get; set; }
        public string alamat_premis2 { get; set; }
        public string status_notice { get; set; }
        public string status_expired_notice { get; set; }
        public string status_kompaun { get; set; }
        public string status_nota_pemeriksaan { get; set; }
        public List<string>? gambar_premis { get; set; }
        public List<premis_license_view>? lesen { get; set; }
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
        public int gid { get; set; }
        public string lot { get; set; }
        public string marker_cukai_status { get; set; }
        public string marker_lesen_status { get; set; }
        public string marker_color { get; set; }
        public JsonDocument? geom { get; set; }
    }
}
