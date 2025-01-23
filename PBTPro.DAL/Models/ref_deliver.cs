using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class ref_deliver
{
    public int deliver_id { get; set; }

    public string? deliver_name { get; set; }

    public int? creator_id { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }
}
