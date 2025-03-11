using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class trn_patrol_officer
{
    public int officer_id { get; set; }

    public string? idno { get; set; }

    public int? schedule_id { get; set; }

    public DateTime? start_time { get; set; }

    public DateTime? end_time { get; set; }

    public int cnt_notice { get; set; }

    public int cnt_cmpd { get; set; }

    public int cnt_notes { get; set; }

    public int cnt_seizure { get; set; }

    public int? creator_id { get; set; }

    public DateTime created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public bool? is_leader { get; set; }

    public int? user_id { get; set; }

    public int? visit_id { get; set; }

    public virtual mst_patrol_schedule? schedule { get; set; }

    public virtual trn_premis_visit? visit { get; set; }
}
