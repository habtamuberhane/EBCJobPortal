using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using EBCJobPortalAdmin.Models;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EBCJobPortalAdmin.Controllers
{
    [Authorize]
    public class ApplicantsController : Controller
    {
        private readonly EbcJobPortalContext _context;
        private readonly INotyfService _notifyService;

        public ApplicantsController(EbcJobPortalContext context, INotyfService notyfService)
        {
            _context = context;
            _notifyService = notyfService;
        }

        public async Task<IActionResult> Index([FromQuery] ApplicantsListModel? model = null)
        {
            return View(await BuildApplicantsListModelAsync(model));
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult FilterIndex(ApplicantsListModel model)
        {
            return RedirectToAction(nameof(Index), new
            {
                model.JobId,
                model.Regid,
                model.Gender,
                model.DisabilityStatus,
                model.EducationLevel,
                model.MinimumCgpa,
                model.MinimumExperienceYears,
                PageNumber = 1
            });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblApplicant = await _context.TblApplicants
                .AsNoTracking()
                .Include(t => t.Job)
                .Include(t => t.Reg)
                .FirstOrDefaultAsync(m => m.ApplyId == id);

            if (tblApplicant == null)
            {
                return NotFound();
            }

            return View(tblApplicant);
        }

        public IActionResult Create()
        {
            ViewData["JobId"] = new SelectList(_context.TblJobLists, "JobId", "JobId");
            ViewData["Regid"] = new SelectList(_context.TblRegions, "Regid", "RegionName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplyId,JobId,FullName,PhoneNumber,EmailAddress,Gender,BirthDate,Institution,Regid,ZoneSubcity,Worede,HouseNumber,MaritalStatus,PositionTitle,EducationLevel,EducationField,GraduationYear,Cgpa,NumberofExprianceYears,CurrentWorkingCompany,MonthlySalary,CompanyPhoneNumber,CompanyPostNumber,Nation,Cvfile,CvShorttermTranings,ReasonForDisablity,AreYouDisable,RegistrationDate,JobLocation")] TblApplicant tblApplicant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblApplicant);
                await _context.SaveChangesAsync();
                _notifyService.Success("Applicant record created successfully.");
                return RedirectToAction(nameof(Index));
            }

            ViewData["JobId"] = new SelectList(_context.TblJobLists, "JobId", "JobId", tblApplicant.JobId);
            ViewData["Regid"] = new SelectList(_context.TblRegions, "Regid", "RegionName", tblApplicant.Regid);
            return View(tblApplicant);
        }

        public IActionResult Edit(int? id)
        {
            _notifyService.Warning("Applicant records are view-only. Editing is disabled for administrators.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ApplyId,JobId,FullName,PhoneNumber,EmailAddress,Gender,BirthDate,Institution,Regid,ZoneSubcity,Worede,HouseNumber,MaritalStatus,PositionTitle,EducationLevel,EducationField,GraduationYear,Cgpa,NumberofExprianceYears,CurrentWorkingCompany,MonthlySalary,CompanyPhoneNumber,CompanyPostNumber,Nation,Cvfile,CvShorttermTranings,ReasonForDisablity,AreYouDisable,RegistrationDate,JobLocation")] TblApplicant tblApplicant)
        {
            _notifyService.Warning("Applicant records are view-only. Editing is disabled for administrators.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblApplicant = await _context.TblApplicants
                .AsNoTracking()
                .Include(t => t.Job)
                .Include(t => t.Reg)
                .FirstOrDefaultAsync(m => m.ApplyId == id);

            if (tblApplicant == null)
            {
                return NotFound();
            }

            return View(tblApplicant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblApplicant = await _context.TblApplicants.FindAsync(id);

            if (tblApplicant != null)
            {
                _context.TblApplicants.Remove(tblApplicant);
                await _context.SaveChangesAsync();
                _notifyService.Success("Applicant record deleted successfully.");
            }
            else
            {
                _notifyService.Warning("Applicant record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<ApplicantsListModel> BuildApplicantsListModelAsync(ApplicantsListModel? filters = null)
        {
            filters ??= new ApplicantsListModel();
            filters.PageSize = ApplicantsListModel.DefaultPageSize;
            filters.PageNumber = filters.PageNumber <= 0 ? 1 : filters.PageNumber;

            var jobs = await _context.TblJobLists
                .AsNoTracking()
                .OrderBy(job => job.JobTitle)
                .Select(job => new SelectListItem
                {
                    Value = job.JobId.ToString(),
                    Text = job.JobTitle ?? $"Job #{job.JobId}"
                })
                .ToListAsync();

            var regions = await _context.TblRegions
                .AsNoTracking()
                .OrderBy(region => region.RegionName)
                .Select(region => new SelectListItem
                {
                    Value = region.Regid.ToString(),
                    Text = region.RegionName ?? $"Region #{region.Regid}"
                })
                .ToListAsync();

            var allApplicants = await _context.TblApplicants
                .AsNoTracking()
                .Include(applicant => applicant.Job)
                .Include(applicant => applicant.Reg)
                .OrderByDescending(applicant => applicant.RegistrationDate ?? DateTime.MinValue)
                .ThenByDescending(applicant => applicant.ApplyId)
                .ToListAsync();

            var filteredApplicants = allApplicants.AsEnumerable();

            if (filters.JobId.HasValue)
            {
                filteredApplicants = filteredApplicants.Where(applicant => applicant.JobId == filters.JobId.Value);
            }

            if (filters.Regid.HasValue)
            {
                filteredApplicants = filteredApplicants.Where(applicant => applicant.Regid == filters.Regid.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Gender))
            {
                filteredApplicants = filteredApplicants.Where(applicant => string.Equals(applicant.Gender, filters.Gender, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filters.DisabilityStatus))
            {
                filteredApplicants = filters.DisabilityStatus.Equals("true", StringComparison.OrdinalIgnoreCase)
                    ? filteredApplicants.Where(applicant => applicant.AreYouDisable == true)
                    : filteredApplicants.Where(applicant => applicant.AreYouDisable != true);
            }

            if (!string.IsNullOrWhiteSpace(filters.EducationLevel))
            {
                filteredApplicants = filteredApplicants.Where(applicant => string.Equals(applicant.EducationLevel, filters.EducationLevel, StringComparison.OrdinalIgnoreCase));
            }

            if (filters.MinimumCgpa.HasValue)
            {
                filteredApplicants = filteredApplicants.Where(applicant => (applicant.Cgpa ?? 0) >= filters.MinimumCgpa.Value);
            }

            if (filters.MinimumExperienceYears.HasValue)
            {
                filteredApplicants = filteredApplicants.Where(applicant => (applicant.NumberofExprianceYears ?? 0) >= filters.MinimumExperienceYears.Value);
            }

            var filteredApplicantList = filteredApplicants.ToList();
            var totalApplicants = filteredApplicantList.Count;
            var totalPages = totalApplicants == 0
                ? 1
                : (int)Math.Ceiling(totalApplicants / (double)filters.PageSize);

            if (filters.PageNumber > totalPages)
            {
                filters.PageNumber = totalPages;
            }

            var applicants = filteredApplicantList
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var genderOptions = allApplicants
                .Select(applicant => applicant.Gender?.Trim())
                .Where(gender => !string.IsNullOrWhiteSpace(gender))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(gender => gender)
                .Select(gender => new SelectListItem
                {
                    Value = gender,
                    Text = gender
                })
                .ToList();

            var educationLevels = allApplicants
                .Select(applicant => applicant.EducationLevel?.Trim())
                .Where(level => !string.IsNullOrWhiteSpace(level))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(level => level)
                .Select(level => new SelectListItem
                {
                    Value = level,
                    Text = level
                })
                .ToList();

            return new ApplicantsListModel
            {
                JobId = filters.JobId,
                Regid = filters.Regid,
                Gender = filters.Gender,
                DisabilityStatus = filters.DisabilityStatus,
                EducationLevel = filters.EducationLevel,
                MinimumCgpa = filters.MinimumCgpa,
                MinimumExperienceYears = filters.MinimumExperienceYears,
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize,
                TotalApplicants = totalApplicants,
                TotalVacanciesCovered = filteredApplicantList
                    .Select(applicant => applicant.Job?.JobTitle)
                    .Where(title => !string.IsNullOrWhiteSpace(title))
                    .Distinct()
                    .Count(),
                TotalDisabilityDisclosed = filteredApplicantList.Count(applicant => applicant.AreYouDisable == true),
                AverageExperience = filteredApplicantList.Count == 0
                    ? 0
                    : Math.Round(filteredApplicantList.Average(applicant => Convert.ToDouble(applicant.NumberofExprianceYears ?? 0)), 1),
                AverageCgpa = filteredApplicantList.Count(applicant => applicant.Cgpa.HasValue) == 0
                    ? 0
                    : Math.Round(filteredApplicantList.Where(applicant => applicant.Cgpa.HasValue).Average(applicant => applicant.Cgpa ?? 0), 2),
                Jobs = jobs,
                Regions = regions,
                Genders = genderOptions,
                DisabilityStatuses = new List<SelectListItem>
                {
                    new() { Value = "true", Text = "With disability" },
                    new() { Value = "false", Text = "Without disability" }
                },
                EducationLevels = educationLevels,
                Applicants = applicants
            };
        }
    }
}
