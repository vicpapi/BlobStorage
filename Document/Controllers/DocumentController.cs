using Document.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Document.Controllers
{
    public class DocumentController : Controller
    {

        private readonly IBlobRepository<CloudBlockBlob> _blobRepository;
        private readonly ILog4NetRepository _loggerRepository;

        public DocumentController(IBlobRepository<CloudBlockBlob> blobRepository, ILog4NetRepository loggerRepository)
        {
            _blobRepository = blobRepository;
            _loggerRepository = loggerRepository;
        }

        // GET: Document
        public async Task<IActionResult> Index()
        {
            List<Core.Models.Document> documents = new List<Core.Models.Document>();
            var list = await _blobRepository.GetFilesAsync();

            foreach (var item in list)
            {
                documents.Add(
                            new Core.Models.Document
                            {
                                Name = item
                            }
                    );
            }

            return View(documents);
        }

        // GET: Document/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Document/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document.Core.Models.Document document, IFormFile file)
        {
            try
            {
                // TODO: Add insert logic here
                using var ms = new MemoryStream();
                file.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();

                var arrayName = file.FileName.Split('.');


                if (arrayName.Length > 0)
                {
                    document.Name = $"{document.Name}.{arrayName[arrayName.Length - 1]}";
                }

                await _blobRepository.UploadFromByteArrayAsync(document.Name, fileBytes);

                _loggerRepository.LogInfo("Save File");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<FileResult> Download(string fileName)
        {
            var bytes = await _blobRepository.DownloadBlobToByteArrayAsync(fileName);

            return File(bytes, "application/octet-stream", fileName);

        }

        public ActionResult Delete(string fileName)
        {
            var product = new Core.Models.Document
            {
                Name = fileName
            };

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string name)
        {
            await _blobRepository.DeleteBlobByFileName(name);

            return RedirectToAction(nameof(Index));
        }
    }
}