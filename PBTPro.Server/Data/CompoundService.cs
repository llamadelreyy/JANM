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
using PBTPro.DAL.Models.PayLoads;
using System.Text;

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

        private List<trn_compound_view> _Lesen { get; set; }

        private List<trn_compound> _Compound {get; set;}
        public IConfiguration _configuration { get; }
        private readonly ApiConnector _apiConnector;
        private readonly PBTAuthStateProvider _PBTAuthStateProvider;
        protected readonly AuditLogger _cf;

        private string _baseReqURL = "/api/Compounds";
        private string LoggerName = "";
        private int LoggerID = 0;
        private int RoleID = 0;

        public CompoundService(IConfiguration configuration, PBTProDbContext dbContext, ApiConnector apiConnector, PBTAuthStateProvider PBTAuthStateProvider)
        {
            _configuration = configuration;
            _PBTAuthStateProvider = PBTAuthStateProvider;
            _apiConnector = apiConnector;
            _apiConnector.accessToken = _PBTAuthStateProvider.accessToken;
            _cf = new AuditLogger(configuration, apiConnector, PBTAuthStateProvider);
            LoggerName = _PBTAuthStateProvider.CurrentUser.Fullname;
            LoggerID = _PBTAuthStateProvider.CurrentUser.Userid;
            RoleID = _PBTAuthStateProvider.CurrentUser.Roleid;
        }

        //public async void CreateLesen()
        //{
        //    try
        //    {
        //        //var platformApiUrl = _configuration["PlatformAPI"];
        //        //var accessToken = _cf.CheckToken();

        //        //var request = _cf.CheckRequest(platformApiUrl + "/api/License/ListLicense");
        //        //string jsonString = await _cf.List(request);

        //        //Open this when the API is completed
        //        //dataSource = JsonConvert.DeserializeObject<List<LesenInfo>>(jsonString);

        //        _Lesen = new List<LesenInfo> {
        //            new LesenInfo {
        //                IdLesen = 1,
        //                Bil = 1,
        //                NoRujukan = "K0808M341",
        //                TarikhMasa = "08/08/2024 12:30:30PM",
        //                NoLesen = "T1000234575-03",
        //                NamaPemilik = "CHEMPAKA KASIM SDN BHD",
        //                NoDaftarSyarikat = "IP01298238",
        //                NamaPerniagaan = "CHEMPAKA KASIM SDN BHD",
        //                AlamatPerniagaan = "2-G, Jalan Seri Sarawak 20A, Taman Seri Andalas, Klang, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "AZIZUL HASNI AWANG",
        //                NamaSaksi = "KHAIRUL NAZMI MOHAMED",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 500,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/01/06")
        //            },
        //            new LesenInfo {
        //                IdLesen = 2,
        //                Bil = 2,
        //                NoRujukan = "K0808M876",
        //                TarikhMasa = "08/08/2024 12:45:30PM",
        //                NoLesen = "T1000786546-07",
        //                NamaPemilik = "GREEN SHOE SDN BHD",
        //                NoDaftarSyarikat = "BH63457890",
        //                NamaPerniagaan = "GREEN SHOE SDN BHD",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 500,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 3,
        //                Bil = 3,
        //                NoRujukan = "K0808M001",
        //                TarikhMasa = "08/08/2024 12:45:30PM",
        //                NoLesen = "L001595404",
        //                NamaPemilik = "Kawan Ku Fashion",
        //                NoDaftarSyarikat = "BH63457890",
        //                NamaPerniagaan = "Kawan Ku Fashion",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 500,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 4,
        //                Bil = 4,
        //                NoRujukan = "K0808M102",
        //                TarikhMasa = "08/08/2024 12:45:30PM",
        //                NoLesen = "L001595202",
        //                NamaPemilik = "Affeine Textile & Fashion",
        //                NoDaftarSyarikat = "BH5678934",
        //                NamaPerniagaan = "Affeine Textile & Fashion",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 450,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 5,
        //                Bil = 5,
        //                NoRujukan = "K0809M122",
        //                TarikhMasa = "09/08/2024 02:38:10PM",
        //                NoLesen = "L001603800",
        //                NamaPemilik = "Fakhira Hanis Resources",
        //                NoDaftarSyarikat = "BH3457890",
        //                NamaPerniagaan = "Fakhira Hanis Resources",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 300,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 6,
        //                Bil = 6,
        //                NoRujukan = "K0809M172",
        //                TarikhMasa = "09/08/2024 09:28:40AM",
        //                NoLesen = "L005147010",
        //                NamaPemilik = "Aqua Fish Aquatic",
        //                NoDaftarSyarikat = "BH45670234",
        //                NamaPerniagaan = "Aqua Fish Aquatic",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 1000,
        //                StatusBayaran = "TELAH DIBAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 7,
        //                Bil = 7,
        //                NoRujukan = "K0809M152",
        //                TarikhMasa = "09/08/2024 03:15:12PM",
        //                NoLesen = "L000821601",
        //                NamaPemilik = "TGI Fitness",
        //                NoDaftarSyarikat = "BH17834087",
        //                NamaPerniagaan = "TGI Fitness",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 1000,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 8,
        //                Bil = 8,
        //                NoRujukan = "K0809M176",
        //                TarikhMasa = "09/08/2024 12:30:30PM",
        //                NoLesen = "L00685630",
        //                NamaPemilik = "Era Global Trade Sdn Bhd",
        //                NoDaftarSyarikat = "BH6435678",
        //                NamaPerniagaan = "Era Global Trade Sdn Bhd",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 500,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 9,
        //                Bil = 9,
        //                NoRujukan = "K0810M112",
        //                TarikhMasa = "10/08/2024 01:29:03PM",
        //                NoLesen = "L001504508",
        //                NamaPemilik = "Miewa Enterprise",
        //                NoDaftarSyarikat = "BH5436732",
        //                NamaPerniagaan = "Miewa Enterprise",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 200,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            },
        //            new LesenInfo {
        //                IdLesen = 10,
        //                Bil = 10,
        //                NoRujukan = "K0810M122",
        //                TarikhMasa = "10/08/2024 11:21:24AM",
        //                NoLesen = "L001598502",
        //                NamaPemilik = "Butik Syed Elysime Daniel",
        //                NoDaftarSyarikat = "BH16437893",
        //                NamaPerniagaan = "Butik Syed Elysime Daniel",
        //                AlamatPerniagaan = "1, Jalan 14/20, 46100 Petaling Jaya, Selangor",
        //                AktaKesalahan = "U22 UNDANG-UNDANG KECIL PELESENAN TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                KodKesalahan = "SEKSYEN 3 (1) - TIADA SEORANG PUN BOLEH MENJALANKAN APA-APA AKTIVITI TRED, PERNIAGAAN DAN PERINDUSTRIAN",
        //                Arahan = "HADIR UNTUK PEMBAHARUAN LESEN",
        //                LokasiKesalahan = "HADAPAN PREMIS",
        //                NamaPegawaiPengeluar = "RUDZUAN SULAIMAN",
        //                NamaSaksi = "ENG AH MENG",
        //                CaraSerahan = "SERAHAN TANGAN",
        //                BuktiPath1 = "\\images\\bukti\\herbstore.jpg",
        //                BuktiPath2 = "\\images\\bukti\\herbstore2.jpg",
        //                BuktiPath3 = "",
        //                BuktiPath4 = "",

        //                AmaunKompaun = 300,
        //                StatusBayaran = "BELUM BAYAR",
        //                AmaunDibayar = 0,
        //                NoResitBayaran  = "-",
        //                DateCreated = DateTime.Parse("2023/03/11")
        //            }
        //         };

        //        await _cf.CreateAuditLog((int)AuditType.Information, className + " - " + MethodBase.GetCurrentMethod().Name, "Papar semua senarai lesen.", LoggerID, LoggerName, GetType().Name, RoleID);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _cf.CreateAuditLog((int)AuditType.Error, className + " - " + MethodBase.GetCurrentMethod().Name, ex.Message, 1, LoggerName, "");
        //    }
        //}

        [AllowAnonymous]
        [HttpGet]
        public Task<List<trn_compound_view>> GetCompoundAsync(CancellationToken ct = default)
        {
            var result = _cf.CreateAuditLog((int)AuditType.Information, GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, "Berjaya muat semula senarai data.", LoggerID, LoggerName, GetType().Name, RoleID);
            return Task.FromResult(_Lesen);
        }


        [HttpGet]
        public async Task<List<trn_compound_view>> ListReport()
        {
            var result = new List<trn_compound_view>();
            string requestUrl = $"{_baseReqURL}/ListReport";
            var response = await _apiConnector.ProcessLocalApi(requestUrl);

            try
            {
                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        result = JsonConvert.DeserializeObject<List<trn_compound_view>>(dataString);
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
                result = new List<trn_compound_view>();
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
        public async Task<ReturnViewModel> Update(trn_compound_view inputModel)
        {
            var result = new ReturnViewModel();
            try
            {
                int id = inputModel.id_kompaun;
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
