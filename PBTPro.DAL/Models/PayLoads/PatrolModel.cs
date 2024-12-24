using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models.PayLoads
{
    public class StartPatrolModel
    {
        public int? patrol_id { get; set; }
        public List<string>? usernames { get; set; }
        public CurrentLocation? current_location { get; set; }
    }

    public class StopPatrolModel
    {
        public int patrol_id { get; set; }
        public CurrentLocation? current_location { get; set; }
    }

    public class PatrolInputMemberModel
    {
        public int patrol_id { get; set; }
        public string username { get; set; }
    }

    public class PatrolSchedulerModel
    {
        public int scheduler_id { get; set; }
        public string? scheduler_officer { get; set; }
        public string? scheduler_location { get; set; }
        public DateTime? scheduler_date { get; set; }
        public List<string>? Usernames { get; set; }
    }

    public class CurrentLocation
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
