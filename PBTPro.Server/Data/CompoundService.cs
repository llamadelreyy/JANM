using PBT.Services;
using PBT.Data;
using MySqlConnector;
using System.Data;
using DevExpress.DashboardCommon;
using System.Data.SqlClient;
using System.Reflection;

namespace PBT.Data
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

        //public IConfiguration Configuration { get; }
        //public CompoundService(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //    _Lesen = CreateLesen();
        //}
        private readonly SqlConnectionConfiguration _configuration;
        public CompoundService(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
            _Lesen = CreateLesen();
        }

        public List<LesenInfo> CreateLesen()
        {
            List<LesenInfo> dataSource = new List<LesenInfo>();

            try
            {
                dataSource = new List<LesenInfo> {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : CreateCompany - {0}", ex);
                return dataSource;
            }
            finally
            {
            }

            return dataSource;
        }

        public Task<List<LesenInfo>> GetLesenAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Lesen);
        }


        public Task<bool> InsertLesenAsync(LesenInfo changed)
        {
            using (MySqlConnection? conn = new MySqlConnection(_configuration.Value))
            {
                const string strSQL = @"insert into dbo.City (Name,State) values (@Name,@State)";
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (MySqlCommand? myCmd = new MySqlCommand(strSQL, conn))
                {
                    try
                    {
                        myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught : " + MethodBase.GetCurrentMethod().Name + " --> {0}", ex);
                        return Task.FromResult(false); ;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateLesenAsync(LesenInfo changed)
        {

            using (MySqlConnection? conn = new MySqlConnection(_configuration.Value))
            {
                const string strSQL = @"";
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (MySqlCommand? myCmd = new MySqlCommand(strSQL, conn))
                {
                    try
                    {
                        myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught : " + MethodBase.GetCurrentMethod().Name + " --> {0}", ex);
                        return Task.FromResult(false); ;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }


            return Task.FromResult(true);
        }

        public Task<bool> RemoveLesenAsync(LesenInfo dtDataItem)
        {
            using (MySqlConnection? conn = new MySqlConnection(_configuration.Value))
            {
                const string strSQL = @"";
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (MySqlCommand? myCmd = new MySqlCommand(strSQL, conn))
                {
                    try
                    {
                        myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught : " + MethodBase.GetCurrentMethod().Name + " --> {0}", ex);
                        return Task.FromResult(false); ;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }

            return Task.FromResult(true);
        }

        public Task<List<LesenInfo>> RefreshLesenAsync()
        {
            List<LesenInfo> arrItem = new List<LesenInfo>();
            LesenInfo _item;

            using (MySqlConnection? conn = new MySqlConnection(_configuration.Value))
            {
                const string strSQL = @"";
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (MySqlCommand? myCmd = new MySqlCommand(strSQL, conn))
                {
                    using (MySqlDataReader? myReader = myCmd.ExecuteReader())
                    {
                        try
                        {
                            //Loop every data
                            while (myReader.Read())
                            {
                                _item = new LesenInfo();
                                //_item.tbId = myReader.IsDBNull("tbID") ? 0 : myReader.GetInt32("tbID");
                                //_item.code = myReader.IsDBNull("tbCode") ? "" : myReader.GetString("tbCode");
                                //_item.divisionName = myReader.IsDBNull("tbDivision") ? "" : myReader.GetString("tbDivision");
                                //_item.sourcesName = myReader.IsDBNull("tbSources") ? "" : myReader.GetString("tbSources");
                                //_item.industryName = myReader.IsDBNull("tbIndustry") ? "" : myReader.GetString("tbIndustry");
                                //_item.sectorName = myReader.IsDBNull("tbSector") ? "" : myReader.GetString("tbSector");
                                //_item.companyName = myReader.IsDBNull("tbCompanyName") ? "" : myReader.GetString("tbCompanyName");
                                //_item.yearOfEstablishment = myReader.IsDBNull("tbYearOfEstablishment") ? null : myReader.GetDateTime("tbYearOfEstablishment");
                                //_item.directorsManagement = myReader.IsDBNull("tbDirectorsManagement") ? "" : myReader.GetString("tbDirectorsManagement");
                                //_item.natureOfBusiness = myReader.IsDBNull("tbNatureOfBusiness") ? "" : myReader.GetString("tbNatureOfBusiness");
                                //_item.ownership = myReader.IsDBNull("tbOwnership") ? "" : myReader.GetString("tbOwnership");
                                //_item.revenue = myReader.IsDBNull("tbRevenue") ? 0 : myReader.GetDouble("tbRevenue");
                                //_item.revenueYear = myReader.IsDBNull("tbYear") ? 0 : myReader.GetInt32("tbYear");
                                //_item.competitionMarketIssues = myReader.IsDBNull("tbCompetitionMarketIssues") ? "" : myReader.GetString("tbCompetitionMarketIssues");
                                //_item.myccsRecommendations = myReader.IsDBNull("tbMyCCsRecommendations") ? "" : myReader.GetString("tbMyCCsRecommendations");
                                //_item.issuesNews = myReader.IsDBNull("tbIssuesNews") ? "" : myReader.GetString("tbIssuesNews");
                                //_item.marketShare = myReader.IsDBNull("tbMarketShare") ? 0 : myReader.GetDouble("tbMarketShare");

                                //////_item.rekCipta = myReader.IsDBNull("rekCipta") ? null : myReader.GetDateTime("rekCipta");
                                //////_item.rekCiptaUserID = myReader.IsDBNull("rekCiptaUserID") ? 0 : myReader.GetInt32("rekCiptaUserID");
                                //////_item.rekUbah = myReader.IsDBNull("rekUbah") ? null : myReader.GetDateTime("rekUbah");
                                //////_item.rekUbahUserID = myReader.IsDBNull("rekUbahUserID") ? 0 : myReader.GetInt32("rekUbahUserID");
                                //////_item.rekStatus = myReader.IsDBNull("rekStatus") ? "" : myReader.GetString("rekStatus");

                                //Add item into list
                                arrItem.Add(_item);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception caught : " + MethodBase.GetCurrentMethod().Name + " --> {0}", ex);
                            return Task.FromResult(arrItem); ;
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                                conn.Close();
                        }
                    }
                }
            }

            return Task.FromResult(arrItem);
        }

    }
}
