using Monitor_economic.Monitor_economic.Domain.Models;

namespace Monitor_economic._1_Monitor_econimic.Domain.Interfaces.IRepository
{
    public interface IIPCRepository
    {
        Task salvarAsync(IPCModel ipcModel);
        Task<List<IPCModel>> obterTodosAsync();
    }
}
