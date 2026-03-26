using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EBCJobPortalAdmin.Models;
using EBCJobPortalAdmin.Filters;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using EBCJobPortalAdmin.ViewModel;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Authorization;

namespace EBCJobPortalAdmin.Controllers
{
    [CheckSessionIsAvailable]
    public class ApplicantsController : Controller
    {
        private readonly EbcJobPortalContext _context;
        private readonly INotyfService _notifyService;
        public ApplicantsController(EbcJobPortalContext context, INotyfService notyfService)
        {
            _context = context;
            _notifyService = notyfService;
        }

        // GET: Applicants
        public async Task<IActionResult> Index()
        {
            ApplicantsListModel model = new ApplicantsListModel();
            model.Applicants = _context.TblApplicants.Include(s=>s.Reg).Include(t => t.Job);
           model.Jobs = _context.TblJobLists.Select(s => new SelectListItem
            {
                Text = s.JobTitle,
                Value = s.JobId.ToString()
            }).ToList();
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ApplicantsListModel model)
        {
            ApplicantsListModel model1 = new ApplicantsListModel();

            // Build the query - start with Include
            IQueryable<TblApplicant> applicantsQuery = _context.TblApplicants
                .Include(s => s.Reg)
                .Include(t => t.Job);

            // Apply filter if JobId is selected
            if (model.JobId.HasValue)
            {
                applicantsQuery = applicantsQuery.Where(s => s.JobId == model.JobId.Value);
            }

            model1.Applicants = applicantsQuery;  // No cast needed now
            model1.JobId = model.JobId;

            // Get jobs for dropdown
            model1.Jobs = await _context.TblJobLists
                .Select(s => new SelectListItem
                {
                    Text = s.JobTitle,
                    Value = s.JobId.ToString()
                })
                .ToListAsync();

            return View(model1);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblApplicant = await _context.TblApplicants
                .Include(t => t.Job)
                .FirstOrDefaultAsync(m => m.ApplyId == id);
            if (tblApplicant == null)
            {
                return NotFound();
            }

            return View(tblApplicant);
        }

        public IActionResult Create(int id)
        {
            ApplicantModel model = new ApplicantModel();
            model.Jobs=_context.TblJobLists.Where(s => s.ExpiredDate > DateTime.Now.Date).Select(s=> new SelectListItem
            {
                Text=s.JobTitle,
                Value=s.JobId.ToString()
            }).ToList();
            model.BirthDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-18));
            model.Regions = _context.TblRegions.Select(s => new SelectListItem
            {
                Value = s.Regid.ToString(),
                Text = s.RegionName,
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
                    applicants.Gender = applicantModel.Gender;
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
                        applicantModel.Jobs = _context.TblJobLists.Where(s => s.ExpiredDate > DateTime.Now.Date).Select(s => new SelectListItem
                        {
                            Text = s.JobTitle,
                            Value = s.JobId.ToString()
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
                    applicantModel.Jobs = _context.TblJobLists.Where(s => s.ExpiredDate > DateTime.Now.Date).Select(s => new SelectListItem
                    {
                        Text = s.JobTitle,
                        Value = s.JobId.ToString()
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
                applicantModel.Jobs = _context.TblJobLists.Where(s => s.ExpiredDate > DateTime.Now.Date).Select(s => new SelectListItem
                {
                    Text = s.JobTitle,
                    Value = s.JobId.ToString()
                }).ToList();
                _notifyService.Error("Please fill required fields");
                return View(applicantModel);
            }
        }
        // GET: Applicants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblApplicant = await _context.TblApplicants.FindAsync(id);
            if (tblApplicant == null)
            {
                return NotFound();
            }
            ViewData["JobId"] = new SelectList(_context.TblJobLists, "JobId", "JobId", tblApplicant.JobId);
            return View(tblApplicant);
        }

        // POST: Applicants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApplyId,FullName,Gender,Nation,AreYouDisable,ReasonForDisablity,Regid,ZoneSubcity,Worede,HouseNumber,PhoneNumber,MaritalStatus,RegistrationDate,JobId,CurrentWorkingCompany,PositionTitle,MonthlySalary,RequirementDate,CompanyPhoneNumber,CompanyPostNumber,IfNojobcurrently,ResignationationReson,ResignationDate,NumberofExprianceYears,EducationLevel,EducationField,Institution,GraduationYear,Cgpa,CvShorttermTranings,BirthDate,Cvfile")] TblApplicant tblApplicant)
        {
            if (id != tblApplicant.ApplyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblApplicant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblApplicantExists(tblApplicant.ApplyId))
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
            ViewData["JobId"] = new SelectList(_context.TblJobLists, "JobId", "JobId", tblApplicant.JobId);
            return View(tblApplicant);
        }

        // GET: Applicants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblApplicant = await _context.TblApplicants
                .Include(t => t.Job)
                .FirstOrDefaultAsync(m => m.ApplyId == id);
            if (tblApplicant == null)
            {
                return NotFound();
            }

            return View(tblApplicant);
        }

        // POST: Applicants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblApplicant = await _context.TblApplicants.FindAsync(id);
            if (tblApplicant != null)
            {
                _context.TblApplicants.Remove(tblApplicant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblApplicantExists(int id)
        {
            return _context.TblApplicants.Any(e => e.ApplyId == id);
        }
    }
}
