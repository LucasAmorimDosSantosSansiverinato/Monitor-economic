using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Entities;
// using MonitorEconomic.Infra.Data.Clients;
// using MonitorEconomic.Infra.Data.Models;
using MonitorEconomic.Infra.Data.Entities;
using Microsoft.EntityFrameworkCore;


namespace MonitorEconomic.Infra.Data.Repository;

public class IPCRepository : IIPCRepository
{

    //------------------------
    // USA SUPABASE
    //------------------------ 

    // private readonly SupabaseClientFactory _factory;
    // public IPCRepository(SupabaseClientFactory factory)
    // {
    //     _factory = factory;
    // }

    // public IPCRepository(IConfiguration config)
    // {
    //     _connectionString = config.GetConnectionString("DefaultConnection");
    // }

    // public async Task salvarAsync(IPCBaseModel ipcBaseModel)
    // {
    //    var client = await _factory.GetClientAsync();
    //     IPCModel model = IPCMapper.ToModel(ipcBaseModel);     
    //    await client.From<IPCModel>().Insert(model);
    // }

    // public async Task<List<IPCBaseModel>> obterTodosAsync()
    // {
    //     var client = await _factory.GetClientAsync();
    //     var response = await client.From<IPCModel>().Get();
    //     List<IPCBaseModel> lista = new List<IPCBaseModel>();
    //     foreach (var model in response.Models)
    //     {
    //         IPCBaseModel entidade = IPCMapper.ToDomain(model);
    //         lista.Add(entidade);
    //     }
    //     return lista;
    // }

    //------------------------
    // USA DOCKER
    //------------------------ 

    private readonly MonitorEconomicDbContext _context;

    public IPCRepository(MonitorEconomicDbContext context)
    {
        _context = context;
    }

    public async Task salvarAsync(IPCBaseModel ipcBaseModel)
{
    var data = ipcBaseModel.Data;

    // remover UTC
    if (data.Kind == DateTimeKind.Utc)
        data = DateTime.SpecifyKind(data, DateTimeKind.Unspecified);

    var entity = new IPCEntity
    {
        Data = data,
        Valor = ipcBaseModel.Valor
    };

    _context.IPC.Add(entity);
    await _context.SaveChangesAsync();
}

//     public async Task salvarAsync(IPCBaseModel ipcBaseModel)
// {
//     // Garantir que Data seja UTC
//     var data = ipcBaseModel.Data;

//     if (data.Kind == DateTimeKind.Unspecified)
//         data = DateTime.SpecifyKind(data, DateTimeKind.Utc);

//     if (data.Kind == DateTimeKind.Local)
//         data = data.ToUniversalTime();

//     var entity = new IPCEntity
//     {
//         Data = data,  // Agora é sempre UTC
//         Valor = ipcBaseModel.Valor
//     };

//     _context.IPC.Add(entity);
//     await _context.SaveChangesAsync();
// }

    // public async Task salvarAsync(IPCBaseModel ipcBaseModel)
    // {
    //     var entity = new IPCEntity
    //     {
    //         Data = ipcBaseModel.Data, 
    //         Valor = ipcBaseModel.Valor
    //     };

    //     _context.IPC.Add(entity);
    //     await _context.SaveChangesAsync();
    // }


    public async Task<List<IPCBaseModel>> obterTodosAsync()
    {
        var entities = await _context.IPC.ToListAsync();

        // Cria IPCBaseModel usando o construtor
        var lista = entities.Select(e => new IPCBaseModel(e.Data, e.Valor)).ToList();

        return lista;
    }
}