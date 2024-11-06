using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using Npgsql;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using PBTPro.Shared.Models;
using PBTPro.Shared.Models.CommonService;
using System.Data;
using System.Reflection;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FaqController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly ILogger<FaqController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _module = "Faq";
        private List<FaqProp> _Faq { get; set; }


        public FaqController(PBTProDbContext dbContext, ILogger<FaqController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = configuration.GetValue<string>("ConnectionStrings");
        }

        [HttpGet]
        [AllowAnonymous]
        public string ListFaq()
        {
            string jsonResult = "[]";

            try
             {
                using (NpgsqlConnection? myConn = new NpgsqlConnection($"{_dbConn}"))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("listFAQ()", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myConn.Open();
                        using (NpgsqlDataReader? myReader = myCmd.ExecuteReader())
                        {
                            //Loop every data
                            while (myReader.Read())
                            {
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

        [HttpPost]
        public int CreateFaq([FromBody] string strJSONAdd, [FromQuery] string userId = "")
        {
            int intId = 0;
            FaqProp faq = JsonConvert.DeserializeObject<FaqProp>(strJSONAdd);

            try
            {
                using (NpgsqlConnection? myConn = new NpgsqlConnection($"{_dbConn}"))
                {
                    using (NpgsqlCommand? myCmd = new NpgsqlCommand("InsertFaq", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myConn.Open();
                        using (NpgsqlDataReader? myReader = myCmd.ExecuteReader())
                        {
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return intId;
        }

        [HttpGet]
        public Task<List<FaqProp>> GetFaqAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Faq);
        }
    }
}
