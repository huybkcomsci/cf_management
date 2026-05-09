using System.ComponentModel.DataAnnotations;

namespace CafeManagement.DTOs;

public class SanphamListItemDto
{
    public Guid Id { get; set; }
    public string TenSP { get; set; } = string.Empty;
    public string? TenNhom { get; set; }
    public string? DonViTinh { get; set; }
    public decimal GiaBan { get; set; }
    public int SoLuongTon { get; set; }
    public bool IsActive { get; set; }
}

public class SanphamUpsertDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Vui long chon nhom san pham")]
    public Guid IdNhom { get; set; }

    [Required(ErrorMessage = "Ten san pham bat buoc")]
    [StringLength(100, ErrorMessage = "Ten san pham toi da 100 ky tu")]
    public string TenSP { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mo ta toi da 500 ky tu")]
    public string? MoTa { get; set; }

    [Range(0, 1000000000, ErrorMessage = "Gia ban khong hop le")]
    public decimal GiaBan { get; set; }

    [Range(0, 1000000000, ErrorMessage = "Gia nhap khong hop le")]
    public decimal GiaNhap { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "So luong ton khong hop le")]
    public int SoLuongTon { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "So luong ton toi thieu khong hop le")]
    public int SLTonMin { get; set; }

    [StringLength(50, ErrorMessage = "Don vi tinh toi da 50 ky tu")]
    public string? DonViTinh { get; set; }

    public bool IsActive { get; set; } = true;
}

public class SanphamDetailsDto
{
    public Guid Id { get; set; }
    public Guid IdNhom { get; set; }
    public string TenSP { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public decimal GiaBan { get; set; }
    public decimal GiaNhap { get; set; }
    public int SoLuongTon { get; set; }
    public int SLTonMin { get; set; }
    public string? DonViTinh { get; set; }
    public bool IsActive { get; set; }
    public string? TenNhom { get; set; }
    public DateTime NgayTao { get; set; }
    public DateTime NgayCapNhat { get; set; }
}

public class SanphamIndexVm
{
    public IReadOnlyList<SanphamListItemDto> Items { get; set; } = Array.Empty<SanphamListItemDto>();
    public string? Search { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}

public class NhomOptionDto
{
    public Guid Id { get; set; }
    public string TenNhom { get; set; } = string.Empty;
}
