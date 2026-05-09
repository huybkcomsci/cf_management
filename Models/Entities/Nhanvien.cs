using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Nhân viên (Employee)
/// </summary>
[Table("Nhanvien")]
public class Nhanvien
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TenNV { get; set; } = null!;

    [StringLength(20)]
    public string? Sdt { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? DiaChi { get; set; }

    [StringLength(20)]
    public string? CMND { get; set; }

    public DateTime? NgaySinh { get; set; }

    [StringLength(50)]
    public string? ChucVu { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal? Luong { get; set; }

    public TrangThaiNhanvien TrangThai { get; set; } = TrangThaiNhanvien.DangLam;

    public DateTime NgayVaoLam { get; set; } = DateTime.UtcNow;

    public DateTime? NgayThaiPhuc { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
    public ICollection<Chamcong> Chamcongs { get; set; } = new List<Chamcong>();
    public ICollection<Bangluong> Bangluongs { get; set; } = new List<Bangluong>();
    public ICollection<Phieunhap> Phieunhaps { get; set; } = new List<Phieunhap>();
    public ICollection<Dinhluong> Dinhluongs { get; set; } = new List<Dinhluong>();
}
