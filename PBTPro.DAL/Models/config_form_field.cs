using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class config_form_field
{
    public int field_id { get; set; }

    public string field_form_type { get; set; } = null!;

    public string field_name { get; set; } = null!;

    public string field_label { get; set; } = null!;

    public string field_type { get; set; } = null!;

    public string? field_option { get; set; }

    public string? field_source_url { get; set; }

    public bool field_required { get; set; }

    public bool field_api_seeded { get; set; }

    public int field_orders { get; set; }

    public bool active_flag { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }
}
