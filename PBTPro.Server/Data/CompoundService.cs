using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Services;
using MySqlConnector;
using System.Data;
using System.Reflection;
using PBTPro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Data
{
    public partial class CompoundService : IDisposable
    {
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                }

                // Dispose unmanaged resources here

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private List<LesenInfo> _Lesen { get; set; }

        const string className = "CompoundService";
        public IConfiguration _configuration { get; }
        private readonly PBTProDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ArchiveAuditService> _logger;
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        protected readonly AuditLogger _cf;

        private string _baseReqURL = "/api/License";
        private string LoggerName = "";

        public CompoundService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ArchiveAuditService> logger, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dbContext = dbContext;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector);
            CreateLesen();
        }       

        public async void CreateLesen()
        {
            try
            {
                //var platformApiUrl = _configuration["PlatformAPI"];
                //var accessToken = _cf.CheckToken();

                //var request = _cf.CheckRequest(platformApiUrl + "/api/License/ListLicense");
                //string jsonString = await _cf.List(request);

                //Open this when the API is completed
                //dataSource = JsonConvert.DeserializeObject<List<LesenInfo>>(jsonString);

                _Lesen = new List<LesenInfo> {
                    new LesenInfo {
                        IdLesen = 1,
                        Bil = 1,
                        NoRujukan = "K0808M341",
                        TarikhMasa = "08/08/2024 12:30:30PM",
                        NoLesen = "T1000234575-03",
                        NamaPemilik = "CHEMPAKA KASIM SDN BHD",
                        NoDaftarSyarikat = "IP01298238",
                        NamaPerniagaan = "CHEMPAKA KASIM SDN BHD",
                        AlamatPerniagaan = "2-G, Jalan Seri Sarawak 20A, Taman Seri Andalas, Klang, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "AZIZUL HASNI AWANG",
                        NamaSaksi = "KHAIRUL NAZMI MOHAMED",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 500,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/01/06")
                    },
                    new LesenInfo {
                        IdLesen = 2,
                        Bil = 2,
                        NoRujukan = "K0808M876",
                        TarikhMasa = "08/08/2024 12:45:30PM",
                        NoLesen = "T1000786546-07",
                        NamaPemilik = "GREEN SHOE SDN BHD",
                        NoDaftarSyarikat = "BH63457890",
                        NamaPerniagaan = "GREEN SHOE SDN BHD",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 500,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 3,
                        Bil = 3,
                        NoRujukan = "K0808M001",
                        TarikhMasa = "08/08/2024 12:45:30PM",
                        NoLesen = "L001595404",
                        NamaPemilik = "Kawan Ku Fashion",
                        NoDaftarSyarikat = "BH63457890",
                        NamaPerniagaan = "Kawan Ku Fashion",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 500,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 4,
                        Bil = 4,
                        NoRujukan = "K0808M102",
                        TarikhMasa = "08/08/2024 12:45:30PM",
                        NoLesen = "L001595202",
                        NamaPemilik = "Affeine Textile & Fashion",
                        NoDaftarSyarikat = "BH5678934",
                        NamaPerniagaan = "Affeine Textile & Fashion",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 450,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 5,
                        Bil = 5,
                        NoRujukan = "K0809M122",
                        TarikhMasa = "09/08/2024 02:38:10PM",
                        NoLesen = "L001603800",
                        NamaPemilik = "Fakhira Hanis Resources",
                        NoDaftarSyarikat = "BH3457890",
                        NamaPerniagaan = "Fakhira Hanis Resources",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 300,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 6,
                        Bil = 6,
                        NoRujukan = "K0809M172",
                        TarikhMasa = "09/08/2024 09:28:40AM",
                        NoLesen = "L005147010",
                        NamaPemilik = "Aqua Fish Aquatic",
                        NoDaftarSyarikat = "BH45670234",
                        NamaPerniagaan = "Aqua Fish Aquatic",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 1000,
                        StatusBayaran = "TELAH DIBAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 7,
                        Bil = 7,
                        NoRujukan = "K0809M152",
                        TarikhMasa = "09/08/2024 03:15:12PM",
                        NoLesen = "L000821601",
                        NamaPemilik = "TGI Fitness",
                        NoDaftarSyarikat = "BH17834087",
                        NamaPerniagaan = "TGI Fitness",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 1000,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 8,
                        Bil = 8,
                        NoRujukan = "K0809M176",
                        TarikhMasa = "09/08/2024 12:30:30PM",
                        NoLesen = "L00685630",
                        NamaPemilik = "Era Global Trade Sdn Bhd",
                        NoDaftarSyarikat = "BH6435678",
                        NamaPerniagaan = "Era Global Trade Sdn Bhd",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 500,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 9,
                        Bil = 9,
                        NoRujukan = "K0810M112",
                        TarikhMasa = "10/08/2024 01:29:03PM",
                        NoLesen = "L001504508",
                        NamaPemilik = "Miewa Enterprise",
                        NoDaftarSyarikat = "BH5436732",
                        NamaPerniagaan = "Miewa Enterprise",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 200,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new LesenInfo {
                        IdLesen = 10,
                        Bil = 10,
                        NoRujukan = "K0810M122",
                        TarikhMasa = "10/08/2024 11:21:24AM",
                        NoLesen = "L001598502",
                        NamaPemilik = "Butik Syed Elysime Daniel",
                        NoDaftarSyarikat = "BH16437893",
                        NamaPerniagaan = "Butik Syed Elysime Daniel",
                        AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
                        NamaSaksi = "ENG AH MENG",
                        CaraSerahan = "SERAHAN TANGAN",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",

                        AmaunKompaun = 300,
                        StatusBayaran = "BELUM BAYAR",
                        AmaunDibayar = 0,
                        NoResitBayaran  = "-",
                        DateCreated = DateTime.Parse("2023/03/11")
                    }
                 };

                await _cf.CreateAuditLog((int)AuditType.Information, className + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai lesen.", 1, LoggerName, "");
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, className + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public Task<List<LesenInfo>> GetLesenAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, className + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai lesen.", 1, LoggerName, "");
            return Task.FromResult(_Lesen);
        }


        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<List<LesenInfo>> RefreshLesenAsyncs()
        //{
        //    List<LesenInfo> arrItem = new List<LesenInfo>();

        //    try
        //    {
        //        var platformApiUrl = _configuration["PlatformAPI"];
        //        var accessToken = _cf.CheckToken();

        //        var request = _cf.CheckRequest(platformApiUrl + "/api/Department/ListDepartment");
        //        string jsonString = await _cf.List(request);
        //        arrItem = JsonConvert.DeserializeObject<List<LesenInfo>>(jsonString);
        //        await _cf.CreateAuditLog((int)AuditType.Information, className + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai lesen.", 1, LoggerName, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, className + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }

        //    return arrItem;
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<LesenInfo>> RefreshLesenAsync()
        {
            var result = new List<LesenInfo>();
            try
            {
                string requestUrl = $"{_baseReqURL}/ListLesen";
                var response = await _apiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<LesenInfo>>(dataString);
                        await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula dan papar senarai jabatan.", 1, LoggerName, "");
                    }
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod :" + response.ReturnCode, 1, LoggerName, "");
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
                result = new List<LesenInfo>();
            }
            return result;
        }

    }
}
