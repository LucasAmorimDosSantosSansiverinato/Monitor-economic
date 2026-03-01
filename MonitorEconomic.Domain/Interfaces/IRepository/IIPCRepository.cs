using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Domain.Interfaces.IRepository
{
    public interface IIPCRepository
    {
        Task salvarAsync(IPCBaseModel ipcBaseModel);
        Task<List<IPCBaseModel>> obterTodosAsync();
    }
}
