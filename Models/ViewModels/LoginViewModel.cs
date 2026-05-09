using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email bat buoc")]
    [EmailAddress(ErrorMessage = "Email khong hop le")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mat khau bat buoc")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}