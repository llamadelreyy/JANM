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
        public int premis_tidak_berlesen { get; set; }
        public int premis_diperiksa { get; set; }
        public int premis_dikenakan_tindakan { get; set; }

    }
}
