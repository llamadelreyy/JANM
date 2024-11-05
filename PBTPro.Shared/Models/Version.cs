using System;
using System.Collections.Generic;

namespace PBTPro.Shared.Models;

public partial class Version
{
    public Guid Versionid { get; set; }

    public string? Versionnumber { get; set; }

    public string? Versionname { get; set; }

    public string? Versiondescription { get; set; }
}
