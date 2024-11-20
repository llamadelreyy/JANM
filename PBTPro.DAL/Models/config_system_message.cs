using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class config_system_message
{
    public int message_id { get; set; }

    public string message_code { get; set; } = null!;

    public string message_type { get; set; } = null!;

    public string message_feature { get; set; } = null!;

    public string? message_body { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }

    public bool? active_flag { get; set; }
}
