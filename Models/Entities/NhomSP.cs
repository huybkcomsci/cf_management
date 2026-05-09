using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeManagement.Models.Enums;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Nhóm sản phẩm (Product Category/Group)
/// </summary>
[Table("NhomSP")]
public class NhomSP
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TenNhom { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    public PhanLoai PhanLoai { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Sanpham> SanPhams { get; set; } = new List<Sanpham>();
}
