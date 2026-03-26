using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EBCJobPortalAdmin.Models;
using EBCJobPortalAdmin.Filters;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EBCJobPortalAdmin.Controllers
{
    [CheckSessionIsAvailable]
    public class RegionsController : Controller
    {
        private readonly EbcJobPortalContext _context;

        public RegionsController(EbcJobPortalContext context)
        {
            _context = context;
        }

        // GET: Regions
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblRegions.ToListAsync());
        }

        // GET: Regions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRegion = await _context.TblRegions
                .FirstOrDefaultAsync(m => m.Regid == id);
            if (tblRegion == null)
            {
                return NotFound();
            }

            return View(tblRegion);
        }

        // GET: Regions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Regions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Regid,RegionName")] TblRegion tblRegion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblRegion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblRegion);
        }

        // GET: Regions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRegion = await _context.TblRegions.FindAsync(id);
            if (tblRegion == null)
            {
                return NotFound();
            }
            return View(tblRegion);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Regid,RegionName")] TblRegion tblRegion)
        {
            if (id != tblRegion.Regid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblRegion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblRegionExists(tblRegion.Regid))
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
            return View(tblRegion);
        }

        // GET: Regions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRegion = await _context.TblRegions
                .FirstOrDefaultAsync(m => m.Regid == id);
            if (tblRegion == null)
            {
                return NotFound();
            }

            return View(tblRegion);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblRegion = await _context.TblRegions.FindAsync(id);
            if (tblRegion != null)
            {
                _context.TblRegions.Remove(tblRegion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblRegionExists(int id)
        {
            return _context.TblRegions.Any(e => e.Regid == id);
        }
    }
}
