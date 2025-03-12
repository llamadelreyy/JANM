using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about owners.
/// </summary>
public partial class mst_owner
{
    public int owner_id { get; set; }

    public string? owner_icno { get; set; }

    public string? owner_name { get; set; }

    public string? owner_email { get; set; }

    public string? owner_addr { get; set; }

    public string? district_code { get; set; }

    public string? state_code { get; set; }

    public string? owner_telno { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }

    public int? town_id { get; set; }

    public static explicit operator mst_owner(mst_owner_premi v)
    {
        return new mst_owner
        {
            owner_id = v.owner_id,
            owner_icno = v.owner_icno,
            owner_name = v.owner_name,
            owner_email = v.owner_email,
            owner_addr = v.owner_addr,
            district_code = v.district_code,
            state_code = v.state_code,
            owner_telno = v.owner_telno,
            creator_id = v.creator_id,
            created_at = v.created_at,
            modifier_id = v.modifier_id,
            modified_at = v.modified_at,
            is_deleted = v.is_deleted,
            town_id = v.town_id
        };
    }

    public static explicit operator mst_owner(mst_owner_licensee v)
    {
        return new mst_owner
        {
            owner_id = v.owner_id,
            owner_icno = v.owner_icno,
            owner_name = v.owner_name,
            owner_email = v.owner_email,
            owner_addr = v.owner_addr,
            district_code = v.district_code,
            state_code = v.state_code,
            owner_telno = v.owner_telno,
            creator_id = v.creator_id,
            created_at = v.created_at,
            modifier_id = v.modifier_id,
            modified_at = v.modified_at,
            is_deleted = v.is_deleted,
            town_id = v.town_id
        };
    }
}
