namespace PBTPro.DAL.Models
{
    public class system_role
    {
        public int role_id { get; set; }
        public string role_name { get; set; }
        public string role_desc { get; set; }
        public bool role_select { get; set; }
        public string Text => $"{role_name} ({role_desc})";
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public bool active_flag { get; set; }
    }
}
