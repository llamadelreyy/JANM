using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores status visited premis that has been done by officer
/// </summary>
public partial class trn_premis_visit
{
    public int visit_id { get; set; }

    public bool? status_visit { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public string? codeid_premis { get; set; }

    public int? schedule_id { get; set; }

    public int? user_id { get; set; }

    public virtual mst_premis? codeid_premisNavigation { get; set; }

    public virtual mst_patrol_schedule? schedule { get; set; }

    public virtual ICollection<trn_patrol_officer> trn_patrol_officers { get; set; } = new List<trn_patrol_officer>();
}
