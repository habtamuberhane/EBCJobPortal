using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using EBCJobPortalAdmin.Models;
using EBCJobPortalAdmin.Security;
using EBCJobPortalAdmin.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBCJobPortalAdmin.Controllers;

public class AccountController : Controller
{
    private readonly EbcJobPortalContext _context;
    private readonly INotyfService _notifyService;
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;

    public AccountController(
        EbcJobPortalContext context,
        INotyfService notifyService,
        ILogger<AccountController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _notifyService = notifyService;
        _logger = logger;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Already-authenticated users should not sit on the login page again.
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new AdminLoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel userModel, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        // The dedicated login view model ensures only login-specific fields are validated here.
        if (!ModelState.IsValid)
        {
            return View(userModel);
        }

        var bootstrapUserName = _configuration["AdminBootstrap:UserName"];
        var bootstrapPassword = _configuration["AdminBootstrap:Password"];

        // A bootstrap admin account can still exist, but its credentials must come from configuration or user secrets instead of source control.
        var isHardcodedAdmin =
            !string.IsNullOrWhiteSpace(bootstrapUserName)
            && !string.IsNullOrWhiteSpace(bootstrapPassword)
            && string.Equals(userModel.UserName, bootstrapUserName, StringComparison.OrdinalIgnoreCase)
            && userModel.Password == bootstrapPassword;

        if (isHardcodedAdmin)
        {
            var bootstrapClaims = new List<Claim>
            {
                new(ClaimTypes.Name, bootstrapUserName!),
                new(ClaimTypes.NameIdentifier, bootstrapUserName!),
                new(ClaimTypes.Role, "SuperAdmin"),
                new("UserName", bootstrapUserName!),
                new("FullName", "System Administrator"),
                new("Email", "admin@ebc.local")
            };

            var bootstrapIdentity = new ClaimsIdentity(bootstrapClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var bootstrapPrincipal = new ClaimsPrincipal(bootstrapIdentity);

            var bootstrapAuthProperties = new AuthenticationProperties
            {
                IsPersistent = userModel.IsRemember,
                RedirectUri = null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                bootstrapPrincipal,
                bootstrapAuthProperties);

            HttpContext.Session.SetString("userId", bootstrapUserName!);
            HttpContext.Session.SetString("userFullname", "System Administrator");
            HttpContext.Session.SetString("username", bootstrapUserName!);

            _notifyService.Success("Welcome back to the admin dashboard.");

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // The current database model exposes tbl_JobPortalUsers, so authentication is validated against that table.
        var encryptedPassword = PawwordEncryption.EncryptPasswordBase64Strig(userModel.Password);

        TblJobPortalUser? dbUser;

        try
        {
            dbUser = await _context.TblJobPortalUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(user =>
                    user.UserName == userModel.UserName &&
                    user.PassWord == encryptedPassword);
        }
        catch (SqlException ex)
        {
            // Database connectivity issues are surfaced cleanly so the login page stays usable.
            _logger.LogError(ex, "Database connection failed during admin login for user {UserName}.", userModel.UserName);
            ModelState.AddModelError(string.Empty, "Database connection is currently unavailable. Please verify SQL Server and try again.");
            _notifyService.Error("Database connection is currently unavailable.");
            return View(userModel);
        }
        catch (InvalidOperationException ex)
        {
            // EF can wrap provider startup issues in InvalidOperationException, so we handle that path as well.
            _logger.LogError(ex, "Login failed because the admin user store is unavailable for user {UserName}.", userModel.UserName);
            ModelState.AddModelError(string.Empty, "Admin user store is currently unavailable. Please try again shortly.");
            _notifyService.Error("Admin user store is currently unavailable.");
            return View(userModel);
        }

        if (dbUser is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            _notifyService.Error("Invalid username or password.");
            return View(userModel);
        }

        var displayName = string.IsNullOrWhiteSpace(dbUser.FullName) ? dbUser.UserName ?? "Admin User" : dbUser.FullName;
        var emailAddress = dbUser.EmailAdress ?? string.Empty;
        var roleName = dbUser.IsSuperAdmin == true ? "SuperAdmin" : "Administrator";

        var claims = new List<Claim>
        {
            // Name is used widely by ASP.NET Core for identity display and logging.
            new(ClaimTypes.Name, dbUser.UserName ?? userModel.UserName),

            // NameIdentifier gives us a stable claim for future user-specific admin features.
            new(ClaimTypes.NameIdentifier, dbUser.UserId.ToString()),

            // Role supports [Authorize(Roles = ...)] if the dashboard grows into role-based access later.
            new(ClaimTypes.Role, roleName),

            // A dedicated username claim keeps the top-right identity UI aligned with the actual login handle.
            new("UserName", dbUser.UserName ?? userModel.UserName),

            // These custom claims feed the top-right admin identity UI cleanly.
            new("FullName", displayName),
            new("Email", emailAddress)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            // Persistent cookies only apply when the remember-me flag is checked.
            IsPersistent = userModel.IsRemember,

            // RedirectUri is left null because MVC handles the return URL after sign-in.
            RedirectUri = null
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        // These session keys preserve compatibility with older admin controllers during the auth migration.
        HttpContext.Session.SetString("userId", dbUser.UserId.ToString());
        HttpContext.Session.SetString("userFullname", displayName);
        HttpContext.Session.SetString("username", string.IsNullOrWhiteSpace(emailAddress) ? (dbUser.UserName ?? userModel.UserName) : emailAddress);

        _notifyService.Success("Welcome back to the admin dashboard.");

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // SignOutAsync invalidates the admin authentication cookie and clears the current session identity.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();

        _notifyService.Success("You have been signed out.");
        return RedirectToAction(nameof(Login));
    }
}
