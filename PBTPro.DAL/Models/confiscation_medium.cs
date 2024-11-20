using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class confiscation_medium
{
    public long confiscation_media_id { get; set; }

    public long confiscation_main_id { get; set; }

    public string? confiscation_url_link { get; set; }

    public DateTime? created_date { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual confiscation_info confiscation_main { get; set; } = null!;
}
