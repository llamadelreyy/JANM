using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class patrol_member
{
    public int member_id { get; set; }

    public int member_patrol_id { get; set; }

    public string member_username { get; set; } = null!;

    public int member_cnt_notice { get; set; }

    public int member_cnt_compound { get; set; }

    public int member_cnt_notes { get; set; }

    public int member_cnt_seizure { get; set; }

    public bool member_leader_flag { get; set; }

    public bool active_flag { get; set; }

    public int? created_by { get; set; }

    public DateTime created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? update_date { get; set; }
}
