namespace PBT.Data
{
    public class UnitProp
    {
        public int unit_id { get; set; }
        public int dept_id { get; set; }
        public string unit_code { get; set; }
        public string unit_name { get; set; }
        public string unit_desc { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }

    }
}
