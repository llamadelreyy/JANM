using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE MAKLUMAT CUKAI TAKSIRAN
/// </summary>
public partial class LicenseTax
{
    public long TaxId { get; set; }

    public long TaxLicenseInfo { get; set; }

    public string TaxLicenseAccount { get; set; } = null!;

    public string? TaxMainAccount { get; set; }

    public string? TaxPropertyAddress1 { get; set; }

    public string? TaxPropertyAddress2 { get; set; }

    public string? TaxPropertyArea { get; set; }

    public decimal? TaxPropertyPcode { get; set; }

    public string? TaxPropertyState { get; set; }

    public string? TaxPropertyStatus { get; set; }

    public decimal? TaxPropertyAmount { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual LicenseInformation TaxLicenseInfoNavigation { get; set; } = null!;
}
