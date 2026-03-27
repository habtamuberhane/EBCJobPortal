using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EBCJobPortalAdmin.Models;
using System.Collections.Generic;
using EBCJobPortalAdmin.Security;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace EBCJobPortalAdmin.Controllers
{
    [Authorize]
    public class AdminlUsersController : Controller
    {
        private readonly EbcJobPortalContext _context;
        private readonly INotyfService _notifyService;

        public AdminlUsersController(EbcJobPortalContext context, INotyfService service)
        {
            _notifyService = service;
            _context = context;
        }

        // GET: AdminlUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblJobPortalUsers.ToListAsync());
        }

        // GET: AdminlUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblJobPortalUser = await _context.TblJobPortalUsers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (tblJobPortalUser == null)
            {
                return NotFound();
            }

            return View(tblJobPortalUser);
        }

        // GET: AdminlUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required.");
                return View(model);
            }

            try
            {
                TblJobPortalUser user = new TblJobPortalUser();
                var userExist = await _context.TblJobPortalUsers.AnyAsync(m => m.UserName == model.UserName);
                if (userExist)
                {
                    _notifyService.Warning("Username already exists. Please try another one");
                    return View(model);
                }

                user.PhoneNumber = model.PhoneNumber;
                user.UserName = model.UserName;
                user.FullName = model.FullName;
                user.PassWord = PawwordEncryption.EncryptPasswordBase64Strig(model.Password);
                user.IsSuperAdmin = true;
                user.EmailAdress=model.EmailAddress;
                _context.TblJobPortalUsers.Add(user);
                int saved=await _context.SaveChangesAsync();
                if (saved>0)
                {
                    _notifyService.Success("Successfully created");
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    _notifyService.Error("Not successfull. Please try again");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                _notifyService.Error(ex.Message + " happened. Please try again");
                return View(model);
            }
        }

        // GET: AdminlUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblJobPortalUser = await _context.TblJobPortalUsers.FindAsync(id);
            if (tblJobPortalUser == null)
            {
                return NotFound();
            }
            UserModel userModel = new UserModel();
            userModel.UserId=tblJobPortalUser.UserId;
            userModel.UserName=tblJobPortalUser.UserName;
            userModel.FullName=tblJobPortalUser.FullName;
            userModel.EmailAddress=tblJobPortalUser.EmailAdress;
            userModel.PhoneNumber=tblJobPortalUser.PhoneNumber;

            return View(userModel);
        }

        // POST: AdminlUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                TblJobPortalUser? jobPortalUser = await _context.TblJobPortalUsers.FindAsync(model.UserId);
                if (jobPortalUser==null)
                {
                    return NotFound();
                }
                jobPortalUser.UserName=model.UserName;
                jobPortalUser.PhoneNumber=model.PhoneNumber;
                jobPortalUser.FullName=model.FullName;
                jobPortalUser.EmailAdress=model.EmailAddress;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    jobPortalUser.PassWord=PawwordEncryption.EncryptPasswordBase64Strig(model.Password);
                }
                jobPortalUser.IsSuperAdmin=true;
                int updated=await _context.SaveChangesAsync();
                if (updated>0)
                {
                    _notifyService.Success("Successfully Updated");
                    return RedirectToAction("Index");

                }
                else
                {
                    _notifyService.Error("Not updated. Please try again");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                _notifyService.Error(ex.Message + " happened. Please try again");
                return View(model);
            }



        }

        // GET: AdminlUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblJobPortalUser = await _context.TblJobPortalUsers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (tblJobPortalUser == null)
            {
                return NotFound();
            }

            return View(tblJobPortalUser);
        }

        // POST: AdminlUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblJobPortalUser = await _context.TblJobPortalUsers.FindAsync(id);
            if (tblJobPortalUser != null)
            {
                _context.TblJobPortalUsers.Remove(tblJobPortalUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblJobPortalUserExists(int id)
        {
            return _context.TblJobPortalUsers.Any(e => e.UserId == id);
        }
    }
}
