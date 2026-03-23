using MonitorEconomic.Application.Dto;

namespace MonitorEconomic.Application.Interfaces.Service
{
    public interface IIPCService
    {
        Task<List<ItemIPCDto>?> obterIPCAsync(string dataInicial, string dataFinal);
    }
}
