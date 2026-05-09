using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeManagement.Models.Entities;

/// <summary>
/// Sản phẩm (Product) - keeping this for backward compatibility with ProductService
/// Will be merged with Sanpham if not needed separately
/// </summary>
[Table("Products")]
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal Price { get; set; }
}
