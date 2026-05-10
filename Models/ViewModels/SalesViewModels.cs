using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CafeManagement.Models.ViewModels;

public class SalesPageViewModel
{
    public List<SalesProductItemViewModel> MenuItems { get; set; } = new();
}

public class SalesProductItemViewModel
{
    public Guid Id { get; set; }
    public string TenSP { get; set; } = string.Empty;
    public string? TenNhom { get; set; }
    public decimal GiaBan { get; set; }
    public int SoLuongTon { get; set; }
    public string? DonViTinh { get; set; }
}

public class SalesLineInputViewModel
{
    [JsonPropertyName("sanphamId")]
    [Required]
    public Guid SanphamId { get; set; }

    [JsonPropertyName("soLuong")]
    [Range(1, int.MaxValue, ErrorMessage = "So luong phai lon hon 0")]
    public int SoLuong { get; set; }
}

public class SalesCheckoutRequestViewModel
{
    [JsonPropertyName("khachhangId")]
    public Guid? KhachhangId { get; set; }

    [JsonPropertyName("giamGia")]
    [Range(0, 1000000000)]
    public decimal GiamGia { get; set; }

    [JsonPropertyName("ghiChu")]
    [StringLength(200)]
    public string? GhiChu { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Hoa don phai co it nhat 1 mon")]
    public List<SalesLineInputViewModel> Items { get; set; } = new();
}

public class SalesCheckoutResultViewModel
{
    public Guid HoadonId { get; set; }
    public string MaHD { get; set; } = string.Empty;
    public decimal ThanhTien { get; set; }
    public decimal GiamGia { get; set; }
    public decimal TongCong { get; set; }
}

public class SalesInvoiceHistoryItemViewModel
{
    public Guid Id { get; set; }
    public string MaHD { get; set; } = string.Empty;
    public string TenKhach { get; set; } = "Khach le";
    public string ThuNgan { get; set; } = "He thong";
    public DateTime NgayLap { get; set; }
    public decimal ThanhTien { get; set; }
    public decimal GiamGia { get; set; }
    public decimal TongCong { get; set; }
    public int SoMon { get; set; }
}

public class SalesPrintViewModel
{
    public Guid HoadonId { get; set; }
    public string MaHD { get; set; } = string.Empty;
    public string TenKhach { get; set; } = "Khach le";
    public string ThuNgan { get; set; } = "He thong";
    public DateTime NgayLap { get; set; }
    public decimal ThanhTien { get; set; }
    public decimal GiamGia { get; set; }
    public decimal TongCong { get; set; }
    public List<SalesPrintLineViewModel> Lines { get; set; } = new();
}

public class SalesPrintLineViewModel
{
    public string TenSP { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}

public class SalesInvoiceDetailViewModel
{
    public Guid HoadonId { get; set; }
    public string MaHD { get; set; } = string.Empty;
    public string TenKhach { get; set; } = "Khach le";
    public string ThuNgan { get; set; } = "He thong";
    public DateTime NgayLap { get; set; }
    public decimal ThanhTien { get; set; }
    public decimal GiamGia { get; set; }
    public decimal TongCong { get; set; }
    public List<SalesInvoiceDetailLineViewModel> Lines { get; set; } = new();
}

public class SalesInvoiceDetailLineViewModel
{
    public string TenSP { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}

public class ProductListItemViewModel
{
    public Guid Id { get; set; }
    public string TenSP { get; set; } = string.Empty;
    public string? TenNhom { get; set; }
    public decimal GiaBan { get; set; }
    public int SoLuongTon { get; set; }
    public string? DonViTinh { get; set; }
    public bool IsActive { get; set; }
}


public class DailySalesReportViewModel
{
    public DateTime Ngay { get; set; }
    public int TongSoHD { get; set; }
    public decimal TongThanhTien { get; set; }
    public decimal TongGiamGia { get; set; }
    public decimal TongCong { get; set; }
    public List<DailySalesProductLineViewModel> Products { get; set; } = new();
}

public class DailySalesProductLineViewModel
{
    public string TenSP { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}
