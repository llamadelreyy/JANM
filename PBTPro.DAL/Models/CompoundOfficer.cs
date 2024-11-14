using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class CompoundOfficer
{
    public long OfficerId { get; set; }

    public string OfficerSerial { get; set; } = null!;

    public string? OfficerName { get; set; }

    public string? OfficerDept { get; set; }

    public string? OfficerGrade { get; set; }

    public string? OfficerGradeDesc { get; set; }

    public decimal? OfficerPbtCode { get; set; }
}
