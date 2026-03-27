using System.Diagnostics;
using EBCJobPortalAdmin.Models;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBCJobPortalAdmin.Controllers;

[Authorize]
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
        var today = DateTime.Today;
        try
        {
            // Read-only dashboard metrics use no-tracking queries to keep the admin landing page efficient.
            var totalJobs = await _context.TblJobLists
                .AsNoTracking()
                .CountAsync();

            var totalApplicants = await _context.TblApplicants
                .AsNoTracking()
                .CountAsync();

            var activeVacancies = await _context.TblJobLists
                .AsNoTracking()
                .CountAsync(job => !job.ExpiredDate.HasValue || job.ExpiredDate.Value.Date >= today);

            var viewModel = new AdminDashboardViewModel
            {
                TotalJobs = totalJobs,
                TotalApplicants = totalApplicants,
                ActiveVacancies = activeVacancies,
                IsDatabaseAvailable = true
            };

            return View(viewModel);
        }
        catch (SqlException ex)
        {
            // SQL connectivity failures should not crash the dashboard shell during local setup issues.
            _logger.LogError(ex, "Unable to load admin dashboard metrics because SQL Server is unavailable.");

            return View(new AdminDashboardViewModel
            {
                IsDatabaseAvailable = false,
                StatusMessage = "SQL Server is currently unavailable. You are signed in, but dashboard data could not be loaded."
            });
        }
        catch (InvalidOperationException ex)
        {
            // EF can surface provider bootstrapping issues as InvalidOperationException, so we handle them the same way.
            _logger.LogError(ex, "Unable to load admin dashboard metrics because the data store is unavailable.");

            return View(new AdminDashboardViewModel
            {
                IsDatabaseAvailable = false,
                StatusMessage = "The admin data store is currently unavailable. You are signed in, but dashboard data could not be loaded."
            });
        }
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
