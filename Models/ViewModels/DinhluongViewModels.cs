using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CafeManagement.Models.ViewModels;

public class DinhluongListItemViewModel
{
    public Guid Id { get; set; }
    public Guid IdSP { get; set; }
    public Guid IdThanhPhan { get; set; }
    public string Sanpham { get; set; } = string.Empty;
    public string Thanhphan { get; set; } = string.Empty;
    public string SanphamName => Sanpham;
    public string ThanhphanName => Thanhphan;
    public int SoLuong { get; set; }
    public string? DonVi { get; set; }
    public string? GhiChu { get; set; }
}

public class DinhluongEditViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Vui long chon san pham")]
    public Guid IdSP { get; set; }

    [Required(ErrorMessage = "Vui long chon nguyen lieu")]
    public Guid IdThanhPhan { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "So luong phai lon hon 0")]
    public int SoLuong { get; set; }

    [StringLength(50)]
    public string? DonVi { get; set; }

    [StringLength(200)]
    public string? GhiChu { get; set; }

    public List<SelectListItem> SanphamOptions { get; set; } = new();
    public List<SelectListItem> ThanhphanOptions { get; set; } = new();
    public List<SelectListItem> SanphamList { get; set; } = new();
}
