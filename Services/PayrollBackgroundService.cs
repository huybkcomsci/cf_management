using CafeManagement.Services.Interfaces;

namespace CafeManagement.Services;

public class PayrollBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PayrollBackgroundService> _logger;

    public PayrollBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PayrollBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                if (now.Day == DateTime.DaysInMonth(now.Year, now.Month) && now.Hour == 23)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var hrService = scope.ServiceProvider.GetRequiredService<IHrService>();
                    await hrService.SavePayrollAsync(now.Year, now.Month, stoppingToken);
                    _logger.LogInformation("Payroll auto-save done for {Year}-{Month}", now.Year, now.Month);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payroll background job failed");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
