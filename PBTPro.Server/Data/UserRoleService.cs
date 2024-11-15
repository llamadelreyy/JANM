using MySqlConnector;
using System.Data;
using System.Reflection;

namespace PBT.Data
{
    public class UserRoleService : IDisposable
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

        private List<UserRoleProp> _UserRole { get; set; }

        private readonly SqlConnectionConfiguration _configuration;
        public UserRoleService(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
            _UserRole = CreateUserRole();
        }

        public List<UserRoleProp> CreateUserRole()
        {
            List<UserRoleProp> dataSource = new List<UserRoleProp>();

            try
            {
                dataSource = new List<UserRoleProp> {
                    new UserRoleProp {
                        table_id = 1,
                        user_id = 1,
                        role_id = 1,
                        user_name = "mbdk240015",
                        role_name = "Administrator",
                        role_desc = "Admin of the system",
                        user_full_name = "Azman Bin Alias",
                        created_date = DateTime.Parse("2024/01/05")
                    },
                    new UserRoleProp {
                        table_id = 2,
                        user_id = 2,
                        role_id = 1,
                        user_name = "mbdk230010",
                        role_name = "Administrator",
                        role_desc = "Admin of the system",
                        user_full_name = "Abu Bakar Bin Jamal",
                        created_date = DateTime.Parse("2023/03/10")
                    },
                    new UserRoleProp {
                        table_id = 3,
                        user_id = 2,
                        role_id = 2,
                        user_name = "mbdk230010",
                        role_name = "Head of Department",
                        role_desc = "Head of department for perlesenan",
                        user_full_name = "Abu Bakar Bin Jamal",
                        created_date = DateTime.Parse("2023/03/10")
                    }
                 };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : CreateUserRole - {0}", ex);
                return dataSource;
            }
            finally
            {
            }

            return dataSource;
        }

        public Task<List<UserRoleProp>> GetUserRoleAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_UserRole);
        }


        public Task<bool> InsertUserRoleAsync(UserRoleProp changed)
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

        public Task<bool> UpdateUserRoleAsync(UserRoleProp changed)
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

        public Task<bool> RemoveUserRoleAsync(UserRoleProp dtDataItem)
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

        public Task<List<UserRoleProp>> RefreshUserRoleAsync()
        {
            List<UserRoleProp> arrItem = new List<UserRoleProp>();
            UserRoleProp _item;

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
                                _item = new UserRoleProp();
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