using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class CompoundLocation
{
    public long LocationId { get; set; }

    public long LocationCompId { get; set; }

    public string? LocationCompNo { get; set; }

    public string? LocationLongitude { get; set; }

    public string? LocationLatitude { get; set; }

    public decimal? LocationPbtCode { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
