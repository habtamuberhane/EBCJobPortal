using System.Diagnostics;
using EBCJobPortal.Models;
using EBCJobPortal.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBCJobPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EbcJobPortalContext _context;

        public HomeController(ILogger<HomeController> logger, EbcJobPortalContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Using a single normalized date value keeps every query aligned to the same request-time boundary.
            var today = DateTime.Today;

            // Read-only portal queries should be no-tracking to reduce EF Core overhead and keep response rendering lightweight.
            // Each query is awaited before the next one starts because a single DbContext instance cannot execute concurrent operations.
            var activeVacancyCount = await _context.TblJobLists
                .AsNoTracking()
                .CountAsync(job => !job.ExpiredDate.HasValue || job.ExpiredDate.Value.Date >= today);

            // The featured jobs list is sorted by most recent posting date so the landing page feels fresh on every visit.
            var featuredJobs = await _context.TblJobLists
                .AsNoTracking()
                .Where(job => !job.ExpiredDate.HasValue || job.ExpiredDate.Value.Date >= today)
                .OrderByDescending(job => job.PostedDate ?? DateTime.MinValue)
                .ThenByDescending(job => job.JobId)
                .Take(4)
                .ToListAsync();

            // A typed view model makes the Razor page easier to maintain than a collection of dynamic ViewBag values.
            var viewModel = new HomeIndexViewModel
            {
                ActiveVacancyCount = activeVacancyCount,
                FeaturedJobs = featuredJobs
            };

            return View(viewModel);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
