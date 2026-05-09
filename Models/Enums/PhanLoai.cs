namespace CafeManagement.Models.Enums;

/// <summary>
/// Classification/Category types used across the system
/// </summary>
public enum PhanLoai
{
    /// <summary>Đồ uống (Beverages)</summary>
    DoUong = 1,
    /// <summary>Thức ăn (Foods)</summary>
    ThucAn = 2,
    /// <summary>Trang miệng (Desserts)</summary>
    TrangMieng = 3,
    /// <summary>Phụ liệu (Supplies)</summary>
    PhuLieu = 4
}

/// <summary>
/// Document/Invoice status
/// </summary>
public enum TrangThai
{
    /// <summary>Nháp (Draft)</summary>
    Nhap = 0,
    /// <summary>đã phê duyệt (Approved)</summary>
    PheDuyet = 1,
    /// <summary>Đã hoàn thành (Completed)</summary>
    HoanThanh = 2,
    /// <summary>Đã hủy (Cancelled)</summary>
    Huy = 3,
    /// <summary>Đang chờ (Pending)</summary>
    DangCho = 4
}

/// <summary>
/// Payment method types
/// </summary>
public enum PhuongThucThanhToan
{
    /// <summary>Tiền mặt (Cash)</summary>
    TienMat = 1,
    /// <summary>Chuyển khoản (Bank transfer)</summary>
    ChuyenKhoan = 2,
    /// <summary>Thẻ tín dụng (Credit card)</summary>
    TheTheCheck = 3,
    /// <summary>Tiền điện tử (E-wallet)</summary>
    TienDienTu = 4,
    /// <summary>Ghi nợ (Payable)</summary>
    GhiNo = 5
}

/// <summary>
/// Expense/Payment type
/// </summary>
public enum LoaiChiPhi
{
    /// <summary>Lương (Salary)</summary>
    Luong = 1,
    /// <summary>Điện (Electricity)</summary>
    Dien = 2,
    /// <summary>Nước (Water)</summary>
    Nuoc = 3,
    /// <summary>Thuê mặt bằng (Rent)</summary>
    ThueMat = 4,
    /// <summary>Bảo trì (Maintenance)</summary>
    BaoTri = 5,
    /// <summary>Vệ sinh (Cleaning)</summary>
    VeSinh = 6,
    /// <summary>Quảng cáo (Advertising)</summary>
    QuangCao = 7,
    /// <summary>Khác (Other)</summary>
    Khac = 8
}

/// <summary>
/// Employee status
/// </summary>
public enum TrangThaiNhanvien
{
    /// <summary>Đang làm việc (Active)</summary>
    DangLam = 1,
    /// <summary>Tạm dừng (On leave)</summary>
    TamDung = 2,
    /// <summary>Đã nghỉ việc (Resigned)</summary>
    DaNghi = 3
}

/// <summary>
/// Attendance status
/// </summary>
public enum TrangThaiChamCong
{
    /// <summary>Có mặt (Present)</summary>
    CoMat = 1,
    /// <summary>Vắng mặt (Absent)</summary>
    VangMat = 2,
    /// <summary>Tế nhị (Late)</summary>
    TeNhi = 3,
    /// <summary>Phép (Leave)</summary>
    Phep = 4
}
