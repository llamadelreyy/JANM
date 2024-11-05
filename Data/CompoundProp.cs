using System.ComponentModel.DataAnnotations;

namespace PBT.Data
{
    public class LesenInfo
    {
        public int IdLesen { get; set; }
        public int Bil { get; set; }
        public string NoRujukan { get; set; }
        public string TarikhMasa { get; set; }
        public string NoLesen { get; set; }
        public string NamaPemilik { get; set; }
        public string NoDaftarSyarikat { get; set; }
        public string NamaPerniagaan { get; set; }
        public string AlamatPerniagaan { get; set; }
        public string AktaKesalahan { get; set; }
        public string KodKesalahan { get; set; }
        public string Arahan { get; set; }
        public string LokasiKesalahan { get; set; }
        public string NamaPegawaiPengeluar { get; set; }
        public string NamaSaksi { get; set; }
        public string CaraSerahan { get; set; }
        public string BuktiPath1 { get; set; }
        public string BuktiPath2 { get; set; }
        public string BuktiPath3 { get; set; }
        public string BuktiPath4 { get; set; }

        public double AmaunKompaun { get; set; }
        public string StatusBayaran { get; set; }
        public double AmaunDibayar { get; set; }
        public string NoResitBayaran { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
