using MySqlConnector;
using System.Data;
using System.Reflection;

namespace PBT.Data
{
    public class JabatanService : IDisposable
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

        private List<JabatanProp> _Jabatan { get; set; }

        private readonly SqlConnectionConfiguration _configuration;
        public JabatanService(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
            _Jabatan = CreateJabatan();
        }

        public List<JabatanProp> CreateJabatan()
        {
            List<JabatanProp> dataSource = new List<JabatanProp>();

            try
            {
                dataSource = new List<JabatanProp> {
                    new JabatanProp {
                        dept_id = 1,
                        dept_code = "001",
                        dept_name = "Perlesenan",
                        dept_desc = "Jabatan Perlesenan",
                        created_date = DateTime.Parse("2023/03/11")
                    },
                    new JabatanProp {
                        dept_id = 2,
                        dept_code = "002",
                        dept_name = "Penilaian",
                        dept_desc = "Jabatan Cukai & Penilaian",
                        created_date = DateTime.Parse("2023/03/11")
                    }
                 };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : CreateJabatan - {0}", ex);
                return dataSource;
            }
            finally
            {
            }

            return dataSource;
        }

        public Task<List<JabatanProp>> GetJabatanAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Jabatan);
        }


        public Task<bool> InsertJabatanAsync(JabatanProp changed)
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

        public Task<bool> UpdateJabatanAsync(JabatanProp changed)
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

        public Task<bool> RemoveJabatanAsync(JabatanProp dtDataItem)
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

        public Task<List<JabatanProp>> RefreshJabatanAsync()
        {
            List<JabatanProp> arrItem = new List<JabatanProp>();
            JabatanProp _item;

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
                                _item = new JabatanProp();
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

                                _item.created_date = myReader.IsDBNull("created_date") ? null : myReader.GetDateTime("created_date");
                                _item.created_by = myReader.IsDBNull("created_by") ? "" : myReader.GetString("created_by");
                                _item.updated_date = myReader.IsDBNull("updated_date") ? null : myReader.GetDateTime("updated_date");
                                _item.updated_by = myReader.IsDBNull("updated_by") ? "" : myReader.GetString("updated_by");
                                _item.active_flag = myReader.IsDBNull("active_flag") ? false : myReader.GetBoolean("active_flag");

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