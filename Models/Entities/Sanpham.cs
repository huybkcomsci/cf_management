using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Sản phẩm (Product) - Main product entity
/// </summary>
[Table("Sanpham")]
public class Sanpham
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(NhomSP))]
    public Guid IdNhom { get; set; }

    [Required]
    [StringLength(100)]
    public string TenSP { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal GiaBan { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal GiaNhap { get; set; }

    public int SoLuongTon { get; set; } = 0;

    public int SLTonMin { get; set; } = 0;

    [StringLength(50)]
    public string? DonViTinh { get; set; }

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual NhomSP NhomSP { get; set; } = null!;

    // Collection navigation properties
    public ICollection<HoadonCT> HoadonCTs { get; set; } = new List<HoadonCT>();
    public ICollection<Dongnhap> Dongnhaps { get; set; } = new List<Dongnhap>();
    public ICollection<Tieuhao> Tieuhaos { get; set; } = new List<Tieuhao>();
    public ICollection<Dinhluong> Dinhluongs { get; set; } = new List<Dinhluong>();
    public ICollection<Dinhluong> DinhluongThanhPhans { get; set; } = new List<Dinhluong>();
}
