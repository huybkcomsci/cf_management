using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models.ViewModels;

public class AttendanceCheckInOutViewModel
{
    [Required]
    public Guid NhanvienId { get; set; }
    public DateTime? Ngay { get; set; }
}

public class AttendanceDashboardRowViewModel
{
    public Guid NhanvienId { get; set; }
    public string TenNhanvien { get; set; } = string.Empty;
    public DateTime Ngay { get; set; }
    public TimeSpan? GioVao { get; set; }
    public TimeSpan? GioRa { get; set; }
    public double TongGio { get; set; }
    public double OvertimeGio { get; set; }
}

public class PayrollResultViewModel
{
    public Guid NhanvienId { get; set; }
    public string TenNhanvien { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public int SoGio { get; set; }
    public decimal LuongCoBan { get; set; }
    public decimal LuongOvertime { get; set; }
    public decimal TongLuong { get; set; }
}
