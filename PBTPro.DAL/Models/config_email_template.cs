using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class config_email_template
{
    public int template_id { get; set; }

    public string template_code { get; set; } = null!;

    public string template_subject { get; set; } = null!;

    public string? template_content { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }

    public bool active_flag { get; set; }
}
