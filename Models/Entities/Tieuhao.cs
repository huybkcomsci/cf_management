using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Tiêu hao (Product Usage/Waste)
/// </summary>
[Table("Tieuhao")]
public class Tieuhao
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Sanpham))]
    public Guid IdSP { get; set; }

    [Required]
    [ForeignKey(nameof(HoadonCT))]
    public Guid IdHoadonCT { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal DonGiaVon { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual Sanpham Sanpham { get; set; } = null!;
    public virtual HoadonCT HoadonCT { get; set; } = null!;
}
