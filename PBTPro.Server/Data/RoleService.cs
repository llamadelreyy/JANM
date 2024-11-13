using MySqlConnector;
using System.Data;
using System.Reflection;

namespace PBT.Data
{
    public class RoleService : IDisposable
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

        private List<RoleProp> _Role { get; set; }

        private readonly SqlConnectionConfiguration _configuration;
        public RoleService(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
            _Role = CreateRole();
        }

        public List<RoleProp> CreateRole()
        {
            List<RoleProp> dataSource = new List<RoleProp>();

            try
            {
                dataSource = new List<RoleProp> {
                    new RoleProp {
                        role_id = 1,
                        role_name = "Administrator",
                        role_desc = "Admin of the system",
                        created_date = DateTime.Parse("2023/03/11")
                    },
                    new RoleProp {
                        role_id = 2,
                        role_name = "Head of Department",
                        role_desc = "Head of department for perlesenan",
                        created_date = DateTime.Parse("2023/03/11")
                    }
                 };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : CreateRole - {0}", ex);
                return dataSource;
            }
            finally
            {
            }

            return dataSource;
        }

        public Task<List<RoleProp>> GetRoleAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Role);
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