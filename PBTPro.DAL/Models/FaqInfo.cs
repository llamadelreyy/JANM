using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class FaqInfo
{
    public int FaqId { get; set; }

    public string FaqCategory { get; set; } = null!;

    public string FaqQuestion { get; set; } = null!;

    public string FaqAnswer { get; set; } = null!;

    public string FaqStatus { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }
}
