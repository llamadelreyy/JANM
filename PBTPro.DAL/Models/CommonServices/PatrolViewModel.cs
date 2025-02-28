using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.CommonServices
{
    public class PatrolViewModel
    {
        public int scheduleId { get; set; }
        public string? ICNo { get; set; }
        public string? OfficerName { get; set; }
        public bool UserSelected { get; set; }
        public string?  DistrictCode { get; set; }
        public string? DistrictName { get; set; }
        public string?  TownCode { get; set; }
        public string? TownName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DeptId { get; set; }
        public string? DeptName { get; set; }
        public int SectionId { get; set; }
        public string? SectionName { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public string PatrolStatus { get; set; } = null!;
        public bool is_deleted { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }

    }
}
