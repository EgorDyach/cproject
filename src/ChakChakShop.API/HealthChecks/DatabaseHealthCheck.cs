using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace ChakChakShop.API.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public DatabaseHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            
            await using var command = new NpgsqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("Database is available");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unavailable", ex);
        }
    }
}


