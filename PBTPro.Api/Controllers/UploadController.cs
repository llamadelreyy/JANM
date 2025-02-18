using Microsoft.AspNetCore.Mvc;
using PBTPro.Api.Controllers.Base;
using PBTPro.DAL;
using PBTPro.DAL.Models;
using System.Text.RegularExpressions;

namespace PBTPro.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    public class UploadController : IBaseController
    {
        protected readonly string? _dbConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UploadController> _logger;
        private readonly string _feature = "MUAT NAIK FAIL";
        const long MaxFileSize = 4_000_000;
        readonly string[] imageExtensions = { ".JPG", ".JPEG", ".GIF", ".PNG" };
        public UploadController(IConfiguration configuration, PBTProDbContext dbContext, ILogger<UploadController> logger) : base(dbContext)
        {
            _dbConn = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> UploadFileAsync(IFormFile myFile)
        {
            try
            {
                if (myFile != null)
                {
                    string DocExt = Path.GetExtension(myFile.FileName).ToUpperInvariant();
                    var isValidExtenstion = imageExtensions.Contains(DocExt);
                    var isValidSize = myFile.Length <= MaxFileSize;
                    if (!isValidExtenstion || !isValidSize)
                        throw new InvalidOperationException();

                    string DocUrl = String.Format("{0}{1}", "document", DocExt);

                    // Write code that saves the 'myFile' file.
                    // Don't rely on or trust the FileName property without validation.
                    await UploadDocument(myFile, "document", DocUrl);

                }
            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpGet]
        public async Task<string> UploadDocument(IFormFile file, string folderName = "", string DocUrl = "")
        {
            ref_doc itm = new ref_doc();

            var fileName = Path.GetFileName(DocUrl);
            var filePath = Path.Combine(new[] { Directory.GetCurrentDirectory(), "wwwroot", RemoveSymbols(folderName), fileName });

            Directory.CreateDirectory(Path.Combine(new[] { Directory.GetCurrentDirectory(), "wwwroot", RemoveSymbols(folderName) }));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                itm.pathurl = filePath;
                itm.filename = fileName;

            }
            return fileName;
        }
        
        [HttpGet]
        public string RemoveSymbols(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9\s]", "");
        }

        [HttpGet]
        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
                );
        }
    }
}
