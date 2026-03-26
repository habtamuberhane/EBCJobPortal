using System.Diagnostics;
using EBCJobPortal.Models;
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

        public IActionResult Index()
        {
            ViewBag.expiredVacancies = _context.TblJobLists.Where(s => s.ExpiredDate < DateTime.Now.Date).ToListAsync().Result.Count();
            ViewBag.activVacancies = _context.TblJobLists.Where(s => s.ExpiredDate > DateTime.Now.Date).ToListAsync().Result.Count();
            ViewBag.adminUsers = _context.TblJobPortalUsers.ToList().Count();
            ViewBag.applicants = _context.TblApplicants.ToList().Count();
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
