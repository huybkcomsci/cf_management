using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Chi tiết hóa đơn (Invoice Detail)
/// </summary>
[Table("HoadonCT")]
public class HoadonCT
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Hoadon))]
    public Guid IdHD { get; set; }

    [Required]
    [ForeignKey(nameof(Sanpham))]
    public Guid IdSP { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "numeric(12,2)")]
    public decimal ThanhTien { get; set; }

    [StringLength(200)]
    public string? GhiChu { get; set; }

    // Foreign key navigation
    public virtual Hoadon Hoadon { get; set; } = null!;
    public virtual Sanpham Sanpham { get; set; } = null!;

    // Collection navigation properties
    public ICollection<Tieuhao> Tieuhaos { get; set; } = new List<Tieuhao>();
}
