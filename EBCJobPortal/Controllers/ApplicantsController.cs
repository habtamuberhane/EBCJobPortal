using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EBCJobPortal.Models;

namespace EBCJobPortal.Controllers
{
    public class ApplicantsController : Controller
    {
        private readonly EbcJobPortalContext _context;

        public ApplicantsController(EbcJobPortalContext context)
        {
            _context = context;
        }

        // GET: Applicants
        public async Task<IActionResult> Index()
        {
            var ebcJobPortalContext = _context.TblApplicants.Include(t => t.Job);
            return View(await ebcJobPortalContext.ToListAsync());
        }

        // GET: Applicants/Details/5
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

        // GET: Applicants/Create
        public IActionResult Create()
        {
            
            ViewData["JobId"] = new SelectList(_context.TblJobLists, "JobId", "JobId");
            return View();
        }

        // POST: Applicants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplyId,FullName,Gender,Nation,AreYouDisable,ReasonForDisablity,Regid,ZoneSubcity,Worede,HouseNumber,PhoneNumber,MaritalStatus,RegistrationDate,JobId,CurrentWorkingCompany,PositionTitle,MonthlySalary,RequirementDate,CompanyPhoneNumber,CompanyPostNumber,IfNojobcurrently,ResignationationReson,ResignationDate,NumberofExprianceYears,EducationLevel,EducationField,Institution,GraduationYear,Cgpa,CvShorttermTranings,BirthDate,Cvfile")] TblApplicant tblApplicant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblApplicant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobId"] = new SelectList(_context.TblJobLists, "JobId", "JobId", tblApplicant.JobId);
            return View(tblApplicant);
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
