using Monitor_economic.Monitor_economic.Application.Dto;

namespace Monitor_economic.Monitor_economic.Application.Interfaces.Service
{
    public interface IIPCService
    {
        abstract Task<List<ItemIPCDto>?> ObterIPCAsync(string dataInicial, string dataFinal);
    }
}
