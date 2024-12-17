namespace PBTPro.DAL.Models
{
    public class section_info
    {
        public int section_id { get; set; }
        public int dept_id { get; set; }
        public string section_code { get; set; }
        public string section_name { get; set; }
        public string section_desc { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
    }
}
