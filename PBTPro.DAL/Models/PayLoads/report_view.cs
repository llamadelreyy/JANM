using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class report_view
    {
        public int report_id { get; set; }
        public string nama_premis { get; set; }
        public string alamat { get; set; }
        public string no_lesen { get; set; }
        public string no_ssm { get; set; }
        public string operasi_lesen { get; set; }
        public string kategori_lesen { get; set; }
        public string no_cukai_taksiran { get; set; }
        public int parlimen_id { get; set; }
        public string parlimen_name { get; set; }
        public int dun_id { get; set; }
        public string dun_name { get; set; }
        public int zon_id { get; set; }
        public string zon_name { get; set; }
        public DateTime tarikh_mula { get; set; }
        public DateTime tarikh_tamat { get; set; }
    }
}
