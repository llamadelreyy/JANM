using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class compound_medium
{
    public long media_comp_id { get; set; }

    public long media_comp_idno { get; set; }

    public string? media_comp_no { get; set; }

    public string? media_url_link { get; set; }

    public decimal? media_pbt_code { get; set; }

    public virtual compound_info media_comp_idnoNavigation { get; set; } = null!;
}
