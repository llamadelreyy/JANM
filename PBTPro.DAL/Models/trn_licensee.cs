using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class trn_licensee
{
    public int id { get; set; }

    public string? codeid_premis { get; set; }

    public string? owner_icno { get; set; }

    public string? owner_name { get; set; }

    public string? owner_email { get; set; }

    public string? owner_addr { get; set; }

    public string? owner_telno { get; set; }

    public string? ssm_no { get; set; }

    public string? business_name { get; set; }

    public string? business_addr { get; set; }

    public string? license_accno { get; set; }

    public int? cat_id { get; set; }

    public int? license_status_id { get; set; }

    public string? floor { get; set; }

    public string? pic_name { get; set; }

    public string? pic_phone_no { get; set; }

    public string? activity { get; set; }

    public string? notes { get; set; }

    public string? image_1 { get; set; }

    public string? image_2 { get; set; }

    public string? image_3 { get; set; }

    public string? image_4 { get; set; }

    public string? image_5 { get; set; }

    public string? image_6 { get; set; }

    public string? status { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }
}
