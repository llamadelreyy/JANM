namespace PBT.Data
{
    public class RoleProp
    {
        public int role_id { get; set; }
        public string role_name { get; set; }
        public string role_desc { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
    }
}
