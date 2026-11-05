using MySqlConnector;

namespace WebApplication1.database
{
    public sealed class dbConnection
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _serverConnectionString;

        public dbConnection(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Thiếu DefaultConnection");

            var connectionStringBuilder = new MySqlConnectionStringBuilder(_connectionString);
            _databaseName = connectionStringBuilder.Database;
            if (string.IsNullOrWhiteSpace(_databaseName))
            {
                throw new InvalidOperationException("Chuỗi kết nối chưa có Database=...");
            }

            connectionStringBuilder.Database = string.Empty;
            _serverConnectionString = connectionStringBuilder.ConnectionString;
        }

        public async Task EnsureDatabaseExistsAsync(CancellationToken cancellationToken = default)
        {
            await using var connection = new MySqlConnection(_serverConnectionString);
            await connection.OpenAsync(cancellationToken);

            var quotedDatabaseName = $"`{_databaseName.Replace("`", "``")}`";
            await using var command = new MySqlCommand(
                $"CREATE DATABASE IF NOT EXISTS {quotedDatabaseName} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;",
                connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}