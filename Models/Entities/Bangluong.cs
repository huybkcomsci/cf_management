using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Bảng lương (Payroll/Salary)
/// </summary>
[Table("Bangluong")]
public class Bangluong
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Nhanvien))]
    public Guid IdNV { get; set; }

    public int Nam { get; set; }

    public int Thang { get; set; }

    public int SoGio { get; set; }

    [Column(TypeName = "numeric(12,2)")]
    public decimal Luong { get; set; }

    [Column(TypeName = "numeric(12,2)")]
    public decimal? PhuCap { get; set; } = 0;

    [Column(TypeName = "numeric(12,2)")]
    public decimal? KhauTru { get; set; } = 0;

    [Column(TypeName = "numeric(12,2)")]
    public decimal TongLuong { get; set; }

    public TrangThai TrangThai { get; set; } = TrangThai.Nhap;

    [StringLength(200)]
    public string? GhiChu { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual Nhanvien Nhanvien { get; set; } = null!;
}
