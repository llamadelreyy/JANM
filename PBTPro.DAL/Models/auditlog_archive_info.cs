using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBTPro.DAL.Models;

public partial class auditlog_archive_info
{
    [Key]
    public int archive_id { get; set; }

    public int archive_role_id { get; set; }

    public string archive_module_name { get; set; } = null!;

    public string archive_description { get; set; } = null!;

    public int? created_by { get; set; }

    public DateTime? created_date { get; set; }

    public int? archive_type { get; set; }

    public string? archive_username { get; set; }

    public string? archive_method { get; set; }

    public bool? archive_isarchived { get; set; }

    public int archive_audit_id { get; set; }

}
