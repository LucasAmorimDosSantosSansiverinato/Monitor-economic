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

    public async Task salvarAsync(IPCBaseDomain ipcBaseModel)
    {
        const string sql = "INSERT INTO ipc (data, valor) VALUES (@data, @valor)";
        
        var parameters = new[]
        {
            new NpgsqlParameter("@data", ipcBaseModel.Data),
            new NpgsqlParameter("@valor", ipcBaseModel.Valor)
        };

        _context.ExecuteNonQuery(sql, parameters);
        await Task.CompletedTask;
    }

    public async Task<List<IPCBaseDomain>> obterTodosAsync()
    {
        const string sql = "SELECT data, valor FROM ipc ORDER BY data DESC";
        
        var lista = new List<IPCBaseDomain>();

        using (var reader = _context.ExecuteReader(sql))
        {
            while (reader.Read())
            {
                var data = reader.GetDateTime(0);
                var valor = reader.GetDecimal(1);
                lista.Add(new IPCBaseDomain(data, valor));
            }
        }

        return await Task.FromResult(lista);
    }
}