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
}
