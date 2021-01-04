using AspNetCoreTest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            this._environment = environment;
        }

        public IActionResult Index()
        {
            var filePaths = Directory.GetFiles(Path.Combine(this._environment.WebRootPath, "Files/"));
            var files = filePaths.Select(p => new FileModel { FileName = Path.GetFileName(p) }).ToList();
            return View(files);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> UploadFile(IFormFile file,string name)
        {
            if (file == null || file.Length == 0)
            {
                return Content("请选择文件");
            }
            var path = $"{Path.Combine(this._environment.WebRootPath, "Files/")}{file.FileName}";
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return RedirectToAction("Index");
        }

        public IActionResult DownloadFile(string fileName)
        {
            var path = $"{Path.Combine(this._environment.WebRootPath, "Files/")}{fileName}";
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
