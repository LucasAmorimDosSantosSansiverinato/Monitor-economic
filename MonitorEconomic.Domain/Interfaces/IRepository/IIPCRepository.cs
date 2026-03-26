using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Domain.Interfaces.IRepository
{
    public interface IIPCRepository
    {
        Task salvarAsync(IPCDomain ipcBaseModel);
        Task<List<IPCDomain>> obterTodosAsync();
    }
}
