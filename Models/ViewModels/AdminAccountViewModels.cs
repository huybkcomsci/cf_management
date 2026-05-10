using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models.ViewModels;

public class AdminUserAccountItemViewModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class AdminCreateAccountViewModel
{
    [Required(ErrorMessage = "Tên nhân viên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên nhân viên tối đa 100 ký tự")]
    public string EmployeeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Xác nhận mật khẩu không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Chức vụ là bắt buộc")]
    [RegularExpression("^(Kế toán|Thu ngân)$", ErrorMessage = "Chỉ được chọn Kế toán hoặc Thu ngân")]
    public string Role { get; set; } = "Kế toán";
}

public class AdminChangePasswordViewModel
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Xác nhận mật khẩu không khớp")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class AdminAccountsPageViewModel
{
    public List<AdminUserAccountItemViewModel> Accounts { get; set; } = new();
    public AdminCreateAccountViewModel CreateAccount { get; set; } = new();
}
