using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SystemPraktykZawodowych.Data;

public class DatabaseConnection
{
    private readonly string _connectionString;
    
    public DatabaseConnection(IConfiguration configuration) // Dzięki dependency injection argument sam sie doda
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("Nie znaleziono ConnectionString w konfiguracji");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
    
    
}