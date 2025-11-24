using Monitor_economic.Monitor_economic.Application.Dto;

namespace Monitor_economic.Monitor_economic.Application.Interfaces.Service
{
    public interface IIPCService
    {
        Task<IPCDto?> ObeterIPCAsync(string dataInicial, string dataFinal);
    }
}
