namespace WebApplication1.Models;

public sealed class AuthPageViewModel
{
    public LoginViewModel Login { get; set; } = new();
    public RegisterViewModel Register { get; set; } = new();
}
