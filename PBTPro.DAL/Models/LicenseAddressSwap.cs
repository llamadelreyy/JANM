using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE BAGI MENYIMPAN ALAMAT LESEN SEDIA ADA DAN BARU
/// </summary>
public partial class LicenseAddressSwap
{
    public long SwapLicenseId { get; set; }

    public long SwapIdInfo { get; set; }

    public string SwapLicenseAccount { get; set; } = null!;

    public string? SwapCurrentAddr1 { get; set; }

    public string? SwapCurrentAddr2 { get; set; }

    public string? SwapCurrentAddr3 { get; set; }

    public string? SwapCurrentArea { get; set; }

    public decimal? SwapCurrentPcode { get; set; }

    public string? SwapCurrentState { get; set; }

    public string? SwapNewAddr1 { get; set; }

    public string? SwapNewAddr2 { get; set; }

    public string? SwapNewAddr3 { get; set; }

    public string? SwapNewArea { get; set; }

    public decimal? SwapNewPcode { get; set; }

    public string? SwapNewState { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual LicenseInformation SwapIdInfoNavigation { get; set; } = null!;
}
