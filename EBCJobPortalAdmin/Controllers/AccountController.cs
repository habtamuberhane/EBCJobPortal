using EBCJobPortalAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using EBCJobPortalAdmin.Filters;
using EBCJobPortalAdmin.Security;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EBCJobPortalAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly EbcJobPortalContext _context;
        private readonly INotyfService _notifyService;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountController(EbcJobPortalContext context, INotyfService notifyService, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _notifyService = notifyService;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserModel userModel)
        {
            string password=PawwordEncryption.EncryptPasswordBase64Strig(userModel.Password);
           var user=_context.TblJobPortalUsers.Where(s=>s.UserName==userModel.UserName&&s.PassWord==password).FirstOrDefault();
            if (user!=null)
            {
                _notifyService.Success("Started your session");
                _contextAccessor.HttpContext.Session.SetString("userId", user.UserId.ToString());
                _contextAccessor.HttpContext.Session.SetString("userFullname", user.FullName.ToString());
                _contextAccessor.HttpContext.Session.SetString("username",user.EmailAdress.ToString());
                return RedirectToAction("Index","Home");
            }
            else
            {
                _notifyService.Error("User doesn't exists. Please try again latter");
                return View(userModel);
            }
        }
        public IActionResult Logout()
        {
            _contextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult FeedBack()
        {
            return View();
        }
        public IActionResult AccountStatus()
        {
            return View();
        }
        [CheckSessionIsAvailable]
        public async Task<IActionResult> Profile()
        {
            UserModel loginModels = new UserModel();
            int id = int.Parse(HttpContext.Session.GetString("userId"));
            if (id == null || _context.TblJobPortalUsers == null)
            {
                return NotFound();
            }
            var tblUser = await _context.TblJobPortalUsers.Where(m => m.UserId == id).FirstOrDefaultAsync();
            if (tblUser == null)
            {
                return NotFound();
            }
            loginModels.UserId = id;
            loginModels.UserName = tblUser.UserName;
            loginModels.Password = tblUser.PassWord;
            loginModels.FullName = tblUser.FullName;
            loginModels.PhoneNumber = tblUser.PhoneNumber;
            loginModels.EmailAddress = tblUser.EmailAdress;

            return View(loginModels);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Profile(UserModel loginModels)
        {
            if (TblUserExists(loginModels.UserId))
            {
                TblJobPortalUser tblUser = _context.TblJobPortalUsers.Find(loginModels.UserId);
                tblUser.PassWord = PawwordEncryption.EncryptPasswordBase64Strig(loginModels.NewPassword);
                tblUser.PhoneNumber = loginModels.PhoneNumber;
                tblUser.FullName = loginModels.FullName;
                tblUser.EmailAdress = loginModels.EmailAddress;
                tblUser.UserName = loginModels.UserName;
                int saved = _context.SaveChanges();
                if (saved > 0)
                {
                    ViewBag.Ok = "Password Successfully changed.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Operation isn't successfully changed. Please try again";
                    return View(loginModels);
                }
            }
            ViewBag.Error = "User doesn't found!. Please try again";
            return View();
        }
        private bool TblUserExists(int id)
        {
            return (_context.TblJobPortalUsers?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
