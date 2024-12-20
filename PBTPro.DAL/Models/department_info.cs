using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models
{
    public class department_info
    {
        public int dept_id { get; set; }

        public string dept_code { get; set; } = null!;

        public string dept_name { get; set; } = null!;

        public string? dept_description { get; set; }

        public string dept_status { get; set; } = null!;

        public DateTime created_date { get; set; }

        public int? created_by { get; set; }

        public DateTime updated_date { get; set; }

        public int? updated_by { get; set; }

    }
}