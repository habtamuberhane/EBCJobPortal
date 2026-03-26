using System;
using System.Linq;
using EBCJobPortalAdmin.Models;
using System.Threading.Tasks;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EBCJobPortalAdmin.Controllers
{
    public class AllJobsController : Controller
    {
        private readonly EbcJobPortalContext _context;
        private readonly INotyfService _notifyService;
        public AllJobsController(EbcJobPortalContext context, INotyfService notyfService)
        {
            _context = context;
            _notifyService = notyfService;
        }

        // GET: AllJobs
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblJobLists.Where(s => s.ExpiredDate > DateTime.Now.Date).ToListAsync());
        }
        public async Task<IActionResult> ExpiredJobs()
        {
            return View(await _context.TblJobLists.Where(s => s.ExpiredDate < DateTime.Now).ToListAsync());
        }
        // GET: AllJobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblJobList = await _context.TblJobLists
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (tblJobList == null)
            {
                return NotFound();
            }

            return View(tblJobList);
        }

        // GET: AllJobs/Create
        public IActionResult Create(int id)
        {
            ApplicantModel model = new ApplicantModel();          
            model.JobId = id;
            model.BirthDate=DateOnly.FromDateTime(DateTime.Now.AddDays(-18));
            model.Regions=_context.TblRegions.Select(s=> new SelectListItem
            {
                Value=s.Regid.ToString(),
                Text=s.RegionName,
            }).ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicantModel applicantModel)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    TblApplicant applicants = new TblApplicant();
                    applicants.JobId = applicantModel.JobId;
                    applicants.Cgpa = applicantModel.Cgpa;
                    applicants.FullName = applicantModel.FullName;
                    applicants.Gender = applicantModel.Gender;
                    applicants.Nation = applicantModel.Nation;
                    applicants.RegistrationDate = DateTime.Now;
                    if (applicantModel.Disable.ToString() == "Yes")
                    {
                        applicants.AreYouDisable = true;
                        applicants.ReasonForDisablity = applicantModel.Nation;
                    }
                    else
                    {
                        applicants.AreYouDisable = false;
                    }
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
                    applicants.Gender= applicantModel.Gender;
                    applicants.Worede = applicantModel.Worede;    
                    applicants.MaritalStatus = applicantModel.MaritalStatus;
                    applicants.PositionTitle = applicantModel.PositionTitle;
                    applicants.EducationLevel = applicantModel.EducationLevel;
                    applicants.CvShorttermTranings = applicantModel.CvShorttermTranings;
                    applicants.EducationField = applicantModel.EducationField;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "admin/Files");

                    //create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    //get file extension
                    FileInfo fileInfo = new FileInfo(applicantModel.Cvfile.FileName);
                    string fileName = Guid.NewGuid().ToString() + applicantModel.Cvfile.FileName;
                    string fileNameWithPath = Path.Combine(path, fileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        applicantModel.Cvfile.CopyTo(stream);
                    }
                    string dbPath = "/admin/Files/" + fileName;
                    applicants.Cvfile = dbPath;
                    _context.TblApplicants.Add(applicants);
                    int saved = await _context.SaveChangesAsync();
                    if (saved > 0)
                    {
                        _notifyService.Success("Your application is successfully submited");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        applicantModel.Regions = _context.TblRegions.Select(s => new SelectListItem
                        {
                            Value = s.Regid.ToString(),
                            Text = s.RegionName,
                        }).ToList();                       
                        _notifyService.Error("Your application isn't successfully submitted. Please try again.");
                        return View(applicantModel);
                    }
                }
                catch (Exception ex)
                {
                    applicantModel.Regions = _context.TblRegions.Select(s => new SelectListItem
                    {
                        Value = s.Regid.ToString(),
                        Text = s.RegionName,
                    }).ToList();                   
                    _notifyService.Error(ex.Message + " happened. Your application isn't successfully submitted. Please try again.");
                    return View(applicantModel);
                }
            }
            else
            {
                applicantModel.Regions = _context.TblRegions.Select(s => new SelectListItem
                {
                    Value = s.Regid.ToString(),
                    Text = s.RegionName,
                }).ToList();               
                _notifyService.Error("Please fill required fields");
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
        public async Task<IActionResult> Edit(int id, [Bind("JobId,RequiredNumber,PostedDate,ExpiredDate,JobTitle,JobDescription")] TblJobList tblJobList)
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

            var tblJobList = await _context.TblJobLists
                .FirstOrDefaultAsync(m => m.JobId == id);
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
