using MySqlConnector;

namespace WebApplication1.database
{
    public class dbConection
    {
        private readonly string _connectionString;

        public dbConection(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Thiếu DefaultConnection");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}