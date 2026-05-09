using CafeManagement.Data;
using CafeManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Services;

public class DatabaseSeedBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseSeedBackgroundService> _logger;

    public DatabaseSeedBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DatabaseSeedBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var attempt = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            attempt++;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<ApplicationDbContext>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                await db.Database.MigrateAsync(stoppingToken);

                var roles = new[] { "Admin", "Kế toán", "Thu ngân" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                    }
                }

                const string adminEmail = "admin@cafemanagement.local";
                const string adminPassword = "Admin@123";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser is null)
                {
                    adminUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        DisplayName = "System Admin"
                    };

                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                await DbSeedData.EnsureSeedDataAsync(services);
                _logger.LogInformation("Database migrations and seed completed successfully.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database seed attempt {Attempt} failed; retrying.", attempt);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}