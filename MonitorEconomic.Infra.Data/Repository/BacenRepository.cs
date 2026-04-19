using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Infrastructure.Data.Context;
using Npgsql;

namespace MonitorEconomic.Infra.Data.Repository;

public class BacenRepository : IBacenRepository
{
    private static readonly SemaphoreSlim SchemaLock = new(1, 1);
    private static readonly HashSet<string> ValidatedTables = new(StringComparer.Ordinal);

    private readonly MonitorEconomicDbContext _context;

    public BacenRepository(MonitorEconomicDbContext context)
    {
        _context = context;
    }

    public async Task salvarAsync(BacenDomain bacen, CancellationToken cancellationToken = default)
    {
        var tableName = ObterNomeTabela(bacen.Serie);
        await EnsureTableAsync(tableName, cancellationToken);

        var sql = $"""
            INSERT INTO {tableName} ("Id", "Data", "Valor")
            VALUES (@id, @data, @valor)
            ON CONFLICT ("Data")
            DO UPDATE SET "Valor" = EXCLUDED."Valor";
            """;

        var parameters = new[]
        {
            new NpgsqlParameter("@id", bacen.Id),
            new NpgsqlParameter("@data", bacen.Data),
            new NpgsqlParameter("@valor", bacen.Valor)
        };

        await _context.ExecuteNonQueryAsync(sql, parameters, cancellationToken);
    }

    public async Task<List<BacenDomain>> obterPorPeriodoAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, CancellationToken cancellationToken = default)
    {
        var tableName = ObterNomeTabela(serie);
        await EnsureTableAsync(tableName, cancellationToken);

        var sql = $"""
            SELECT "Id", "Data", "Valor"
            FROM {tableName}
            WHERE "Data" BETWEEN @dataInicial AND @dataFinal
            ORDER BY "Data" ASC;
            """;

        var parameters = new[]
        {
            new NpgsqlParameter("@dataInicial", dataInicial.Date),
            new NpgsqlParameter("@dataFinal", dataFinal.Date)
        };

        var registros = new List<BacenDomain>();

        using var reader = await _context.ExecuteReaderAsync(sql, parameters, cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            registros.Add(new BacenDomain(
                reader.GetGuid(0),
                serie,
                reader.GetDateTime(1),
                reader.GetDecimal(2)));
        }

        return registros;
    }

    private async Task EnsureTableAsync(string tableName, CancellationToken cancellationToken)
    {
        if (ValidatedTables.Contains(tableName))
        {
            return;
        }

        await SchemaLock.WaitAsync(cancellationToken);

        try
        {
            if (ValidatedTables.Contains(tableName))
            {
                return;
            }

            await EnsureTimescaleExtensionAsync(cancellationToken);

            var tableExists = await TableExistsAsync(tableName, cancellationToken);
            var hasExpectedSchema = tableExists && await HasExpectedSchemaAsync(tableName, cancellationToken);
            var isHypertable = tableExists && await IsHypertableAsync(tableName, cancellationToken);

            if (!tableExists || !hasExpectedSchema || !isHypertable)
            {
                await RecreateTableAsync(tableName, cancellationToken);
            }

            ValidatedTables.Add(tableName);
        }
        finally
        {
            SchemaLock.Release();
        }
    }

    private async Task EnsureTimescaleExtensionAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            CREATE EXTENSION IF NOT EXISTS timescaledb;
            """;

        await _context.ExecuteNonQueryAsync(sql, cancellationToken: cancellationToken);
    }

    private async Task<bool> TableExistsAsync(string tableName, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_name = @tableName
            );
            """;

        var parameters = new[]
        {
            new NpgsqlParameter("@tableName", tableName)
        };

        var result = await _context.ExecuteScalarAsync(sql, parameters, cancellationToken);
        return result is bool exists && exists;
    }

    private async Task<bool> HasExpectedSchemaAsync(string tableName, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT column_name, data_type
            FROM information_schema.columns
            WHERE table_schema = 'public'
              AND table_name = @tableName
            ORDER BY ordinal_position;
            """;

        var parameters = new[]
        {
            new NpgsqlParameter("@tableName", tableName)
        };

        var columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        using var reader = await _context.ExecuteReaderAsync(sql, parameters, cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            columns[reader.GetString(0)] = reader.GetString(1);
        }

        return columns.Count == 3
            && columns.TryGetValue("Id", out var idType)
            && string.Equals(idType, "uuid", StringComparison.OrdinalIgnoreCase)
            && columns.TryGetValue("Data", out var dataType)
            && string.Equals(dataType, "date", StringComparison.OrdinalIgnoreCase)
            && columns.TryGetValue("Valor", out var valorType)
            && string.Equals(valorType, "numeric", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> IsHypertableAsync(string tableName, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM timescaledb_information.hypertables
                WHERE hypertable_schema = 'public'
                  AND hypertable_name = @tableName
            );
            """;

        var parameters = new[]
        {
            new NpgsqlParameter("@tableName", tableName)
        };

        var result = await _context.ExecuteScalarAsync(sql, parameters, cancellationToken);
        return result is bool exists && exists;
    }

    private async Task RecreateTableAsync(string tableName, CancellationToken cancellationToken)
    {
        var dropTableSql = $"DROP TABLE IF EXISTS {tableName};";
        var createTableSql = $"""
            CREATE TABLE {tableName} (
                "Id" UUID NOT NULL,
                "Data" DATE NOT NULL,
                "Valor" NUMERIC(10,4) NOT NULL
            );
            """;
        var createHypertableSql = $"""
            SELECT create_hypertable('{tableName}', 'Data', if_not_exists => true);
            """;
        var createIndexSql = $"""
            CREATE UNIQUE INDEX "IX_{tableName}_Data_Unique" ON {tableName} ("Data");
            """;

        await _context.ExecuteNonQueryAsync(dropTableSql, cancellationToken: cancellationToken);
        await _context.ExecuteNonQueryAsync(createTableSql, cancellationToken: cancellationToken);
        await _context.ExecuteScalarAsync(createHypertableSql, cancellationToken: cancellationToken);
        await _context.ExecuteNonQueryAsync(createIndexSql, cancellationToken: cancellationToken);
    }

    private static string ObterNomeTabela(BacenSerie serie)
    {
        if (!Enum.IsDefined(serie))
        {
            throw new InvalidOperationException($"Serie sem tabela configurada: {serie}.");
        }

        return serie.ToString().ToLowerInvariant();
    }
}