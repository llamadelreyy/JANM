using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class LicenseMedium
{
    public long MediaId { get; set; }

    public long MediaIdInfo { get; set; }

    public string? MediaLicenseAccount { get; set; }

    public string? MediaUrlLink { get; set; }

    public virtual LicenseInformation MediaIdInfoNavigation { get; set; } = null!;
}
