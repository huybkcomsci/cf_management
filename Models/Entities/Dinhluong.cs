using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Định lượng (Recipe/Formula - defining ingredient composition)
/// </summary>
[Table("Dinhluong")]
public class Dinhluong
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Sanpham))]
    public Guid IdSP { get; set; }

    [Required]
    [ForeignKey(nameof(ThanhphanSanpham))]
    public Guid IdThanhPhan { get; set; }

    public int SoLuong { get; set; }

    [StringLength(50)]
    public string? DonVi { get; set; }

    [StringLength(200)]
    public string? GhiChu { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Foreign key navigation
    public virtual Sanpham Sanpham { get; set; } = null!;
    public virtual Sanpham ThanhphanSanpham { get; set; } = null!;
}
