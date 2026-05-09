using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Phiếu nhập (Purchase Receipt)
/// </summary>
[Table("Phieunhap")]
public class Phieunhap
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string MaPN { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Nhacungcap))]
    public Guid IdNCC { get; set; }

    [Required]
    [ForeignKey(nameof(Nhanvien))]
    public Guid IdNV { get; set; }

    public DateTime NgayLapPN { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "numeric(12,2)")]
    public decimal TongTien { get; set; }

    public TrangThai TrangThai { get; set; } = TrangThai.Nhap;

    [StringLength(200)]
    public string? GhiChu { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual Nhacungcap Nhacungcap { get; set; } = null!;
    public virtual Nhanvien Nhanvien { get; set; } = null!;

    // Collection navigation properties
    public ICollection<Dongnhap> Dongnhaps { get; set; } = new List<Dongnhap>();
}
