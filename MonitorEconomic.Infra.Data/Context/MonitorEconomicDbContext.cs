using Npgsql;

namespace MonitorEconomic.Infrastructure.Data.Context;

public class MonitorEconomicDbContext : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    public MonitorEconomicDbContext(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }
    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        if (_connection.State == System.Data.ConnectionState.Closed)
        {
            await _connection.OpenAsync();
        }

        return _connection;
    }
    public async Task<NpgsqlDataReader> ExecuteReaderAsync(string sql, params NpgsqlParameter[] parameters)
    {
        var command = (await GetConnectionAsync()).CreateCommand();
        command.CommandText = sql;

        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteReaderAsync();
    }
    public async Task<object?> ExecuteScalarAsync(string sql, params NpgsqlParameter[] parameters)
    {
        var command = (await GetConnectionAsync()).CreateCommand();
        command.CommandText = sql;

        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteScalarAsync();
    }
    public async Task<int> ExecuteNonQueryAsync(string sql, params NpgsqlParameter[] parameters)
    {
        var command = (await GetConnectionAsync()).CreateCommand();
        command.CommandText = sql;

        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteNonQueryAsync();
    }
    public async Task<NpgsqlTransaction> BeginTransactionAsync()
    {
        return (await GetConnectionAsync()).BeginTransaction();
    }
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}