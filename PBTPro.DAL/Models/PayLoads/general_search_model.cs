using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{

    public class general_search_result_view
    {
        public int gid { get; set; }
        public JsonDocument? geom { get; set; }
        public string lot { get; set; }
        public string no_lesen { get; set; }
        public string no_cukai { get; set; }
        public string status_lesen { get; set; }
        public string status_cukai { get; set; }
        public string nama_perniagaan { get; set; }
        public string nama_pemilik { get; set; }
        public string alamat_premis { get; set; }
        public string icno_pemilik { get; set; }
    }
}
