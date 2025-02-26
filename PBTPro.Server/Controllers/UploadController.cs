using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBTPro.Data;

namespace PBTPro.Controllers
{
    public class ChunkMetadata
    {
        public int Index { get; set; }
        public int TotalCount { get; set; }
        public int FileSize { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FileGuid { get; set; }
    }

    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        FileUrlStorageService _fileUrlStorageService;


        //const long MaxFileSize = 4_000_000;
        //readonly string[] documentExtensions = { ".PDF" };

        public UploadController(IWebHostEnvironment hostingEnvironment, FileUrlStorageService fileUrlStorageService)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileUrlStorageService = fileUrlStorageService;
        }

        [HttpPost]
        [Route("UploadFile")]
        [DisableRequestSizeLimit]
        public ActionResult UploadFile(IFormFile ImageUpload, string chunkMetadata)
        {
            var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "tempProfile");
            // Removes temporary files
            RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 5, 0));

            try
            {
                //////if (!string.IsNullOrEmpty(chunkMetadata)) {
                //////    var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                //////    var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + ".tmp");
                //////    if (!Directory.Exists(tempPath))
                //////        Directory.CreateDirectory(tempPath);

                //////    AppendContentToFile(tempFilePath, ImageUpload);

                //////    if (metaDataObject.Index == (metaDataObject.TotalCount - 1)) {
                //////        string fileName = Path.GetFileNameWithoutExtension(metaDataObject.FileName);
                //////        string extension = Path.GetExtension(metaDataObject.FileName);
                //////        string strNewFileName = fileName + "_" + DateTime.Now.ToString("yyMMddhhmmss") + extension;

                //////        //ProcessUploadedFile(tempFilePath, metaDataObject.FileName);
                //////        //_fileUrlStorageService.Add(Guid.Parse(metaDataObject.FileGuid), @"Document\" + metaDataObject.FileName);
                //////        ProcessUploadedFile(tempFilePath, strNewFileName);
                //////        //_fileUrlStorageService.Add(Guid.Parse(metaDataObject.FileGuid), @"\images\Profile\" + strNewFileName);
                //////        _fileUrlStorageService.Add(Guid.Parse(metaDataObject.FileGuid), tempFilePath);

                //////    }
                //////}
                ///
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                    var tempExtension = Path.GetExtension(metaDataObject.FileName);
                    var tempFileName = metaDataObject.FileGuid + "tmp" + tempExtension;
                    var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + "tmp" + tempExtension);
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    AppendContentToFile(tempFilePath, ImageUpload);

                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(metaDataObject.FileName);
                        string extension = Path.GetExtension(metaDataObject.FileName);
                        string strNewFileName = fileName + "_" + DateTime.Now.ToString("yyMMddhhmmss") + extension;

                        ////ProcessUploadedFile(tempFilePath, strNewFileName);
                        //_fileUrlStorageService.Add(Guid.Parse(metaDataObject.FileGuid), @"\images\Profile\" + strNewFileName);
                        _fileUrlStorageService.Add(Guid.Parse(metaDataObject.FileGuid), tempFilePath + ";" + @"\images\tempProfile\" + tempFileName + ";" + strNewFileName);

                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }
        void RemoveTempFilesAfterDelay(string path, TimeSpan delay)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
                //////foreach (var file in dir.GetFiles("*.tmp").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                //////    file.Delete();
                foreach (var file in dir.GetFiles("*tmp.*").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                    file.Delete();
        }

        [HttpPost]
        [Route("TestUpload")]
        public ActionResult TestUpload()
        {

            return Ok();
        }

        [HttpPost]
        public ActionResult ProcessUploadedFile(string tempFilePath, string fileName)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images", "profile");
            var imagePath = Path.Combine(path, fileName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            System.IO.File.Copy(tempFilePath, imagePath);

            return Ok();
        }

        void AppendContentToFile(string path, IFormFile content)
        {
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                content.CopyTo(stream);
            }
        }

        #region added by farhana
        ///tix 056(B) & 057(B)
        [HttpPost]
        [Route("UploadDocument")]
        [DisableRequestSizeLimit]
        public ActionResult UploadDocument(IFormFile DocUpload, string chunkMetadata)
        {
            var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, "document", "tempDocument");
            // Removes temporary files
            //RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 5, 0));

            try
            {
                //var ext = Path.GetExtension(DocUpload.FileName).ToUpperInvariant();
                //var isValidExtenstion = documentExtensions.Contains(ext);
                //var isValidSize = DocUpload.Length <= MaxFileSize;
                //if (!isValidExtenstion || !isValidSize)
                //    throw new InvalidOperationException();

                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                    var tempExtension = Path.GetExtension(metaDataObject.FileName);
                    var tempFileName = metaDataObject.FileGuid + "tmp" + tempExtension;
                    var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + "tmp" + tempExtension);
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    AppendContentToFile(tempFilePath, DocUpload);

                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(metaDataObject.FileName);
                        string extension = Path.GetExtension(metaDataObject.FileName);
                        string strNewFileName = fileName + "_" + DateTime.Now.ToString("yyMMddhhmmss") + extension;

                        _fileUrlStorageService.Add(Guid.Parse(metaDataObject.FileGuid), tempFilePath + ";" + @"\document\tempDocument\" + tempFileName + ";" + strNewFileName);

                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }




        #endregion
    }
}
