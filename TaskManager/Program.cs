using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess.Data;
using TaskManager.DataAccess.Models;
using TaskManager.DataAccess.Repositories;
using TaskManager.Business.Services;

namespace TaskManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configure cookie settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS
                options.Cookie.SameSite = SameSiteMode.Strict; // CSRF protection
            });

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(TaskManager.Business.Mappings.MappingProfile));

            // Register Repositories
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Register Services
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IRecommendationService, RecommendationService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add security headers
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Add security headers middleware
            app.Use(async (context, next) =>
            {
                // Prevent clickjacking
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                
                // Prevent MIME-sniffing
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                
                // Enable XSS protection
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                
                // Content Security Policy - strict but allows Bootstrap Icons and CDN
                context.Response.Headers.Add("Content-Security-Policy", 
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                    "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                    "font-src 'self' https://cdn.jsdelivr.net data:; " +
                    "img-src 'self' data: https:; " +
                    "connect-src 'self'");
                
                // Referrer Policy
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                
                // Permissions Policy (formerly Feature Policy)
                context.Response.Headers.Add("Permissions-Policy", 
                    "geolocation=(), microphone=(), camera=()");

                await next();
            });

            app.UseRouting();

            app.UseAuthentication(); // Must be before UseAuthorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Database migration and seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    logger.LogInformation("Starting database migration check...");
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    
                    logger.LogInformation("Database: {Database}", context.Database.GetConnectionString());
                    
                    // ALWAYS run migrations (temporarily for debugging)
                    logger.LogInformation("Applying database migrations...");
                    context.Database.Migrate();
                    logger.LogInformation("? Database migrations applied successfully!");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "? MIGRATION ERROR: {Message}", ex.Message);
                    logger.LogError("Inner Exception: {InnerException}", ex.InnerException?.Message ?? "None");
                    logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                    
                    // TEMPORARY: Re-throw to see error in browser (when ASPNETCORE_ENVIRONMENT=Development)
                    throw;
                }
            }

            app.Run();
        }
    }
}
