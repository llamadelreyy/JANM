namespace PBTPro.DAL.Models
{
    public class user_role
    {
        public int table_id { get; set; }
        public int role_id { get; set; }
        public string role_name { get; set; }
        public string role_desc { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_full_name { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public bool active_flag { get; set; }
    }
}
