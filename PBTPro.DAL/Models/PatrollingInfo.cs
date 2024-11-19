using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class PatrollingInfo
{
    public int PatrollingId { get; set; }

    public string? PatrollingOfficerName { get; set; }

    public string? PatrollingLocation { get; set; }

    public DateTime? PatrollingStartTime { get; set; }

    public DateTime? PatrollingEndTime { get; set; }

    public string? PatrollingStatus { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }
}
