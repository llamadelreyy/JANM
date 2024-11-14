using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE BAGI MAKLUMAT LESEN
/// </summary>
public partial class LicenseInformation
{
    public long LicenseId { get; set; }

    public decimal? LicensePbtOrigin { get; set; }

    public string LicenseAccountNumber { get; set; } = null!;

    public string? LicenseType { get; set; }

    public string? LicenseRiskStatus { get; set; }

    public DateOnly? LicenseApplyDate { get; set; }

    public DateOnly? LicenseApprovedDate { get; set; }

    public DateOnly? LicenseStartDate { get; set; }

    public DateOnly? LicenseEndDate { get; set; }

    public decimal? LicensePeriod { get; set; }

    public string? LicenseStatus { get; set; }

    public string? LicenseBusinessName { get; set; }

    public string? LicenseBusinessAddr1 { get; set; }

    public string? LicenseBusinessAddr2 { get; set; }

    public string? LicenseBusinessAddr3 { get; set; }

    public string? LicenseBusinessArea { get; set; }

    public decimal? LicenseBusinessPcode { get; set; }

    public string? LicenseBusinessState { get; set; }

    public decimal? LicenseAmount { get; set; }

    public decimal? LicenseAmountBalance { get; set; }

    public string? LicensePaymentStatus { get; set; }

    public string? LicensePeriodStatus { get; set; }

    public string? LicenseLongitud { get; set; }

    public string? LicenseLatitude { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<LicenseAddressSwap> LicenseAddressSwaps { get; set; } = new List<LicenseAddressSwap>();

    public virtual ICollection<LicenseHolder> LicenseHolders { get; set; } = new List<LicenseHolder>();

    public virtual ICollection<LicenseMedium> LicenseMedia { get; set; } = new List<LicenseMedium>();

    public virtual ICollection<LicenseTax> LicenseTaxes { get; set; } = new List<LicenseTax>();

    public virtual ICollection<LicenseTransaction> LicenseTransactions { get; set; } = new List<LicenseTransaction>();
}
