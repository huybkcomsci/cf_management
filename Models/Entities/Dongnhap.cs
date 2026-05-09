using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Dòng nhập (Purchase Line Item)
/// </summary>
[Table("Dongnhap")]
public class Dongnhap
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Sanpham))]
    public Guid IdSP { get; set; }

    [Required]
    [ForeignKey(nameof(Phieunhap))]
    public Guid IdPhieuNhap { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal DonGia { get; set; }

    [StringLength(200)]
    public string? GhiChu { get; set; }

    // Foreign key navigation
    public virtual Sanpham Sanpham { get; set; } = null!;
    public virtual Phieunhap Phieunhap { get; set; } = null!;
}
