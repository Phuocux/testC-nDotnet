using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(100, ErrorMessage = "Tên đăng nhập tối đa 100 ký tự.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(128, ErrorMessage = "Mật khẩu tối đa 128 ký tự.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
