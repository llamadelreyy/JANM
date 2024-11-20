using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class confiscation_info
{
    public long confiscation_id { get; set; }

    public string? confiscation_offender { get; set; }

    public string? confiscation_address1 { get; set; }

    public string? confiscation_address2 { get; set; }

    public string? confiscation_address3 { get; set; }

    public string? confiscation_area { get; set; }

    public decimal? confiscation_pcode { get; set; }

    public string? confiscation_state { get; set; }

    public string? confiscation_longitude { get; set; }

    public string? confiscation_langitude { get; set; }

    public string? confiscation_officer { get; set; }

    public string? confiscation_act_code { get; set; }

    public string? confiscation_offence_code { get; set; }

    public DateOnly? confiscation_date { get; set; }

    public string? confiscation_status { get; set; }

    public string? confiscation_detail { get; set; }

    public decimal? confiscation_amount { get; set; }

    public string? confiscation_offender_id { get; set; }

    public string? confiscation_license_no { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<confiscation_medium> confiscation_media { get; set; } = new List<confiscation_medium>();
}
