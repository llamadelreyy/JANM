using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.CommonServices
{
    public class PatrolViewModel
    {
        public int Id { get; set; }
        public string? OfficerName { get; set; }
        public bool UserSelected { get; set; }
        public int StateId { get; set; }
        public string? StateName { get; set; }
        public int DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public int TownId { get; set; }
        public string? TownName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
