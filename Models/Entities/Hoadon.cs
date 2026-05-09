using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Hóa đơn (Invoice/Bill)
/// </summary>
[Table("Hoadon")]
public class Hoadon
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string MaHD { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Khachhang))]
    public Guid IdKH { get; set; }

    [Required]
    [ForeignKey(nameof(Nhanvien))]
    public Guid IdNV { get; set; }

    public DateTime NgayLapHD { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "numeric(12,2)")]
    public decimal ThanhTien { get; set; }

    [Column(TypeName = "numeric(12,2)")]
    public decimal? GiamGia { get; set; }

    [Column(TypeName = "numeric(12,2)")]
    public decimal TongCong { get; set; }

    public PhuongThucThanhToan PhuongThucThanhToan { get; set; } = PhuongThucThanhToan.TienMat;

    public TrangThai TrangThai { get; set; } = TrangThai.Nhap;

    [StringLength(200)]
    public string? GhiChu { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual Khachhang Khachhang { get; set; } = null!;
    public virtual Nhanvien Nhanvien { get; set; } = null!;

    // Collection navigation properties
    public ICollection<HoadonCT> HoadonCTs { get; set; } = new List<HoadonCT>();
}
