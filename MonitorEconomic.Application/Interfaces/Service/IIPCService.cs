using MonitorEconomic.Application.Dto;

namespace MonitorEconomic.Application.Interfaces.Service
{
    public interface IIPCService
    {
        abstract Task<List<ItemIPCDto>?> obterIPCAsync(string dataInicial, string dataFinal);
    }
}
