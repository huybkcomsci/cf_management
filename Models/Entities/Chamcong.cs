using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Chấm công (Attendance Record)
/// </summary>
[Table("Chamcong")]
public class Chamcong
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Nhanvien))]
    public Guid IdNV { get; set; }

    public DateTime Ngay { get; set; }

    public TimeSpan? GioVao { get; set; }

    public TimeSpan? GioRa { get; set; }

    public TrangThaiChamCong TrangThai { get; set; } = TrangThaiChamCong.CoMat;

    [StringLength(200)]
    public string? GhiChu { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual Nhanvien Nhanvien { get; set; } = null!;
}
