using AspNetCoreHero.ToastNotification;
using EBCJobPortalAdmin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NToastNotify;

namespace EBCJobPortalAdmin;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<EbcJobPortalContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("EBCJOBDB"),
                sqlOptions =>
                {
                    // A short command timeout keeps local pages responsive while the database connection is still being finalized.
                    sqlOptions.CommandTimeout(builder.Environment.IsDevelopment() ? 5 : 15);

                    // Long retry chains make local pages feel like they are hanging when SQL Server is offline.
                    // Development gets a fast-fail configuration, while non-development keeps a light retry policy.
                    if (builder.Environment.IsDevelopment())
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 0,
                            maxRetryDelay: TimeSpan.Zero,
                            errorNumbersToAdd: null);
                    }
                    else
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(2),
                            errorNumbersToAdd: null);
                    }
                });
        });

        // Cookie authentication is the core security mechanism for the admin side.
        // It stores an encrypted authentication ticket in a secure HTTP-only cookie after login.
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                // Unauthenticated users are always redirected to the login screen.
                options.LoginPath = "/Account/Login";

                // Unauthorized users land on the same login page for this simple admin workflow.
                options.AccessDeniedPath = "/Account/Login";

                // The cookie name is customized to avoid collisions with the client portal.
                options.Cookie.Name = "EBCJobPortalAdmin.Auth";

                // HTTP-only cookies reduce exposure to client-side script access.
                options.Cookie.HttpOnly = true;

                // Lax is a safe default for an admin app that does not require cross-site posting.
                options.Cookie.SameSite = SameSiteMode.Lax;

                // SecurePolicy.SameAsRequest preserves local HTTP development while still using HTTPS in production.
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                // Sliding expiration keeps active administrators signed in without forcing frequent re-login.
                options.SlidingExpiration = true;

                // A short but usable window is a good fit for an internal admin dashboard.
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

        builder.Services.AddAuthorization();

        // A global authorization filter protects every MVC endpoint by default.
        // AccountController uses [AllowAnonymous] on the login endpoints to opt out explicitly.
        builder.Services.AddControllersWithViews(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        });

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
        {
            ProgressBar = true,
            Timeout = 5000
        });

        builder.Services.AddNotyf(config =>
        {
            config.DurationInSeconds = 10;
            config.HasRippleEffect = true;
            config.IsDismissable = true;
            config.Position = NotyfPosition.TopRight;
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // The admin file library must exist before PhysicalFileProvider is created, otherwise startup throws.
        var filesRootPath = Path.Combine(builder.Environment.ContentRootPath, "Files");

        // Creating the directory at startup keeps local development and hot reload working on fresh clones.
        Directory.CreateDirectory(filesRootPath);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(filesRootPath),
            RequestPath = "/Files"
        });

        app.UseRouting();

        // Authentication must run before Authorization so user claims are available to the policy system.
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
