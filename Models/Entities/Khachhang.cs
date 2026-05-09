using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Khách hàng (Customer)
/// </summary>
[Table("Khachhang")]
public class Khachhang
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TenKH { get; set; } = null!;

    [StringLength(20)]
    public string? Sdt { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? DiaChi { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal? CongNo { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "timestamp")]
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "timestamp")]
    public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
}
