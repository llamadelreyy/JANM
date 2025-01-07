using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models
{
    public class system_user
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string ic_no { get; set; }
        public string mobile_no { get; set; }
        public string full_name { get; set; }
        public string user_email { get; set; }
        public string Text => $"{user_name} ({full_name})";
        public int dept_id { get; set; }
        public string dept_name { get; set; }
        public int div_id { get; set; }
        public string div_name { get; set; }
        public int unit_id { get; set; }
        public string unit_name { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public bool active_flag { get; set; }
    }
}
