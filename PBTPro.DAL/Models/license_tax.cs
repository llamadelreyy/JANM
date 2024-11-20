using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE MAKLUMAT CUKAI TAKSIRAN
/// </summary>
public partial class license_tax
{
    public long tax_id { get; set; }

    public long tax_license_info { get; set; }

    public string tax_license_account { get; set; } = null!;

    public string? tax_main_account { get; set; }

    public string? tax_property_address1 { get; set; }

    public string? tax_property_address2 { get; set; }

    public string? tax_property_area { get; set; }

    public decimal? tax_property_pcode { get; set; }

    public string? tax_property_state { get; set; }

    public string? tax_property_status { get; set; }

    public decimal? tax_property_amount { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public string? created_by { get; set; }

    public string? updated_by { get; set; }

    public virtual license_information tax_license_infoNavigation { get; set; } = null!;
}
