using Microsoft.AspNetCore.Builder;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<WebApplication1.database.dbConnection>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

//void CreateDatabaseIfNotExists()
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Thiếu ConnectionStrings:DefaultConnection trong appsettings.json");

    // Bỏ phần Database ra khỏi chuỗi kết nối
    var csb = new MySqlConnectionStringBuilder(connectionString);
    var dbName = csb.Database;

    if (string.IsNullOrWhiteSpace(dbName))
        throw new InvalidOperationException("Chuỗi kết nối chưa có Database=...");

    csb.Database = "";

    using var connection = new MySqlConnection(csb.ConnectionString);
    connection.Open();

    using var cmd = new MySqlCommand(
        $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;",
        connection);
    cmd.ExecuteNonQuery();

    Console.WriteLine($"Database '{dbName}' sẵn sàng.");
}