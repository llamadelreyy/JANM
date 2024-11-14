using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// TABLE SEJARAH LESEN
/// </summary>
public partial class LicenseHistory
{
    public long LicenseHistId { get; set; }

    public string LicenseHistAccount { get; set; } = null!;

    public string? LicenseHistHolder { get; set; }

    public DateOnly? LicenseHistStartd { get; set; }

    public DateOnly? LicenseHistEndd { get; set; }

    public string? LicenseHistAddr1 { get; set; }

    public string? LicenseHistAddr2 { get; set; }

    public string? LicenseHistAddr3 { get; set; }

    public string? LicenseHistArea { get; set; }

    public decimal? LicenseHistPcode { get; set; }

    public string? LicenseHistState { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastUpdated { get; set; }
}
