using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Infra.Data.Clients;
using MonitorEconomic.Infra.Data.Models;

namespace MonitorEconomic.Infra.Data.Repository;

public class IPCRepository : IIPCRepository
{
    private readonly SupabaseClientFactory _factory;

    public IPCRepository(SupabaseClientFactory factory)
    {
        _factory = factory;
    }

    public async Task salvarAsync(IPCBaseModel ipcBaseModel)
    {
        var client = await _factory.GetClientAsync();

        IPCModel model = IPCMapper.ToModel(ipcBaseModel);
        

        await client.From<IPCModel>().Insert(model);
    }

    public async Task<List<IPCBaseModel>> obterTodosAsync()
    {
        var client = await _factory.GetClientAsync();

        var response = await client.From<IPCModel>().Get();

        List<IPCBaseModel> lista = new List<IPCBaseModel>();

        foreach (var model in response.Models)
        {
            IPCBaseModel entidade = IPCMapper.ToDomain(model);
            lista.Add(entidade);
        }

        return lista;
    }
}