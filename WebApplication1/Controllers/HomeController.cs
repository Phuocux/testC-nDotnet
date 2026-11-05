using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MySqlConnector;
using WebApplication1.Models;
using WebApplication1.database;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly dbConnection _db;
    private readonly AccountService _accounts;
    private readonly ILogger<HomeController> _logger;

    public HomeController(dbConnection db, AccountService accounts, ILogger<HomeController> logger)
    {
        _db = db;
        _accounts = accounts;
        _logger = logger;
    }

    public async Task<IActionResult> TestDb()
    {
        try
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();
            return Content("Kết nối MySQL thành công!");
        }
        catch (Exception ex)
        {
            return Content("Lỗi kết nối: " + ex.Message);
        }
    }

    public IActionResult Index()
    {
        ViewData["ActiveTab"] = TempData["ActiveTab"] as string ?? "login";
        ViewData["AuthMessage"] = TempData["AuthMessage"] as string;
        return View(new AuthPageViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind(Prefix = "Register")] RegisterViewModel input)
    {
        ViewData["ActiveTab"] = "register";
        if (!ModelState.IsValid)
        {
            ClearRegistrationPasswords(input);
            return View("Index", new AuthPageViewModel { Register = input });
        }

        try
        {
            var registered = await _accounts.RegisterAsync(input.Username, input.Password);
            if (!registered)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập đã được sử dụng.");
                ClearRegistrationPasswords(input);
                return View("Index", new AuthPageViewModel { Register = input });
            }
        }
        catch (MySqlException exception)
        {
            _logger.LogError(exception, "Database operation failed during account registration.");
            ModelState.AddModelError(string.Empty, "Không thể truy cập dữ liệu tài khoản. Vui lòng kiểm tra cấu hình MySQL và bảng account.");
            ClearRegistrationPasswords(input);
            return View("Index", new AuthPageViewModel { Register = input });
        }

        TempData["ActiveTab"] = "login";
        TempData["AuthMessage"] = "Đăng ký thành công. Bạn có thể đăng nhập.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind(Prefix = "Login")] LoginViewModel input)
    {
        ViewData["ActiveTab"] = "login";
        if (!ModelState.IsValid)
        {
            ClearLoginPassword(input);
            return View("Index", new AuthPageViewModel { Login = input });
        }

        Account? account;
        try
        {
            account = await _accounts.ValidateCredentialsAsync(input.Username, input.Password);
        }
        catch (MySqlException exception)
        {
            _logger.LogError(exception, "Database operation failed during account login.");
            ModelState.AddModelError(string.Empty, "Không thể truy cập dữ liệu tài khoản. Vui lòng kiểm tra cấu hình MySQL và bảng account.");
            ClearLoginPassword(input);
            return View("Index", new AuthPageViewModel { Login = input });
        }

        if (account is null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            ClearLoginPassword(input);
            return View("Index", new AuthPageViewModel { Login = input });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.IdAccount.ToString()),
            new Claim(ClaimTypes.Name, account.Username)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        TempData["AuthMessage"] = $"Đăng nhập thành công. Xin chào {account.Username}!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["AuthMessage"] = "Bạn đã đăng xuất.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void ClearLoginPassword(LoginViewModel input)
    {
        input.Password = string.Empty;
        ModelState.SetModelValue("Login.Password", ValueProviderResult.None);
    }

    private void ClearRegistrationPasswords(RegisterViewModel input)
    {
        input.Password = string.Empty;
        input.ConfirmPassword = string.Empty;
        ModelState.SetModelValue("Register.Password", ValueProviderResult.None);
        ModelState.SetModelValue("Register.ConfirmPassword", ValueProviderResult.None);
    }
}
