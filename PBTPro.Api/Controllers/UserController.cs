using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using static DevExpress.Data.Filtering.Helpers.PropertyDescriptorCriteriaCompilationSupport;
using System.Data.Common;
using System.Data;
using NetTopologySuite.Index.HPRtree;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class UserController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<FaqController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _module = "Faq";
        private readonly PBTProDbContext _dbContext;
        private List<TbFaq> _Faq { get; set; }

        public UserController(PBTProDbContext dbContext, ILogger<FaqController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = _configuration.GetConnectionString("DefaultConnection");//configuration.GetValue<string>("ConnectionStrings");
            _dbContext = dbContext;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public string GetListFAQ()
        //{
        //    string jsonResult = "[]";
        //    try
        //    {
        //        using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
        //        {
        //            using (NpgsqlCommand? myCmd = new NpgsqlCommand("call ListFAQ(:_soalanfaq, :_jawapanfaq, :_kategorifaq, :_rekCiptaUserID, :_rekstatus, :_rekCipta)", myConn))
        //            {
        //                myCmd.CommandType = CommandType.Text;
        //                myConn.Open();
        //                using (NpgsqlDataReader? myReader = myCmd.ExecuteReader())
        //                {
        //                    //Loop every data
        //                    while (myReader.Read())
        //                    {
        //                        myReader.Read();
        //                        jsonResult = string.IsNullOrEmpty(myReader["dtJSON"].ToString()) ? "[]" : myReader["dtJSON"].ToString();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //    return jsonResult;
        //}

        [AllowAnonymous]
        [HttpPost]
        public Task<bool> InsertFAQAsync(TbFaq changed, int intCurrentUserId)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call proc_InsertFAQ(:_soalanfaq, :_jawapanfaq, :_kategorifaq, :_rekCiptaUserID, :_rekstatus)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_soalanfaq", DbType.String).Value = "Soalan hari ini?";
                        myCmd.Parameters.AddWithValue("_jawapanfaq", DbType.String).Value = "Jawapan hari ini.";
                        myCmd.Parameters.AddWithValue("_kategorifaq", DbType.String).Value = "1";
                        myCmd.Parameters.AddWithValue("_rekCiptaUserID", DbType.Int32).Value = 1;
                        myCmd.Parameters.AddWithValue("_rekstatus", DbType.String).Value = "A";

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : InsertFAQAsync - {0}", ex);
                return Task.FromResult(false); ;
            }
            finally
            {
            }

            return Task.FromResult(true);
        }

        [AllowAnonymous]
        [HttpPut]
        public Task<bool> UpdateFAQAsync(int _faqid, TbFaq changed, int intCurrentUserId)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call proc_updatefaq(:_faqid, :_soalanfaq, :_jawapanfaq, :_kategorifaq, :_rekUbahUserID, :_rekstatus)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_faqid", DbType.Int32).Value = _faqid;
                        myCmd.Parameters.AddWithValue("_soalanfaq", DbType.String).Value = changed.Soalanfaq;
                        myCmd.Parameters.AddWithValue("_jawapanfaq", DbType.String).Value = changed.Jawapanfaq;
                        myCmd.Parameters.AddWithValue("_kategorifaq", DbType.String).Value = changed.Kategorifaq;
                        myCmd.Parameters.AddWithValue("_rekUbahUserID", DbType.Int32).Value = intCurrentUserId;
                        myCmd.Parameters.AddWithValue("_rekstatus", DbType.String).Value = changed.Rekstatus;

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : UpdateFAQAsync - {0}", ex);
                return Task.FromResult(false); ;
            }
            finally
            {
            }

            return Task.FromResult(true);
        }

        [AllowAnonymous]
        [HttpPut]
        public Task<bool> RemoveFAQAsync(int _faqid)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call proc_deletefaq(:_faqid, :_rekstatus)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_faqid", _faqid);
                        myCmd.Parameters.AddWithValue("_rekstatus", DbType.String).Value = "TA";
                        //myCmd.Parameters.AddWithValue("_rekUbahUserID", 1);

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : RemoveFAQAsync - {0}", ex);
                return Task.FromResult(false); ;
            }
            finally
            {
            }

            return Task.FromResult(true);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<string> ListFAQAsync()
        {
            List<TbFaq> arrItem = new List<TbFaq>();
            string jsonStr = "";
            try
            {
                using (var myConn = new NpgsqlConnection(_dbConn))
                {
                    await myConn.OpenAsync();

                    // Start a transaction to ensure the refcursor is handled properly
                    using (var transaction = await myConn.BeginTransactionAsync())
                    {
                        // Create a command to call the function that returns a refcursor
                        using (var myCmd = new NpgsqlCommand("SELECT funct_listfaq()", myConn))
                        {
                            // Execute the function and get the refcursor
                            var cursorName = (await myCmd.ExecuteScalarAsync()) as string;

                            if (string.IsNullOrEmpty(cursorName))
                            {
                                throw new Exception("Cursor not returned from the stored procedure.");
                            }

                            // Now, use the cursor to fetch the results
                            using (var fetchCmd = new NpgsqlCommand($"FETCH ALL IN \"{cursorName}\"", myConn))
                            {
                                using (var myReader = await fetchCmd.ExecuteReaderAsync())
                                {
                                    // Loop through all rows returned by the cursor
                                    while (await myReader.ReadAsync())
                                    {
                                        var _item = new TbFaq
                                        {
                                            Faqid = myReader.GetInt32("faqid"),
                                            Kategorifaq = myReader.GetString("kategorifaq"),
                                            Soalanfaq = myReader.GetString("soalanfaq"),
                                            Jawapanfaq = myReader.GetString("jawapanfaq"),
                                            Rekstatus = myReader.GetString("rekstatus"),
                                            Rekcipta = myReader.GetDateTime("rekcipta"),
                                            Rekciptauserid = myReader.GetInt32("rekciptauserid"),
                                            Rekubahuserid = (int)(myReader.IsDBNull(myReader.GetOrdinal("rekubahuserid"))? (int?)null  : myReader.GetInt32("rekubahuserid")),
                                            Rekubah = myReader.GetDateTime("rekubah"),
                                            //Rekubahuserid = myReader.GetInt32("rekubahuserid")                                            
                                        };

                                        arrItem.Add(_item);
                                    }
                                }
                            }
                        }
                        jsonStr = JsonConvert.SerializeObject(arrItem);
                        // Commit the transaction (if applicable)
                        await transaction.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in ListFAQAsync: {ex.Message}");
                // Optionally log the error or handle it according to your needs.
            }

            return jsonStr;
        }

        //public Task<List<TbFaq>> ListFAQAsync()
        //{
        //    List<TbFaq> arrItem = new List<TbFaq>();
        //    TbFaq _item;

        //    try
        //    {
        //        using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
        //        {
        //            using (NpgsqlCommand? myCmd = new NpgsqlCommand("call proc_listfaq(ref_cursor)", myConn))
        //            {
        //                myCmd.CommandType = CommandType.Text;
        //                myConn.Open();
        //                using (NpgsqlDataReader? myReader = myCmd.ExecuteReader())
        //                {
        //                    //Loop every data
        //                    while (myReader.Read())
        //                    {
        //                        _item = new TbFaq();
        //                        _item.Faqid = myReader.GetInt32("faqId");
        //                        _item.Jawapanfaq = myReader.GetString("jawapanfaq");
        //                        //_item.Soalanfaq = myReader.GetString("Soalanfaq");
        //                        //_item.Kategorifaq = myReader.GetString("Kategorifaq");
        //                        //_item.Rekcipta = myReader.IsDBNull("Rekcipta") ? null : myReader.GetDateTime("Rekcipta");
        //                        //_item.Rekciptauserid = myReader.IsDBNull("Rekciptauserid") ? 0 : myReader.GetInt32("Rekciptauserid");
        //                        //_item.Rekubah = myReader.IsDBNull("Rekubah") ? null : myReader.GetDateTime("Rekubah");
        //                        //_item.Rekubahuserid = myReader.IsDBNull("Rekubahuserid") ? 0 : myReader.GetInt32("Rekubahuserid");
        //                        //_item.Rekstatus = myReader.IsDBNull("Rekstatus") ? "" : myReader.GetString("Rekstatus");

        //                        //Add item into list
        //                        arrItem.Add(_item);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception caught : ListFAQAsync - {0}", ex);
        //        return Task.FromResult(arrItem);
        //    }
        //    finally
        //    {
        //    }

        //    return Task.FromResult(arrItem);
        //}
    }
}
