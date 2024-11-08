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

        [AllowAnonymous]
        [HttpGet]
        public string GetListFAQ()
        {
            string jsonResult = "[]";
            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection(_dbConn))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("getListFAQs", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myConn.Open();
                        using (NpgsqlDataReader? myReader = myCmd.ExecuteReader())
                        {
                            //Loop every data
                            while (myReader.Read())
                            {
                                myReader.Read();
                                jsonResult = string.IsNullOrEmpty(myReader["dtJSON"].ToString()) ? "[]" : myReader["dtJSON"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return jsonResult;
        }

        //public Task<bool> InsertFAQAsync(TbFaq changed, int intCurrentUserId)
        //{
        //    try
        //    {
        //        using (MySqlConnection? myConn = new MySqlConnection(_dbConn))
        //        {
        //            using (MySqlCommand? myCmd = new MySqlCommand("SELECT * FROM Insert_FAQ()", myConn))
        //            {
        //                myCmd.CommandType = CommandType.StoredProcedure;
        //                myCmd.Parameters.AddWithValue("_namaSesi", Jfunc.Tstring(changed.namaSesi));
        //                myCmd.Parameters.AddWithValue("_tarikhSesiDari", Convert.ToDateTime(changed.tarikhSesiDari).ToString("yyyy-MM-dd"));
        //                myCmd.Parameters.AddWithValue("_tarikhSesiHingga", Convert.ToDateTime(changed.tarikhSesiHingga).ToString("yyyy-MM-dd"));
        //                myCmd.Parameters.AddWithValue("_rekCiptaUserID", intCurrentUserId);

        //                myConn.Open();
        //                myCmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception caught : InsertFAQAsync - {0}", ex);
        //        return Task.FromResult(false); ;
        //    }
        //    finally
        //    {
        //    }

        //    return Task.FromResult(true);
        //}
    }
}
