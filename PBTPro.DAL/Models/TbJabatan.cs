using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class TbJabatan
{
    public int jabid { get; set; }

    public string kodjab { get; set; } = null!;

    public string namajab { get; set; } = null!;

    public string catatan { get; set; } = null!;

    public string Rekstatus { get; set; } = null!;

    public DateTime? Rekcipta { get; set; }

    public int Rekciptauserid { get; set; }

    public DateTime? Rekubah { get; set; }

    public int Rekubahuserid { get; set; }
}
