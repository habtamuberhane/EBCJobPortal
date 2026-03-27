using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using EBCJobPortal.Models;
using EBCJobPortal.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EBCJobPortal.Controllers
{
    public class AllJobsController : Controller
    {
        private readonly EbcJobPortalContext _context;
        private readonly INotyfService _notifyService;
        private readonly IConfiguration _configuration;

        public AllJobsController(
            EbcJobPortalContext context,
            INotyfService notyfService,
            IConfiguration configuration
        )
        {
            _context = context;
            _notifyService = notyfService;
            _configuration = configuration;
        }

        // GET: AllJobs
        public async Task<IActionResult> Index(int page = 1)
        {
            string domains = _configuration.GetSection("mySettings:url").Value ?? string.Empty;
            const int pageSize = VacanciesIndexViewModel.DefaultPageSize;
            var normalizedPage = page < 1 ? 1 : page;
            var activeJobsQuery = _context
                .TblJobLists.Where(s => !s.ExpiredDate.HasValue || s.ExpiredDate >= DateTime.Now.Date);
            var totalJobs = await activeJobsQuery.CountAsync();
            var jobs = await activeJobsQuery
                    .OrderByDescending(s => s.PostedDate ?? DateTime.MinValue)
                    .ThenByDescending(s => s.JobId)
                    .Skip((normalizedPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            ViewBag.title = "Open Vacancies";
            ViewBag.description = "Explore current job opportunities at Ethiopian Broadcasting Corporation.";
            ViewBag.keywords = "EBC jobs, open vacancies, Ethiopian Broadcasting Corporation careers";
            ViewBag.url = domains + "/AllJobs/Index";
            ViewBag.image = domains + "/assets/vacancy_2.jpg";
            ViewBag.canonical = domains + "/AllJobs/Index";
            ViewBag.facebooktag = domains + "/assets/vacancy_2.jpg";
            return View(new VacanciesIndexViewModel
            {
                Jobs = jobs,
                PageNumber = normalizedPage,
                PageSize = pageSize,
                TotalJobs = totalJobs
            });
        }

        public IActionResult ExpiredJobs()
        {
            return RedirectToAction(nameof(Index));
        }

        // GET: AllJobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblJobList = await _context.TblJobLists.FirstOrDefaultAsync(m => m.JobId == id);
            if (tblJobList == null)
            {
                return NotFound();
            }

            return View(tblJobList);
        }

        // GET: AllJobs/Create
        public async Task<IActionResult> Create(int id)
        {
            var job = await _context.TblJobLists
                .AsNoTracking()
                .FirstOrDefaultAsync(jobItem => jobItem.JobId == id && (!jobItem.ExpiredDate.HasValue || jobItem.ExpiredDate >= DateTime.Now.Date));

            if (job is null)
            {
                _notifyService.Error("The selected vacancy is no longer available.");
                return RedirectToAction(nameof(Index));
            }

            var model = new ApplicantModel
            {
                JobId = id,
                JobTitle = job.JobTitle,
                BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-18))
            };

            await PopulateApplicantSelectionsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(ApplicantModel applicantModel)
        {
            var job = await _context.TblJobLists
                .AsNoTracking()
                .FirstOrDefaultAsync(jobItem => jobItem.JobId == applicantModel.JobId && (!jobItem.ExpiredDate.HasValue || jobItem.ExpiredDate >= DateTime.Now.Date));

            if (job is null)
            {
                _notifyService.Error("The selected vacancy is no longer available.");
                return RedirectToAction(nameof(Index));
            }

            applicantModel.JobTitle = job.JobTitle;

            if (!ModelState.IsValid)
            {
                await PopulateApplicantSelectionsAsync(applicantModel);
                ViewData["FormStatus"] = "warning";
                ViewData["FormMessage"] = "Please complete all required fields before submitting your application.";
                _notifyService.Warning("Please complete all required fields before submitting your application.");
                return View(applicantModel);
            }

            try
            {
                var fileExtension = string.Empty;
                if (applicantModel.Cvfile is not null && applicantModel.Cvfile.Length > 0)
                {
                    fileExtension = Path.GetExtension(applicantModel.Cvfile.FileName);
                    if (!string.Equals(fileExtension, ".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        await PopulateApplicantSelectionsAsync(applicantModel);
                        ViewData["FormStatus"] = "error";
                        ViewData["FormMessage"] = "Only PDF files are allowed for the CV upload.";
                        _notifyService.Error("Only PDF files are allowed for the CV upload.");
                        return View(applicantModel);
                    }
                }

                TblApplicant applicants = new TblApplicant();
                applicants.JobId = applicantModel.JobId;
                applicants.Cgpa = applicantModel.Cgpa;
                applicants.FullName = applicantModel.FullName;
                applicants.Gender = applicantModel.Gender;
                applicants.Nation = applicantModel.Nation;
                applicants.RegistrationDate = DateTime.Now;
                applicants.AreYouDisable = string.Equals(applicantModel.Disable, "Yes", StringComparison.OrdinalIgnoreCase);
                applicants.ReasonForDisablity = null;
                applicants.CurrentWorkingCompany = applicantModel.CurrentWorkingCompany;
                applicants.NumberofExprianceYears = applicantModel.NumberofExprianceYears;
                applicants.GraduationYear = applicantModel.GraduationYear;
                applicants.Institution = applicantModel.Institution;
                applicants.BirthDate = applicantModel.BirthDate;
                applicants.MonthlySalary = applicantModel.MonthlySalary;
                applicants.PhoneNumber = applicantModel.PhoneNumber;
                applicants.HouseNumber = applicantModel.HouseNumber;
                applicants.ZoneSubcity = applicantModel.ZoneSubcity;
                applicants.Regid = applicantModel.Regid;
                applicants.Worede = applicantModel.Worede;
                applicants.MaritalStatus = applicantModel.MaritalStatus;
                applicants.PositionTitle = applicantModel.PositionTitle;
                applicants.EducationLevel = applicantModel.EducationLevel;
                applicants.EducationField = applicantModel.EducationField;
                // Save directly to the sibling Admin project's shared file library.
                // The admin app serves /Files from its root-level Files folder, so uploads must land there.
                string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "EBCJobPortalAdmin", "Files");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (applicantModel.Cvfile is not null && applicantModel.Cvfile.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid():N}{fileExtension.ToLowerInvariant()}";
                    string fileNameWithPath = Path.Combine(path, fileName);
                    await using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await applicantModel.Cvfile.CopyToAsync(stream);
                    }
                    // Store path relative to the Admin base URL (e.g., /Files/filename.pdf)
                    string dbPath = "/Files/" + fileName;
                    applicants.Cvfile = dbPath;
                }

                _context.TblApplicants.Add(applicants);
                int saved = await _context.SaveChangesAsync();
                if (saved > 0)
                {
                    if (applicantModel.JobId.HasValue)
                    {
                        MarkJobAsApplied(applicantModel.JobId.Value);
                    }

                    TempData["ApplicationStatus"] = "success";
                    TempData["ApplicationMessage"] = $"Application submitted successfully for {job.JobTitle}.";
                    _notifyService.Success("Application submitted successfully.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    await PopulateApplicantSelectionsAsync(applicantModel);
                    ViewData["FormStatus"] = "error";
                    ViewData["FormMessage"] = "Your application could not be submitted. Please try again.";
                    _notifyService.Error(
                        "Your application isn't successfully submitted. Please try again."
                    );
                    return View(applicantModel);
                }
            }
            catch (Exception)
            {
                await PopulateApplicantSelectionsAsync(applicantModel);
                ViewData["FormStatus"] = "error";
                ViewData["FormMessage"] = "Your application could not be submitted. Please try again.";
                _notifyService.Error("Your application could not be submitted. Please try again.");
                return View(applicantModel);
            }
        }

        // GET: AllJobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblJobList = await _context.TblJobLists.FindAsync(id);
            if (tblJobList == null)
            {
                return NotFound();
            }
            return View(tblJobList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("JobId,RequiredNumber,PostedDate,ExpiredDate,JobTitle,JobDescription")]
                TblJobList tblJobList
        )
        {
            if (id != tblJobList.JobId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblJobList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblJobListExists(tblJobList.JobId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tblJobList);
        }

        // GET: AllJobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblJobList = await _context.TblJobLists.FirstOrDefaultAsync(m => m.JobId == id);
            if (tblJobList == null)
            {
                return NotFound();
            }

            return View(tblJobList);
        }

        // POST: AllJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblJobList = await _context.TblJobLists.FindAsync(id);
            if (tblJobList != null)
            {
                _context.TblJobLists.Remove(tblJobList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblJobListExists(int id)
        {
            return _context.TblJobLists.Any(e => e.JobId == id);
        }

        private void MarkJobAsApplied(int jobId)
        {
            var appliedJobIds = GetAppliedJobIds();
            appliedJobIds.Add(jobId);

            HttpContext.Session.SetString("AppliedJobs", string.Join(",", appliedJobIds.OrderBy(id => id)));
        }

        private HashSet<int> GetAppliedJobIds()
        {
            var sessionValue = HttpContext.Session.GetString("AppliedJobs");

            if (string.IsNullOrWhiteSpace(sessionValue))
            {
                return new HashSet<int>();
            }

            return sessionValue
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(value => int.TryParse(value, out var parsedId) ? parsedId : (int?)null)
                .Where(parsedId => parsedId.HasValue)
                .Select(parsedId => parsedId!.Value)
                .ToHashSet();
        }

        private async Task PopulateApplicantSelectionsAsync(ApplicantModel model)
        {
            model.Regions = await _context.TblRegions
                .AsNoTracking()
                .OrderBy(region => region.RegionName)
                .Select(region => new SelectListItem
                {
                    Value = region.Regid.ToString(),
                    Text = region.RegionName,
                })
                .ToListAsync();

            model.Nations = new List<SelectListItem>
            {
                new() { Value = "Ethiopian", Text = "Ethiopian" },
                new() { Value = "Eritrean", Text = "Eritrean" },
                new() { Value = "Kenyan", Text = "Kenyan" },
                new() { Value = "Somali", Text = "Somali" },
                new() { Value = "South Sudanese", Text = "South Sudanese" },
                new() { Value = "Sudanese", Text = "Sudanese" },
                new() { Value = "Other", Text = "Other" }
            };

            model.EducationLevels = new List<SelectListItem>
            {
                new() { Value = "Diploma", Text = "Diploma" },
                new() { Value = "Advanced Diploma", Text = "Advanced Diploma" },
                new() { Value = "Bachelor's Degree", Text = "Bachelor's Degree" },
                new() { Value = "Master's Degree", Text = "Master's Degree" },
                new() { Value = "PhD", Text = "PhD" }
            };
        }
    }

    public class MaritalStatus
    {
        public string? name { get; set; }
    }

    public class Gender
    {
        public string? name { get; set; }
    }

    public class DisablityStatus
    {
        public string? name { get; set; }
    }
}
