using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace PBTPro.DAL.Models;

public partial class trn_premis
{
    public int id { get; set; }

    public string? codeid_premis { get; set; }

    public string? owner_icno { get; set; }

    public string? owner_name { get; set; }

    public string? owner_email { get; set; }

    public string? owner_addr { get; set; }

    public string? owner_telno { get; set; }

    public string? tax_accno { get; set; }

    public int? tax_status_id { get; set; }

    public Point? geom { get; set; }

    public string? lot { get; set; }

    public string? image { get; set; }

    public string? category { get; set; }

    public string? address { get; set; }

    public string? notes { get; set; }

    public string? status { get; set; }

    public int? creator_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? modifier_id { get; set; }

    public DateTime? modified_at { get; set; }

    public bool? is_deleted { get; set; }
}
