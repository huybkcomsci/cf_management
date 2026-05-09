using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Phiếu chi (Expense/Payment Voucher)
/// </summary>
[Table("Phieuchi")]
public class Phieuchi
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string MaPC { get; set; } = null!;

    [ForeignKey(nameof(Nhanvien))]
    public Guid? IdNV { get; set; }

    [ForeignKey(nameof(Nhacungcap))]
    public Guid? IdNCC { get; set; }

    public DateTime NgayLapPC { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "numeric(12,2)")]
    public decimal SoTien { get; set; }

    public PhuongThucThanhToan PhuongThucThanhToan { get; set; } = PhuongThucThanhToan.TienMat;

    public LoaiChiPhi LoaiChiPhi { get; set; }

    public TrangThai TrangThai { get; set; } = TrangThai.Nhap;

    [StringLength(200)]
    public string? GhiChu { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual Nhanvien? Nhanvien { get; set; }
    public virtual Nhacungcap? Nhacungcap { get; set; }
}
