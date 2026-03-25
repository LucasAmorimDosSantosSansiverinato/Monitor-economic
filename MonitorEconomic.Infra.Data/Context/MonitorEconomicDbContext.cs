using Npgsql;

namespace MonitorEconomic.Infrastructure.Data.Context;

/// <summary>
/// Contexto de acesso ao banco de dados PostgreSQL.
/// Gerencia a conexão com o banco de dados sem usar ORM.
/// </summary>
public class MonitorEconomicDbContext : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    public MonitorEconomicDbContext(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Obtém uma conexão aberta com o banco de dados.
    /// </summary>
    public NpgsqlConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        if (_connection.State == System.Data.ConnectionState.Closed)
        {
            _connection.Open();
        }

        return _connection;
    }

    /// <summary>
    /// Executa um comando SQL retornando um reader.
    /// </summary>
    public NpgsqlDataReader ExecuteReader(string sql, params NpgsqlParameter[] parameters)
    {
        var command = GetConnection().CreateCommand();
        command.CommandText = sql;
        
        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return command.ExecuteReader();
    }

    /// <summary>
    /// Executa um comando SQL retornando um scalar.
    /// </summary>
    public object? ExecuteScalar(string sql, params NpgsqlParameter[] parameters)
    {
        var command = GetConnection().CreateCommand();
        command.CommandText = sql;
        
        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return command.ExecuteScalar();
    }

    /// <summary>
    /// Executa um comando SQL não-query (INSERT, UPDATE, DELETE).
    /// </summary>
    public int ExecuteNonQuery(string sql, params NpgsqlParameter[] parameters)
    {
        var command = GetConnection().CreateCommand();
        command.CommandText = sql;
        
        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Inicia uma transação no banco de dados.
    /// </summary>
    public NpgsqlTransaction BeginTransaction()
    {
        return GetConnection().BeginTransaction();
    }

    /// <summary>
    /// Testa a conexão com o banco de dados.
    /// </summary>
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