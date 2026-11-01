using MySqlConnector;

namespace WebApplication1.database
{
    public class dbConnection
    {
        private readonly string _connectionString;

        public dbConnection(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Thiếu DefaultConnection");
              var csb = new MySqlConnectionStringBuilder(_connectionString);
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

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}