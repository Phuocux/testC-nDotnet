namespace WebApplication1.Models;

public sealed class Account
{
    public int IdAccount { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
