using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class ref_patrol_type
{
    public int type_id { get; set; }

    public string? type_code { get; set; }

    public string? type_name { get; set; }

    public string? type_desc { get; set; }

    public int? creator_id { get; set; }

    public DateTime created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public virtual ICollection<mst_patrol_schedule> mst_patrol_schedules { get; set; } = new List<mst_patrol_schedule>();
}
