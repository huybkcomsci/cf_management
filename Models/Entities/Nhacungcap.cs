using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Nhà cung cấp (Supplier)
/// </summary>
[Table("Nhacungcap")]
public class Nhacungcap
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TenNCC { get; set; } = null!;

    [StringLength(20)]
    public string? Sdt { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? DiaChi { get; set; }

    [StringLength(100)]
    public string? NguoiDaiDien { get; set; }

    [StringLength(20)]
    public string? TaxID { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal? CongNo { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Phieunhap> Phieunhaps { get; set; } = new List<Phieunhap>();
}
