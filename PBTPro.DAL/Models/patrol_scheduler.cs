using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class patrol_scheduler
{
    public int scheduler_id { get; set; }

    public string? scheduler_officer { get; set; }

    public string? scheduler_location { get; set; }

    public DateTime? scheduler_date { get; set; }

    public bool active_flag { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }
}
