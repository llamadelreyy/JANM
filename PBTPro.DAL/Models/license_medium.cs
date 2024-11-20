using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class license_medium
{
    public long media_id { get; set; }

    public long media_id_info { get; set; }

    public string? media_license_account { get; set; }

    public string? media_url_link { get; set; }

    public virtual license_information media_id_infoNavigation { get; set; } = null!;
}
