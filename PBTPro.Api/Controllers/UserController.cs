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
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call InsertFAQ(:_soalanfaq, :_jawapanfaq, :_kategorifaq, :_rekCiptaUserID, :_rekstatus)", myConn))
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
        public Task<bool> UpdateFAQAsync(TbFaq changed, int intCurrentUserId)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call UpdateFAQ(:_soalanfaq, :_jawapanfaq, :_kategorifaq, :_rekUbahUserID, :_rekstatus)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_soalanfaq", DbType.String).Value = "Soalan hari ini?"; //changed.SoalanFaq;
                        myCmd.Parameters.AddWithValue("_jawapanfaq", DbType.String).Value = "Jawapan hari ini."; //changed.Jawapanfaq;
                        myCmd.Parameters.AddWithValue("_kategorifaq", DbType.String).Value = "1";
                        myCmd.Parameters.AddWithValue("_rekUbahUserID", DbType.Int32).Value = 1;
                        myCmd.Parameters.AddWithValue("_rekstatus", DbType.String).Value = "A";

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
        public Task<bool> RemoveFAQAsync(TbFaq changed, int intCurrentUserId)
        {
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call DeleteFAQ(:_faqId, :_rekUbahUserID, :_rekstatus)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.Parameters.AddWithValue("_faqId", changed.Faqid);
                        myCmd.Parameters.AddWithValue("_rekstatus", DbType.String).Value = "TA";
                        myCmd.Parameters.AddWithValue("_rekUbahUserID", 1);

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
        public Task<List<TbFaq>> ListFAQAsync()
        {
            List<TbFaq> arrItem = new List<TbFaq>();
            TbFaq _item;

            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("call ListFAQ(faqId, jawapanfaq)", myConn))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myConn.Open();
                        using (NpgsqlDataReader? myReader = myCmd.ExecuteReader())
                        {
                            //Loop every data
                            while (myReader.Read())
                            {
                                _item = new TbFaq();
                                _item.Faqid = myReader.GetInt32("faqId");
                                _item.Jawapanfaq = myReader.GetString("jawapanfaq");
                                //_item.Soalanfaq = myReader.GetString("Soalanfaq");
                                //_item.Kategorifaq = myReader.GetString("Kategorifaq");
                                //_item.Rekcipta = myReader.IsDBNull("Rekcipta") ? null : myReader.GetDateTime("Rekcipta");
                                //_item.Rekciptauserid = myReader.IsDBNull("Rekciptauserid") ? 0 : myReader.GetInt32("Rekciptauserid");
                                //_item.Rekubah = myReader.IsDBNull("Rekubah") ? null : myReader.GetDateTime("Rekubah");
                                //_item.Rekubahuserid = myReader.IsDBNull("Rekubahuserid") ? 0 : myReader.GetInt32("Rekubahuserid");
                                //_item.Rekstatus = myReader.IsDBNull("Rekstatus") ? "" : myReader.GetString("Rekstatus");

                                //Add item into list
                                arrItem.Add(_item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : ListFAQAsync - {0}", ex);
                return Task.FromResult(arrItem);
            }
            finally
            {
            }

            return Task.FromResult(arrItem);
        }
    }
}
