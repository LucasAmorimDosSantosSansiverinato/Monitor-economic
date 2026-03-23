using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Domain.Interfaces.IRepository
{
    public interface IIPCRepository
    {
        Task salvarAsync(IPCBaseDomain ipcBaseModel);
        Task<List<IPCBaseDomain>> obterTodosAsync();
    }
}
