using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PBTPro.DAL.Models;

public partial class AuditlogInfo
{
    public int AuditId { get; set; }

    public int AuditRoleId { get; set; }

    public string AuditModuleName { get; set; } = null!;

    public string? AuditDescription { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? AuditType { get; set; }

    public string? AuditUsername { get; set; }

    public string? AuditMethod { get; set; }

    public bool? AuditIsarchived { get; set; }
}
public enum AuditType
{
    [Description("Error")]
    Error = 1,
    [Description("Information")]
    Information = 2
}
public enum AuditTypeLookup
{
    [Display(Name = "Ralat")]
    Ralat = 1,
    [Display(Name = "Informasi")]
    Informasi = 2
}
