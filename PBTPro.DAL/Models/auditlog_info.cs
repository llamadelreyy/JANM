using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class auditlog_info
{
    public int audit_id { get; set; }

    public int audit_role_id { get; set; }

    public string audit_module_name { get; set; } = null!;

    public string audit_description { get; set; } = null!;

    public int? created_by { get; set; }

    public DateTime? created_date { get; set; }

    public int? audit_type { get; set; }

    public string? audit_username { get; set; }

    public string? audit_method { get; set; }

    public bool? audit_isarchived { get; set; }
}
