using System.ComponentModel.DataAnnotations;

namespace PBTPro.Shared.Models.SystemVersion
{
    public class VersionInformation
    {
        [Key]
        public Guid VersionId { get; set; }
        public string? VersionNumber { get; set; }
        public string? VersionName { get; set; }
        public string? VersionDescription { get; set; }
    }
}
