using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class CompoundMedium
{
    public long MediaCompId { get; set; }

    public long MediaCompIdno { get; set; }

    public string? MediaCompNo { get; set; }

    public string? MediaUrlLink { get; set; }

    public decimal? MediaPbtCode { get; set; }
}
