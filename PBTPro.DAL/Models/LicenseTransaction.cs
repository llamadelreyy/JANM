using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE TRANSAKSI LESEN
/// </summary>
public partial class LicenseTransaction
{
    public long LicenseTransId { get; set; }

    public long LicenseTransInfo { get; set; }

    public string LicenseTransAccount { get; set; } = null!;

    public string LicenseTransCode { get; set; } = null!;

    public string? LicenseTransName { get; set; }

    public decimal? LicenseTransAmount { get; set; }

    public string? LicenseTransStatus { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual LicenseInformation LicenseTransInfoNavigation { get; set; } = null!;
}
