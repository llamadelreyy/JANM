using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

public partial class mst_dashboard
{
    public int JumlahNotisSP { get; set; }
    public int JumlahPremisBerlesenSP { get; set; }
    public int JumlahNotis { get; set; }
    public int JumlahKompaun { get; set; }
    public int JumlahNotaPemeriksaan { get; set; }
    public int JumlahSitaan { get; set; }
    public int JumlahPremisBerlesen { get; set; }
    public int JumlahPremisTidakBerlesen { get; set; }
    public int JumlahLesenAktif { get; set; }
    public int JumlahLesenTamatTempoh { get; set; }
    public int JumlahLesenGantung { get; set; }
    public int JumlahLesenTiadaData { get; set; }
    public int PertambahanLesenTahunSemasa { get; set; }
    public int PertambahanLesenBulanSemasa { get; set; }

}
