using CafeManagement.Models.ViewModels;

namespace CafeManagement.Services.Interfaces;

public interface IHrService
{
    Task CheckInAsync(Guid nhanvienId, DateTime? ngay = null, CancellationToken cancellationToken = default);
    Task CheckOutAsync(Guid nhanvienId, DateTime? ngay = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceDashboardRowViewModel>> GetAttendanceAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PayrollResultViewModel>> CalculatePayrollAsync(int year, int month, CancellationToken cancellationToken = default);
    Task SavePayrollAsync(int year, int month, CancellationToken cancellationToken = default);
}
