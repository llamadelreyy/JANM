using GoogleMapsComponents.Maps;

namespace PBTPro.DAL.Models
{
    public class NoticeProp
    {
        public int IdNotice { get; set; }
        public int Bil { get; set; }
        public int Type { get; set; }
        public string NoLot { get; set; }
        public LatLngLiteral? Position { get; set; }

        public string NoRujukan { get; set; }
        public string TarikhMasa { get; set; }
        public string TarikhTamatNotis { get; set; }
        public string NoLesen { get; set; }
        public string NamaPemilik { get; set; }
        public string NoDaftarSyarikat { get; set; }
        public string NamaPerniagaan { get; set; }
        public string AlamatPerniagaan { get; set; }
        public string AktaKesalahan { get; set; }
        public string KodKesalahan { get; set; }
        public string Arahan { get; set; }
        public string Ulasan { get; set; }
        public string LokasiKesalahan { get; set; }
        public string NamaPegawaiPengeluar { get; set; }
        public string NamaSaksi { get; set; }
        public string CaraSerahan { get; set; }
        public string BuktiPath1 { get; set; }
        public string BuktiPath2 { get; set; }
        public string BuktiPath3 { get; set; }
        public string BuktiPath4 { get; set; }
        public string StatusNotis { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
