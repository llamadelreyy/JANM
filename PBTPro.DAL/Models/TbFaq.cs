using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class TbFaq
{
    public int Faqid { get; set; }

    public string Kategorifaq { get; set; } = null!;

    public string Soalanfaq { get; set; } = null!;

    public string Jawapanfaq { get; set; } = null!;

    public string Rekstatus { get; set; } = null!;

    public DateTime? Rekcipta { get; set; }

    public int Rekciptauserid { get; set; }

    public DateTime? Rekubah { get; set; }

    public int Rekubahuserid { get; set; }
}
