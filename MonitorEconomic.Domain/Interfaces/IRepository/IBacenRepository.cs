using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Domain.Interfaces.IRepository
{
    public interface IBacenRepository
    {
        Task salvarAsync(BacenDomain ipcBaseModel, CancellationToken cancellationToken = default);
        Task<List<BacenDomain>> obterTodosAsync(CancellationToken cancellationToken = default);
    }
}
