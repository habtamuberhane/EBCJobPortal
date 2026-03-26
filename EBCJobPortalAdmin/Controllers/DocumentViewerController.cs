using Microsoft.AspNetCore.Mvc;
using EBCJobPortalAdmin.Filters;
using Microsoft.AspNetCore.StaticFiles;

namespace EBCJobPortalAdmin.Controllers
{
    [CheckSessionIsAvailable]
    public class DocumentViewerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Edit(int id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Delete(int id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult DocumentViewer(string path, string? methodController, string? method)
        {
            ViewBag.path = path;
            ViewBag.controller = methodController;
            ViewBag.action = method;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DownloadEvidenceFile(string path, string? methodController, string? method)
        {
            string filename = path.Substring(7);
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "admin\\", filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filepath, out var contenttype))
            {
                contenttype = "application/octet-stream";
            }
            if (!System.IO.File.Exists(filepath))
            {
                return RedirectToAction(nameof(FileNotFound), new { path = path, methodController = methodController, method = method });
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(bytes, contenttype, Path.GetFileName(filepath));

        }

        public async Task<IActionResult>? FileNotFound(string path, string? methodController, string? method)
        {
            ViewBag.controller = methodController;
            ViewBag.action = method;
            return View();
        }

    }
}
