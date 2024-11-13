namespace PBT.Data
{
    public class UserRoleProp
    {
        public int role_id { get; set; }
        public int user_id { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
    }
}
