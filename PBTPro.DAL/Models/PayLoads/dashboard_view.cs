using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class dashboard_view
    {
        public int total_notis { get; set; }
        public int total_kompaun { get; set; }
        public int total_inspection { get; set; }
        public int total_confiscation { get; set; }
        public int premis_berlesen { get; set; }
        public int premis_tamat_tempoh_lesen { get; set; }
        public int premis_diperiksa { get; set; }
        public int premis_dikenakan_tindakan { get; set; }
        public int premis_tiada_lesen { get; set; }
        public int total_cukai_tahunan { get; set; }
        public decimal? amaun_kutipan_cukai { get; set; }
        public decimal? cukai_taksiran_dibyr { get; set; }
        public decimal? cukai_taksiran_blm_dibyr { get; set; }
        public decimal? hsl_lesen_dibyr { get; set; }
        public decimal? hsl_lesen_blm_dibyr { get; set; }
        public decimal? kompaun_dibyr { get; set; }
        public decimal? kompaun_blm_dibyr { get; set; }
        public int total_lesen_aktif { get; set; }
        public int lesen_tamat_tempoh { get; set; }
        public int total_premis_perniagaan { get; set; }
        public decimal? hsl_tahunan_semasa { get; set; }
        public decimal? ptmbahan_lesen_thn_semasa { get; set; }
        public decimal? ptmbahan_lesen_semasa { get; set; }
        public int? total_premis_dilawat { get; set; }
        public int? total_rondaan_dibuat { get; set; }
        public int? total_lokasi_baru { get; set; }
        public int lesen_dikenakan_tindakan { get; set; }
    }
    public class graph_report_view
    {
        public List<int> total { get; set; }
        public List<int> month { get; set; }
    }
}
