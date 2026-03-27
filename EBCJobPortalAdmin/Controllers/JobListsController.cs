using AspNetCoreHero.ToastNotification.Abstractions;
using EBCJobPortalAdmin.Models;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBCJobPortalAdmin.Controllers;

[Authorize]
public class JobListsController : Controller
{
    private readonly EbcJobPortalContext _context;
    private readonly INotyfService _notifyService;

    public JobListsController(EbcJobPortalContext context, INotyfService service)
    {
        _notifyService = service;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.TblJobLists
            .AsNoTracking()
            .Where(job => !job.ExpiredDate.HasValue || job.ExpiredDate > DateTime.Now.Date)
            .OrderByDescending(job => job.PostedDate ?? DateTime.MinValue)
            .ThenByDescending(job => job.JobId)
            .ToListAsync());
    }

    public async Task<IActionResult> ExpiredJobs()
    {
        return View(await _context.TblJobLists
            .AsNoTracking()
            .Where(job => job.ExpiredDate < DateTime.Now.Date)
            .OrderByDescending(job => job.JobId)
            .ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var tblJobList = await _context.TblJobLists
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.JobId == id);

        if (tblJobList is null)
        {
            return NotFound();
        }

        return View(tblJobList);
    }

    public IActionResult Create()
    {
        return View(new JobModel
        {
            PostedDate = DateTime.Now,
            ExpiredDate = DateTime.Now.AddDays(10)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var tblJobList = new TblJobList
            {
                RequiredNumber = model.RequiredNumber,
                JobDescription = model.JobDescription,
                PostedDate = DateTime.Now,
                ExpiredDate = model.ExpiredDate,
                JobTitle = model.JobTitle
            };

            _context.TblJobLists.Add(tblJobList);
            var saved = await _context.SaveChangesAsync();

            if (saved > 0)
            {
                _notifyService.Success("Job successfully created.");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Job was not created. Please try again.");
            return View(model);
        }
        catch (Exception ex)
        {
            _notifyService.Error($"Error: {ex.Message}");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var tblJobList = await _context.TblJobLists.FindAsync(id);

        if (tblJobList is null)
        {
            return NotFound();
        }

        return View(new JobModel
        {
            JobId = id,
            ExpiredDate = tblJobList.ExpiredDate,
            PostedDate = tblJobList.PostedDate,
            JobDescription = tblJobList.JobDescription,
            JobTitle = tblJobList.JobTitle,
            RequiredNumber = tblJobList.RequiredNumber
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(JobModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var tblJobList = await _context.TblJobLists.FindAsync(model.JobId);

            if (tblJobList is null)
            {
                return NotFound();
            }

            tblJobList.JobDescription = model.JobDescription;
            tblJobList.JobTitle = model.JobTitle;
            tblJobList.ExpiredDate = model.ExpiredDate;
            tblJobList.RequiredNumber = model.RequiredNumber;
            tblJobList.PostedDate = model.PostedDate;

            var updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                _notifyService.Success("Job successfully updated.");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Job was not updated. Please try again.");
            return View(model);
        }
        catch (Exception ex)
        {
            _notifyService.Error($"Error: {ex.Message}");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var tblJobList = await _context.TblJobLists
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.JobId == id);

        if (tblJobList is null)
        {
            return NotFound();
        }

        return View(tblJobList);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var hasApplicants = await _context.TblApplicants
                .AsNoTracking()
                .AnyAsync(applicant => applicant.JobId == id);

            if (hasApplicants)
            {
                _notifyService.Warning("This vacancy already has applicants, so it cannot be deleted.");
                return RedirectToAction(nameof(Delete), new { id });
            }

            var tblJobList = await _context.TblJobLists.FindAsync(id);

            if (tblJobList is null)
            {
                _notifyService.Error("Job was not found.");
                return RedirectToAction(nameof(Index));
            }

            _context.TblJobLists.Remove(tblJobList);
            var deleted = await _context.SaveChangesAsync();

            if (deleted > 0)
            {
                _notifyService.Success("Job successfully deleted.");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Job was not deleted. Please try again.");
            return RedirectToAction(nameof(Delete), new { id });
        }
        catch (Exception ex)
        {
            _notifyService.Error(ex.Message);
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
