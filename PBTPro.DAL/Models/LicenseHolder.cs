using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE MAKLUMAT PELESEN
/// </summary>
public partial class LicenseHolder
{
    public long LicenseHolderId { get; set; }

    public long LicenseHolderInfo { get; set; }

    public string LicenseHolderAccount { get; set; } = null!;

    public string LicenseHolderName { get; set; } = null!;

    public string LicenseHolderCustid { get; set; } = null!;

    public string LicenseHolderAddr1 { get; set; } = null!;

    public string LicenseHolderAddr2 { get; set; } = null!;

    public string LicenseHolderAddr3 { get; set; } = null!;

    public string? LicenseHolderArea { get; set; }

    public decimal LicenseHolderPcode { get; set; }

    public string LicenseHolderState { get; set; } = null!;

    public decimal LicenseHolderPhone { get; set; }

    public string? LicenseHolderEmail { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual LicenseInformation LicenseHolderInfoNavigation { get; set; } = null!;
}
