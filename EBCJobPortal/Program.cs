using NToastNotify;
using EBCJobPortal.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification;
using Microsoft.Extensions.FileProviders;

namespace EBCJobPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<EbcJobPortalContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("EBCJOBDB"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    });
            });
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddMvc();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });
            builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
            {
                ProgressBar = true,
                Timeout = 5000,

            });
            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 10;
                config.HasRippleEffect = true;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            var adminPath = Path.Combine(builder.Environment.ContentRootPath, "admin");
            if (Directory.Exists(adminPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(adminPath),
                    RequestPath = "/admin"
                });
            }
            app.UseSession();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
