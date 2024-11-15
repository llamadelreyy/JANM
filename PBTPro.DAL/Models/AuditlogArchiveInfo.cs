using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class AuditlogArchiveInfo
{
    public int ArchiveId { get; set; }

    public int ArchiveRoleId { get; set; }

    public string ArchiveModuleName { get; set; } = null!;

    public string ArchiveDescription { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ArchiveType { get; set; }

    public string? ArchiveUsername { get; set; }

    public string? ArchiveMethod { get; set; }

    public bool? ArchiveIsarchived { get; set; }
}
