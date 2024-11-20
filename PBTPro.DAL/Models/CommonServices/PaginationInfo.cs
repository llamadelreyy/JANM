namespace PBTPro.DAL.Models.CommonServices
{
    public class PaginationInfo
    {
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int RecordPerPage { get; set; }
        public int CurrentPageNo { get; set; }
    }
}
