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
        public string? no_lesen { get; set; }
        public string? nama_perniagaan {get; set;}
        public string? nama_pemilik { get; set; }
        public string? nama_pegawai { get; set; }
        public double? amaun { get; set; }
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
    }
}
