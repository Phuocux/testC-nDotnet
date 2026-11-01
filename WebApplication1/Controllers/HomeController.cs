using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using MySqlConnector;
using WebApplication1.database;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly dbConnection _db;

    public HomeController(dbConnection db)
    {
        _db = db;
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
        return View();
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
}
