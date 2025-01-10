using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class contact_us
{
    public int contact_id { get; set; }

    public string? contact_inq_no { get; set; }

    public string? contact_name { get; set; }

    public string? contact_email { get; set; }

    public string? contact_telno { get; set; }

    public string? contact_subject { get; set; }

    public string? contact_message { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public string? contact_status { get; set; }

    public string? response_message { get; set; }
}
