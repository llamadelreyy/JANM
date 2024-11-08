using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class TbHubungikami
{
    public int Hubkamiid { get; set; }

    public string Tiketid { get; set; } = null!;

    public string Namapenghantar { get; set; } = null!;

    public string Emailpenghantar { get; set; } = null!;

    public string Telnopenghantar { get; set; } = null!;

    public string? Catatan { get; set; }

    public string Namapenerima { get; set; } = null!;

    public DateTime? Rekcipta { get; set; }

    public int Rekciptauserid { get; set; }

    public DateTime? Rekubah { get; set; }

    public int Rekubahuserid { get; set; }
}
