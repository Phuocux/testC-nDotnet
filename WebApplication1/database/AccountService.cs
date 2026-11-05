using Microsoft.AspNetCore.Identity;
using MySqlConnector;
using WebApplication1.Models;

namespace WebApplication1.database;

public sealed class AccountService
{
    private readonly dbConnection _db;
    private readonly IPasswordHasher<Account> _passwordHasher;

    public AccountService(dbConnection db, IPasswordHasher<Account> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        var account = new Account { Username = username.Trim() };
        account.PasswordHash = _passwordHasher.HashPassword(account, password);

        await using var connection = _db.GetConnection();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO account (username, password)
            VALUES (@username, @password)
            """;

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", account.Username);
        command.Parameters.AddWithValue("@password", account.PasswordHash);

        try
        {
            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (MySqlException exception) when (exception.Number == 1062)
        {
            return false;
        }
    }

    public async Task<Account?> ValidateCredentialsAsync(string username, string password)
    {
        await using var connection = _db.GetConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT id_account, username, password
            FROM account
            WHERE username = @username
            LIMIT 1
            """;

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username.Trim());

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        var account = new Account
        {
            IdAccount = reader.GetInt32(reader.GetOrdinal("id_account")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password"))
        };

        var result = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : account;
    }
}
