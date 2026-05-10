using CafeManagement.Data;
using CafeManagement.Repositories;
using CafeManagement.Repositories.Implementations;
using CafeManagement.Services;
using CafeManagement.Services.Interfaces;
using CafeManagement.Services.Exports;
using CafeManagement.Mappings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(CafeMappingProfile));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

// Add session support for simple authentication
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure PostgreSQL database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(120);
    });
});

// Register repositories and services
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISanphamRepository, SanphamRepository>();
builder.Services.AddScoped<ISanphamService, SanphamService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IHrService, HrService>();
builder.Services.AddScoped<IDinhluongService, DinhluongService>();
builder.Services.AddScoped<PdfTemplateService>();
builder.Services.AddScoped<ExcelTemplateService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddHostedService<PayrollBackgroundService>();

var app = builder.Build();

if (args.Contains("--seed-data"))
{
    try
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();

        // Seed default users (plain text passwords for simplicity)
        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(new[]
            {
                new CafeManagement.Models.User 
                { 
                    Email = "admin@cafemanagement.local", 
                    Password = "Admin@123", 
                    Role = "Admin",
                    IsActive = true
                },
                new CafeManagement.Models.User 
                { 
                    Email = "keytoan@cafemanagement.local", 
                    Password = "Admin@123", 
                    Role = "Kế toán",
                    IsActive = true
                },
                new CafeManagement.Models.User 
                { 
                    Email = "thunga@cafemanagement.local", 
                    Password = "Admin@123", 
                    Role = "Thu ngân",
                    IsActive = true
                }
            });
            await db.SaveChangesAsync();
        }

        await DbSeedData.EnsureSeedDataAsync(services);
        app.Logger.LogInformation("Database migrations and seed completed successfully.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database seed command failed.");
        Environment.ExitCode = 1;
    }

    return;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    version = "1.0.0"
}));

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
