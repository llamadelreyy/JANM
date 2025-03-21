using DevExpress.Xpo.Logger;
using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using PBTPro.DAL.Services;
using System.Reflection;
using System.Text;


namespace PBTPro.Data
{
    public partial class NoticeService : IDisposable
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

        private List<NoticeProp> _Notice { get; set; }
        const string className = "NoticeService";
        public IConfiguration Configuration { get; }
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        protected readonly AuditLogger _cf;

        private string _baseReqURL = "/api/Notices";
        private string LoggerName = "";
        private int LoggerID = 0;
        private int RoleID = 0;

        public NoticeService(IConfiguration configuration, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            Configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
            CreateNotice();
            LoggerName = _PBTAuthStateProvider.CurrentUser.Fullname;
            LoggerID = _PBTAuthStateProvider.CurrentUser.Userid;
            RoleID = _PBTAuthStateProvider.CurrentUser.Roleid;
        }

        //public NoticeService(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //    CreateNotice();
        //}

        //public List<NoticeProp> CreateNotice()
        public async void CreateNotice()
        {
            List<NoticeProp> dataSource = new List<NoticeProp>();

            try
            {
                _Notice = new List<NoticeProp> {
                    new NoticeProp {
                        IdNotice = 1,
                        Bil = 1,
                        Type = "Aktif",
                        NoLot = "101",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05051,
                            Lng = 101.62411
                        },
                        NoRujukan = "K0808M341",
                        TarikhMasa = "08/08/2024 12:30:30PM",
                        NoLesen = "T1000234575-03",
                        NamaPemilik = "Geo Sepakat Sdn Bhd",
                        NoDaftarSyarikat = "IP01298238",
                        NamaPerniagaan = "Geo Sepakat Sdn Bhd",
                        AlamatPerniagaan = "2-G, Jalan Seri Sarawak 20A, Taman Seri Andalas, Klang, Selangor",
                        AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
                        Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
                        LokasiKesalahan = "HADAPAN PREMIS",
                        NamaPegawaiPengeluar = "AZIZUL HASNI AWANG",
                        NamaSaksi = "KHAIRUL NAZMI MOHAMED",
                        CaraSerahan = "SERAHAN TANGAN",
                        TarikhTamatNotis = "16/08/2024 (7 Hari)",
                        Ulasan = "TELAH PATUH NOTIS",
                        BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
                        BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
                        BuktiPath3 = "",
                        BuktiPath4 = "",
                        StatusNotis = "TAMAT TEMPOH",
                        DateCreated = DateTime.Parse("2023/01/06")
                    },
                    new NoticeProp {
                        IdNotice = 2,
                        Bil = 2,
                        Type = "Aktif",
                        NoLot = "102",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05049,
                            Lng = 101.62404
                        },
                        NoRujukan = "K0808M876",
                        TarikhMasa = "08/08/2024 12:45:30PM",
                        NoLesen = "T1000786546-07",
                        NamaPemilik = "STO Tour Sdn Bhd",
                        NoDaftarSyarikat = "BH63457890",
                        NamaPerniagaan = "STO Tour Sdn Bhd",
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
                        TarikhTamatNotis = "22/08/2024(14 Hari)",
                        Ulasan = "TELAH PATUH NOTIS",
                        StatusNotis = "BARU",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 3,
                        Bil = 3,
                        Type = "Tamat Tempoh",
                        NoLot = "103",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05045,
                            Lng = 101.62400
                        },
                        NoRujukan = "K0808M001",
                        TarikhMasa = "08/08/2024 12:45:30PM",
                        NoLesen = "L001595404",
                        NamaPemilik = "99 Speed Mart Sdn Bhd",
                        NoDaftarSyarikat = "BH63457890",
                        NamaPerniagaan = "99 Speed Mart Sdn Bhd",
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
                        TarikhTamatNotis = "19/08/2024 (7 Hari)",
                        Ulasan = "NOTIS DIKELUARKAN",
                        StatusNotis = "BARU",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 4,
                        Bil = 4,
                        Type = "Gantung",
                        NoLot = "104",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05041,
                            Lng = 101.62396
                        },
                        NoRujukan = "K0808M102",
                        TarikhMasa = "08/08/2024 12:45:30PM",
                        NoLesen = "L001595202",
                        NamaPemilik = "Malayan Banking Berhad",
                        NoDaftarSyarikat = "BH5678934",
                        NamaPerniagaan = "Malayan Banking Berhad",
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
                        TarikhTamatNotis = "14/08/2024 (2 Hari)",
                        Ulasan = "TINDAKAN TELAH DIAMBIL",
                        StatusNotis = "TUTUP",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 5,
                        Bil = 5,
                        Type = "Tamat Tempoh",
                        NoLot = "105",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05037,
                            Lng = 101.62391
                        },
                        NoRujukan = "K0809M122",
                        TarikhMasa = "09/08/2024 02:38:10PM",
                        NoLesen = "L001603800",
                        NamaPemilik = "FAS Empire",
                        NoDaftarSyarikat = "BH3457890",
                        NamaPerniagaan = "FAS Empire",
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
                        TarikhTamatNotis = "20/08/2024 (7 Hari)",
                        Ulasan = "NOTIS TELAH DIKELUARKAN",
                        StatusNotis = "BARU",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 6,
                        Bil = 6,
                        Type = "Aktif",
                        NoLot = "106",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05033,
                            Lng = 101.62387
                        },
                        NoRujukan = "K0809M172",
                        TarikhMasa = "09/08/2024 09:28:40AM",
                        NoLesen = "L005147010",
                        NamaPemilik = "Raghavan Textile",
                        NoDaftarSyarikat = "BH45670234",
                        NamaPerniagaan = "Raghavan Textile",
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

                        TarikhTamatNotis = "20/08/2024 (7 Hari)",
                        Ulasan = "NOTIS TELAH DIKELUARKAN",
                        StatusNotis = "BARU",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 7,
                        Bil = 7,
                        Type = "Aktif",
                        NoLot = "107",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05030,
                            Lng = 101.62382
                        },
                        NoRujukan = "K0809M152",
                        TarikhMasa = "09/08/2024 03:15:12PM",
                        NoLesen = "L000821601",
                        NamaPemilik = "NJ Nawar Enterprise",
                        NoDaftarSyarikat = "BH17834087",
                        NamaPerniagaan = "NJ Nawar Enterprise",
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
                        TarikhTamatNotis = "26/08/2024 (14 Hari)",
                        Ulasan = "NOTIS TELAH DIKELUARKAN",
                        StatusNotis = "BARU",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 8,
                        Bil = 8,
                        Type = "Tiada Data",
                        NoLot = "108",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05025,
                            Lng = 101.624378
                        },
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
                        TarikhTamatNotis = "27/08/2024 (14 Hari)",
                        Ulasan = "TINDAKAN TELAH DIAMBIL",
                        StatusNotis = "TUTUP",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 9,
                        Bil = 9,
                        Type = "Tamat Tempoh",
                        NoLot = "109",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05022,
                            Lng = 101.62373
                        },
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

                        TarikhTamatNotis = "27/08/2024 (14 Hari)",
                        Ulasan = "TIADA RESPON DARI PEMILIK",
                        StatusNotis = "TAMAT TEMPOH",
                        DateCreated = DateTime.Parse("2023/03/11")
                    },
                    new NoticeProp {
                        IdNotice = 10,
                        Bil = 10,
                        Type = "Aktif",
                        NoLot = "110",
                        Position = new LatLngLiteral()
                        {
                            Lat = 3.05017,
                            Lng = 101.62369
                        },
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

                        TarikhTamatNotis = "22/08/2024 (14 Hari)",
                        Ulasan = "",
                        StatusNotis = "BARU",
                        DateCreated = DateTime.Parse("2023/03/11")
                    }
                 };
                await _cf.CreateAuditLog((int)AuditType.Information, className + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai notis.", LoggerID, LoggerName, GetType().Name, RoleID);
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, className + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            finally
            {
            }
        }

        public Task<List<NoticeProp>> GetNoticeAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, className + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai notis.", LoggerID, LoggerName, GetType().Name, RoleID);
            return Task.FromResult(_Notice);
        }

        [HttpGet]
        public async Task<List<trn_notices_view>> ListReport()
        {
            var result = new List<trn_notices_view>();
            string requestUrl = $"{_baseReqURL}/ListReport";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<trn_notices_view>>(dataString);
                    }
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Papar semula senarai data.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod : " + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
                result = new List<trn_notices_view>();
            }
            return result;
        }

        public async Task<ReturnViewModel> Delete(int id)
        {
            var result = new ReturnViewModel();
            try
            {
                string requestUrl = $"{_baseReqURL}/DeleteReport/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Delete);

                result = response;
                if (result.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya padam data.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod :" + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            return result;
        }
        public async Task<ReturnViewModel> Update(trn_notices_view inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                int id = inputModel.id_notis;
                var reqData = JsonConvert.SerializeObject(inputModel);
                var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

                string requestUrl = $"{_baseReqURL}/UpdateReport/{id}";
                var response = await _apiConnector.ProcessLocalApi(requestUrl, HttpMethod.Put, reqContent);

                result = response;
                if (result.ReturnCode == 200)
                {
                    await _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya kemaskini data.", LoggerID, LoggerName, GetType().Name, RoleID);
                }
                else
                {
                    await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Ralat - Status Kod :" + response.ReturnCode, LoggerID, LoggerName, GetType().Name, RoleID);
                }
            }
            catch (Exception ex)
            {
                result = new ReturnViewModel();
                await _cf.CreateAuditLog((int)AuditType.Error, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, LoggerID, LoggerName, GetType().Name, RoleID);
            }
            return result;
        }
    }
}
