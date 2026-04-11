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
    public async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        if (_connection.State == System.Data.ConnectionState.Closed)
        {
            await _connection.OpenAsync(cancellationToken);
        }

        return _connection;
    }
    public async Task<NpgsqlDataReader> ExecuteReaderAsync(string sql, NpgsqlParameter[]? parameters = null, CancellationToken cancellationToken = default)
    {
        var command = (await GetConnectionAsync(cancellationToken)).CreateCommand();
        command.CommandText = sql;

        if (parameters is { Length: > 0 })
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteReaderAsync(cancellationToken);
    }
    public async Task<object?> ExecuteScalarAsync(string sql, NpgsqlParameter[]? parameters = null, CancellationToken cancellationToken = default)
    {
        var command = (await GetConnectionAsync(cancellationToken)).CreateCommand();
        command.CommandText = sql;

        if (parameters is { Length: > 0 })
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteScalarAsync(cancellationToken);
    }
    public async Task<int> ExecuteNonQueryAsync(string sql, NpgsqlParameter[]? parameters = null, CancellationToken cancellationToken = default)
    {
        var command = (await GetConnectionAsync(cancellationToken)).CreateCommand();
        command.CommandText = sql;

        if (parameters is { Length: > 0 })
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }
    public async Task<NpgsqlTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return (await GetConnectionAsync(cancellationToken)).BeginTransaction();
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