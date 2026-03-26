using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Infrastructure.Data.Context;
using Npgsql;

namespace MonitorEconomic.Infra.Data.Repository;

public class IPCRepository : IIPCRepository
{
    private readonly MonitorEconomicDbContext _context;

    public IPCRepository(MonitorEconomicDbContext context)
    {
        _context = context;
    }

    public async Task salvarAsync(IPCDomain ipcBaseModel)
    {
        const string sql = "INSERT INTO ipc (\"Data\", \"Valor\") VALUES (@data, @valor)";
        
        var parameters = new[]
        {
            new NpgsqlParameter("@data", ipcBaseModel.Data),
            new NpgsqlParameter("@valor", ipcBaseModel.Valor)
        };

        try
        {
            var linhasAfetadas = _context.ExecuteNonQuery(sql, parameters);
            Console.WriteLine($"✅ IPC Salvo: Data={ipcBaseModel.Data}, Valor={ipcBaseModel.Valor}, Linhas afetadas={linhasAfetadas}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao salvar IPC: {ex.Message}");
            throw;
        }
    }

    public async Task<List<IPCDomain>> obterTodosAsync()
    {
        const string sql = "SELECT \"Data\", \"Valor\" FROM ipc ORDER BY \"Data\" DESC";
        
        var lista = new List<IPCDomain>();

        using (var reader = _context.ExecuteReader(sql))
        {
            while (reader.Read())
            {
                var data = reader.GetDateTime(0);
                var valor = reader.GetDecimal(1);
                lista.Add(new IPCDomain(data, valor));
            }
        }

        return await Task.FromResult(lista);
    }
}