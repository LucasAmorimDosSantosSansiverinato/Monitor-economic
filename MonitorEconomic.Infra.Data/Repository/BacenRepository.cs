using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Infrastructure.Data.Context;
using Npgsql;
using System.Threading;

namespace MonitorEconomic.Infra.Data.Repository;

public class BacenRepository : IBacenRepository
{
    private const string EnsureTableSql = """
        CREATE TABLE IF NOT EXISTS ipc (
            \"Id\" UUID PRIMARY KEY,
            \"Serie\" INTEGER NOT NULL,
            \"Data\" DATE NOT NULL,
            \"Valor\" NUMERIC(10,4) NOT NULL
        );
        """;

    private const string EnsureSerieColumnSql = "ALTER TABLE ipc ADD COLUMN IF NOT EXISTS \"Serie\" INTEGER NOT NULL DEFAULT 1;";

    private const string DeduplicateSql = """
        WITH registros_duplicados AS (
            SELECT ctid,
                   ROW_NUMBER() OVER (PARTITION BY \"Data\", \"Serie\" ORDER BY \"Id\") AS linha
            FROM ipc
        )
        DELETE FROM ipc
        WHERE ctid IN (
            SELECT ctid
            FROM registros_duplicados
            WHERE linha > 1
        );
        """;

    private const string EnsureUniqueConstraintSql = """
        DO $$
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM pg_constraint
                WHERE conname = 'UQ_ipc_Data_Serie'
            ) THEN
                ALTER TABLE ipc
                ADD CONSTRAINT \"UQ_ipc_Data_Serie\" UNIQUE (\"Data\", \"Serie\");
            END IF;
        END
        $$;
        """;

    private static readonly SemaphoreSlim SchemaLock = new(1, 1);
    private static bool _schemaValidated;

    private readonly MonitorEconomicDbContext _context;

    public BacenRepository(MonitorEconomicDbContext context)
    {
        _context = context;
    }

    public async Task salvarAsync(BacenDomain ipcBaseModel, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);

        const string sql = """
            INSERT INTO ipc (\"Id\", \"Serie\", \"Data\", \"Valor\")
            VALUES (@id, @serie, @data, @valor)
            ON CONFLICT (\"Data\", \"Serie\")
            DO UPDATE SET \"Valor\" = EXCLUDED.\"Valor\";
            """;
        
        var parameters = new[]
        {
            new NpgsqlParameter("@id", ipcBaseModel.Id),
            new NpgsqlParameter("@serie", (int)ipcBaseModel.Serie),
            new NpgsqlParameter("@data", ipcBaseModel.Data),
            new NpgsqlParameter("@valor", ipcBaseModel.Valor)
        };

        try
        {
            var linhasAfetadas = await _context.ExecuteNonQueryAsync(sql, parameters, cancellationToken);

            if (linhasAfetadas > 0)
                Console.WriteLine($"✅ Bacen salvo: Id={ipcBaseModel.Id}, Serie={ipcBaseModel.Serie}, Data={ipcBaseModel.Data}, Valor={ipcBaseModel.Valor}, Linhas afetadas={linhasAfetadas}");
            else
                Console.WriteLine("⚠ Nenhuma linha inserida para Bacen.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao salvar dados do Bacen: {ex.Message}");
            throw;
        }
    }

    public async Task<List<BacenDomain>> obterTodosAsync(CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);

        const string sql = "SELECT \"Id\", \"Serie\", \"Data\", \"Valor\" FROM ipc ORDER BY \"Data\" DESC";
        
        var lista = new List<BacenDomain>();

        using (var reader = await _context.ExecuteReaderAsync(sql, cancellationToken: cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken)) 
            {
                var id = reader.GetGuid(0);
                var serie = (MonitorEconomic.Domain.Enums.BacenSerie)reader.GetInt32(1);
                var data = reader.GetDateTime(2);
                var valor = reader.GetDecimal(3);
                lista.Add(new BacenDomain(id, serie, data, valor));
            }
        }

        return lista;
    }

    private async Task EnsureSchemaAsync(CancellationToken cancellationToken)
    {
        if (_schemaValidated)
        {
            return;
        }

        await SchemaLock.WaitAsync(cancellationToken);

        try
        {
            if (_schemaValidated)
            {
                return;
            }

            await _context.ExecuteNonQueryAsync(EnsureTableSql, cancellationToken: cancellationToken);
            await _context.ExecuteNonQueryAsync(EnsureSerieColumnSql, cancellationToken: cancellationToken);
            await _context.ExecuteNonQueryAsync(DeduplicateSql, cancellationToken: cancellationToken);
            await _context.ExecuteNonQueryAsync(EnsureUniqueConstraintSql, cancellationToken: cancellationToken);

            _schemaValidated = true;
        }
        finally
        {
            SchemaLock.Release();
        }
    }
}