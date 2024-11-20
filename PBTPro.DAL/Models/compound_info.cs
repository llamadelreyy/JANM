using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class compound_info
{
    public long compound_id { get; set; }

    public string compound_no { get; set; } = null!;

    public decimal? compound_pbt_code { get; set; }

    public string? compound_act_code { get; set; }

    public string? compound_offence_code { get; set; }

    public string? compound_trans_code { get; set; }

    public string? compound_offender_id { get; set; }

    public string? compound_offender_name { get; set; }

    public string? compound_offender_addr1 { get; set; }

    public string? compound_offender_addr2 { get; set; }

    public string? compound_offender_addr3 { get; set; }

    public string? compound_offender_area { get; set; }

    public decimal? compound_offender_pcode { get; set; }

    public string? compound_offender_state { get; set; }

    public DateOnly? compound_date { get; set; }

    public decimal? compound_amount { get; set; }

    public string? compound_license_no { get; set; }

    public string? compound_vehicle_plate { get; set; }

    public string? compound_road_tax { get; set; }

    public string? compound_status { get; set; }

    public string? compound_pay_status { get; set; }

    public DateOnly? compound_pay_date { get; set; }

    public string? compound_vehicle_type { get; set; }

    public string? compound_vehicle_brand { get; set; }

    public string? compound_vehicle_model { get; set; }

    public DateOnly? compound_court_date { get; set; }

    public decimal? compound_pay_amount { get; set; }

    public string? compound_desc { get; set; }

    public string? compound_officer_code { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<compound_location> compound_locations { get; set; } = new List<compound_location>();

    public virtual ICollection<compound_medium> compound_media { get; set; } = new List<compound_medium>();
}
