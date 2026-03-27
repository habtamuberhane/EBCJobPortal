using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace EBCJobPortalAdmin.Controllers
{
    [Authorize]
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
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            return RedirectToAction(nameof(Index));
        }

        public ActionResult DocumentViewer(string path, string? methodController, string? method)
        {
            ViewBag.path = NormalizeRequestPath(path);
            ViewBag.controller = methodController;
            ViewBag.action = method;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadEvidenceFile(string path, string? methodController, string? method)
        {
            var normalizedPath = NormalizeRequestPath(path);
            var physicalPath = ResolvePhysicalPath(normalizedPath);

            if (!System.IO.File.Exists(physicalPath))
            {
                return RedirectToAction(nameof(FileNotFound), new
                {
                    path = normalizedPath,
                    methodController,
                    method
                });
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(physicalPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
            return File(bytes, contentType, Path.GetFileName(physicalPath));
        }

        public IActionResult FileNotFound(string path, string? methodController, string? method)
        {
            ViewBag.path = path;
            ViewBag.controller = methodController;
            ViewBag.action = method;
            return View();
        }

        private static string NormalizeRequestPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            var normalizedPath = path.Replace('\\', '/');

            if (normalizedPath.StartsWith("/admin/Files/", StringComparison.OrdinalIgnoreCase))
            {
                normalizedPath = normalizedPath["/admin".Length..];
            }

            if (!normalizedPath.StartsWith("/Files/", StringComparison.OrdinalIgnoreCase))
            {
                normalizedPath = $"/Files/{Path.GetFileName(normalizedPath)}";
            }

            return normalizedPath;
        }

        private static string ResolvePhysicalPath(string normalizedPath)
        {
            var fileName = Path.GetFileName(normalizedPath);
            var primaryPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);

            if (System.IO.File.Exists(primaryPath))
            {
                return primaryPath;
            }

            // Backward compatibility for files that were previously written into wwwroot/Files.
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", fileName);
        }
    }
}
