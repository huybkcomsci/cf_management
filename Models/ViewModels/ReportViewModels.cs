namespace CafeManagement.Models.ViewModels;

public class RevenueFilterViewModel
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
}

public class RevenueSummaryViewModel
{
    public decimal TotalRevenue { get; set; }
    public int InvoiceCount { get; set; }
    public List<RevenuePointViewModel> DailySeries { get; set; } = new();
    public List<RevenuePointViewModel> MonthlySeries { get; set; } = new();
    public List<RevenuePointViewModel> YearlySeries { get; set; } = new();
    public List<TopProductViewModel> TopProducts { get; set; } = new();
}

public class RevenuePointViewModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class TopProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
}

public class InventoryReportRowViewModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public int Nhap { get; set; }
    public int Xuat { get; set; }
    public int TonCuoi { get; set; }
    public decimal GiaTriTonKho { get; set; }
    public bool SapHetHang { get; set; }
}

public class InventoryReportViewModel
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<InventoryReportRowViewModel> Rows { get; set; } = new();
    public decimal TongGiaTriTonKho { get; set; }
    public int SoMatHangSapHet { get; set; }
}

public class AdminDashboardViewModel
{
    public decimal RevenueToday { get; set; }
    public int InvoiceToday { get; set; }
    public string? TopProductToday { get; set; }
    public int LowStockCount { get; set; }
    public int ActiveEmployees { get; set; }
    public RevenueSummaryViewModel RevenueSummary { get; set; } = new();
}
