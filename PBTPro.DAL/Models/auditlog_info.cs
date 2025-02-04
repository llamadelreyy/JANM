using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class auditlog_info
{
    //public int audit_id { get; set; }

    //public int audit_role_id { get; set; }

    //public string audit_module_name { get; set; } = null!;

    //public string audit_description { get; set; } = null!;

    //public int? created_by { get; set; }

    //public DateTime? created_date { get; set; }

    //public int? audit_type { get; set; }

    //public string? audit_username { get; set; }

    //public string? audit_method { get; set; }

    //public bool? audit_isarchived { get; set; }

    public int log_id { get; set; }

    public int role_id { get; set; }

    public string module_name { get; set; } = null!;

    public string log_descr { get; set; } = null!;

    public int? log_type { get; set; }

    public string? username { get; set; }

    public string? log_method { get; set; }

    public bool? is_archived { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }
}
