using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SystemPraktykZawodowych.Data;

public class DatabaseConnection
{
    private readonly string _connectionString;
    private readonly int _maxRetries = 3;
    private int _retryDelayMs = 1000;
    
    public DatabaseConnection(IConfiguration configuration) // Dzięki dependency injection argument sam sie doda
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("ConnectionString not found in configuration");
    }

    public IDbConnection CreateConnection()
    {
        Console.WriteLine("Connecting to database...");
        return CreateConnectionWithRetry();
    }

    private SqlConnection CreateConnectionWithRetry() {
        SqlConnection connection = null;
        int attemptCount = 0;
        bool connected = false;

        while (!connected && attemptCount < _maxRetries) {
            try {
                attemptCount++;
                connection = new SqlConnection(_connectionString);
                connection.Open();
                connected = true;

                if (attemptCount > 1)
                {
                    Console.WriteLine($"Database connection successful after {attemptCount} attempts. \n");
                }
            }
            catch (SqlException ex) {
                if (attemptCount == _maxRetries)
                {
                    Console.WriteLine($"Failed to connect to database after {_maxRetries} attempts. Error: {ex.Message}");
                    throw;
                }

                Console.WriteLine($"Attempt {attemptCount} failed: {ex.Message}. Retrying in {_retryDelayMs} ms...");
                Thread.Sleep(_retryDelayMs);
                _retryDelayMs *= 2;
            }
        }
        return connection;
    }
    
    
}