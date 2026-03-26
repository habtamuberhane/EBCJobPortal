using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EBCJobPortalAdmin.Models;
using EBCJobPortalAdmin.IModels;
using EBCJobPortalAdmin.Filters;
using System.Collections.Generic;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EBCJobPortalAdmin.Controllers
{
    [CheckSessionIsAvailable]
    public class JobListsController : Controller
    {
        private readonly EbcJobPortalContext _context;
        private readonly INotyfService _notifyService;

        public JobListsController(EbcJobPortalContext context, INotyfService service)
        {
            _notifyService = service;
            _context = context;
        }

        // GET: JobLists
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblJobLists.Where(s=>s.ExpiredDate>DateTime.Now.Date).OrderByDescending(s=>s.JobId).ToListAsync());
        }
        public async Task<IActionResult> ExpiredJobs()
        {
            return View(await _context.TblJobLists.Where(s => s.ExpiredDate < DateTime.Now.Date).OrderByDescending(s => s.JobId).ToListAsync());
        }
        // GET: JobLists/Details/5
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

        // GET: JobLists/Create
        public IActionResult Create()
        {
            JobModel model= new JobModel();
            model.PostedDate = DateTime.Now;
            model.ExpiredDate=DateTime.Now.AddDays(10);
            return View(model);
        }

        // POST: JobLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobModel model)
        {
            try
            {
                TblJobList tblJobList= new TblJobList();
                tblJobList.RequiredNumber = model.RequiredNumber;
                tblJobList.JobDescription = model.JobDescription;
                tblJobList.PostedDate= DateTime.Now;
                tblJobList.ExpiredDate=model.ExpiredDate;
                tblJobList.JobTitle = model.JobTitle;
                _context.TblJobLists.Add(tblJobList);
                int saved=await _context.SaveChangesAsync();
                if (saved > 0)
                {
                    _notifyService.Success("Successfully added!");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _notifyService.Error("Not added. Please try later.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _notifyService.Error("Error " + ex.Message + " happened. ");
                return View(model);
            }
        }

        // GET: JobLists/Edit/5
        public async Task<IActionResult> Edit(int id)
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
            JobModel model = new JobModel();
            model.JobId = id;
            model.ExpiredDate = tblJobList.ExpiredDate;
            model.PostedDate=tblJobList.PostedDate;
            model.JobDescription = tblJobList.JobDescription;
            model.JobTitle = tblJobList.JobTitle;
          //  model.RequiredNumber = tblJobList.RequiredNumber;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobModel model)
        {
            try
            {
                TblJobList tblJobList=_context.TblJobLists.Find(model.JobId);
                if (tblJobList == null)
                {
                    return NotFound();
                }
                tblJobList.JobDescription = model.JobDescription;
                tblJobList.JobTitle = model.JobTitle;
                tblJobList.ExpiredDate = model.ExpiredDate;
                //tblJobList.RequiredNumber = model.RequiredNumber;
                tblJobList.PostedDate = model.PostedDate;
                int updated=await _context.SaveChangesAsync();
                if (updated > 0)
                {
                    _notifyService.Success("Successfully updated!");
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    _notifyService.Error("Not updated. Please try later.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _notifyService.Error("Error " + ex.Message + " happened. ");
                return View(model);
            }
        }

        // GET: JobLists/Delete/5
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var tblJobList = await _context.TblJobLists.FindAsync(id);
                if (tblJobList != null)
                {
                    _context.TblJobLists.Remove(tblJobList);
                }  
               int deleted= await _context.SaveChangesAsync();
                if (deleted > 0)
                {
                    _notifyService.Success("Sucessfully Deleted");
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
                else
                {
                    _notifyService.Error("Not deleted. Please try again.");
                    return RedirectToAction(nameof(Delete), new { id = id });
                }               
            }
            catch (Exception ex)
            {
                _notifyService.Error(ex.Message);
                return RedirectToAction(nameof(Delete), new {id=id});
            }
        }
        private bool TblJobListExists(int id)
        {
            return _context.TblJobLists.Any(e => e.JobId == id);
        }
    }
}
