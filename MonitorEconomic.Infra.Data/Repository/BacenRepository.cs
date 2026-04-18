using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
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
        await EnsureSchemaAsync(tableName, cancellationToken);

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

        try
        {
            var linhasAfetadas = await _context.ExecuteNonQueryAsync(sql, parameters, cancellationToken);

            if (linhasAfetadas > 0)
                Console.WriteLine($"✅ Bacen salvo: Tabela={tableName}, Id={bacen.Id}, Serie={bacen.Serie}, Data={bacen.Data}, Valor={bacen.Valor}, Linhas afetadas={linhasAfetadas}");
            else
                Console.WriteLine("⚠ Nenhuma linha inserida para Bacen.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao salvar dados do Bacen: {ex.Message}");
            throw;
        }
    }

    public async Task<List<BacenDomain>> obterPorPeriodoAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, CancellationToken cancellationToken = default)
    {
        var tableName = ObterNomeTabela(serie);
        await EnsureSchemaAsync(tableName, cancellationToken);

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

    private async Task EnsureSchemaAsync(string tableName, CancellationToken cancellationToken)
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

            var schema = await ObterSchemaTabelaAsync(tableName, cancellationToken);

            if (!schema.Existe)
            {
                await CriarTabelaAsync(tableName, cancellationToken);
            }
            else if (!schema.EhCompativel)
            {
                await MigrarTabelaLegadaAsync(tableName, cancellationToken);
            }

            await RemoverDuplicadosAsync(tableName, cancellationToken);
            await GarantirConstraintUnicaAsync(tableName, cancellationToken);

            ValidatedTables.Add(tableName);
        }
        finally
        {
            SchemaLock.Release();
        }
    }

    private async Task<TabelaSchemaInfo> ObterSchemaTabelaAsync(string tableName, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT column_name, data_type
            FROM information_schema.columns
            WHERE table_schema = 'public'
              AND table_name = @tableName
            ORDER BY ordinal_position;
            """;

        var columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var parameters = new[]
        {
            new NpgsqlParameter("@tableName", tableName)
        };

        using var reader = await _context.ExecuteReaderAsync(sql, parameters, cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            columns[reader.GetString(0)] = reader.GetString(1);
        }

        return new TabelaSchemaInfo(columns);
    }

    private async Task CriarTabelaAsync(string tableName, CancellationToken cancellationToken)
    {
        var sql = $"""
            CREATE TABLE IF NOT EXISTS {tableName} (
                "Id" UUID PRIMARY KEY,
                "Data" DATE NOT NULL,
                "Valor" NUMERIC(10,4) NOT NULL
            );
            """;

        await _context.ExecuteNonQueryAsync(sql, cancellationToken: cancellationToken);
    }

    private async Task MigrarTabelaLegadaAsync(string tableName, CancellationToken cancellationToken)
    {
        var tempTableName = $"{tableName}_schema_fix";
        var backupTableName = $"{tableName}_legacy_{DateTime.UtcNow:yyyyMMddHHmmss}";
        var hashExpression = "md5(\"Data\"::text || '|' || COALESCE(\"Valor\"::text, ''))";

        var dropTempTableSql = $"DROP TABLE IF EXISTS {tempTableName};";
        await _context.ExecuteNonQueryAsync(dropTempTableSql, cancellationToken: cancellationToken);

        await CriarTabelaAsync(tempTableName, cancellationToken);

        var migrateDataSql = $"""
            INSERT INTO {tempTableName} ("Id", "Data", "Valor")
            SELECT DISTINCT ON ("Data"::date)
                (
                    SUBSTRING({hashExpression}, 1, 8) || '-' ||
                    SUBSTRING({hashExpression}, 9, 4) || '-' ||
                    SUBSTRING({hashExpression}, 13, 4) || '-' ||
                    SUBSTRING({hashExpression}, 17, 4) || '-' ||
                    SUBSTRING({hashExpression}, 21, 12)
                )::uuid,
                "Data"::date,
                ROUND("Valor"::numeric, 4)
            FROM {tableName}
            ORDER BY "Data"::date, "Id";
            """;

        var renameOldTableSql = $"ALTER TABLE {tableName} RENAME TO {backupTableName};";
        var renameTempTableSql = $"ALTER TABLE {tempTableName} RENAME TO {tableName};";

        await _context.ExecuteNonQueryAsync(migrateDataSql, cancellationToken: cancellationToken);
        await _context.ExecuteNonQueryAsync(renameOldTableSql, cancellationToken: cancellationToken);
        await _context.ExecuteNonQueryAsync(renameTempTableSql, cancellationToken: cancellationToken);
    }

    private async Task RemoverDuplicadosAsync(string tableName, CancellationToken cancellationToken)
    {
        var sql = $"""
            WITH registros_duplicados AS (
                SELECT ctid,
                       ROW_NUMBER() OVER (PARTITION BY "Data" ORDER BY "Id") AS linha
                FROM {tableName}
            )
            DELETE FROM {tableName}
            WHERE ctid IN (
                SELECT ctid
                FROM registros_duplicados
                WHERE linha > 1
            );
            """;

        await _context.ExecuteNonQueryAsync(sql, cancellationToken: cancellationToken);
    }

    private async Task GarantirConstraintUnicaAsync(string tableName, CancellationToken cancellationToken)
    {
        var indexName = $"IX_{tableName}_Data_Unique";
        var sql = $"""
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1
                    FROM pg_indexes
                    WHERE schemaname = 'public'
                      AND tablename = '{tableName}'
                      AND (
                          indexname = '{indexName}'
                          OR indexdef ILIKE 'CREATE UNIQUE INDEX%("Data")%'
                      )
                ) THEN
                    CREATE UNIQUE INDEX "{indexName}" ON {tableName} ("Data");
                END IF;
            END
            $$;
            """;

        await _context.ExecuteNonQueryAsync(sql, cancellationToken: cancellationToken);
    }

    private static string ObterNomeTabela(BacenSerie serie)
    {
        if (!Enum.IsDefined(serie))
            throw new InvalidOperationException($"Serie sem tabela configurada: {serie}.");

        return serie.ToString().ToLowerInvariant();
    }

    private sealed class TabelaSchemaInfo
    {
        public TabelaSchemaInfo(IReadOnlyDictionary<string, string> columns)
        {
            Columns = columns;
        }

        private IReadOnlyDictionary<string, string> Columns { get; }

        public bool Existe => Columns.Count > 0;

        public bool EhCompativel =>
            Columns.TryGetValue("Id", out var idType) && string.Equals(idType, "uuid", StringComparison.OrdinalIgnoreCase)
            && Columns.TryGetValue("Data", out var dataType) && string.Equals(dataType, "date", StringComparison.OrdinalIgnoreCase)
            && Columns.TryGetValue("Valor", out var valorType) && string.Equals(valorType, "numeric", StringComparison.OrdinalIgnoreCase);
    }
}