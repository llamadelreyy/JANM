using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models.PayLoads
{
    public class StartPatrolModel
    {
        public List<string>? Usernames { get; set; }
        public Point? CurrentLocation { get; set; }
    }

    public class StopPatrolModel
    {
        public int PatrolId { get; set; }
        public Point? CurrentLocation { get; set; }
    }
    public class PatrolSchedulerModel
    {
        public int scheduler_id { get; set; }
        public string? scheduler_officer { get; set; }
        public string? scheduler_location { get; set; }
        public DateTime? scheduler_date { get; set; }
        public List<string>? Usernames { get; set; }
    }

}
