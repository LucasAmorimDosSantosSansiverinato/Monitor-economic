using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Entities;
using Microsoft.EntityFrameworkCore;


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
    var data = ipcBaseModel.Data;

    // remover UTC
    if (data.Kind == DateTimeKind.Utc)
        data = DateTime.SpecifyKind(data, DateTimeKind.Unspecified);

    var entity = new IPCBaseDomain
    {
        Data = data,
        Valor = ipcBaseModel.Valor
    };

    _context.IPC.Add(entity);
    await _context.SaveChangesAsync();
}


    public async Task<List<IPCBaseDomain>> obterTodosAsync()
    {
        var entities = await _context.IPC.ToListAsync();

        // Cria IPCBaseModel usando o construtor
        var lista = entities.Select(e => new IPCBaseDomain(e.Data, e.Valor)).ToList();

        return lista;
    }
}