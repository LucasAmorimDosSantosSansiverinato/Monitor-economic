using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Domain.Interfaces.IRepository
{
    public interface IBacenRepository
    {
        Task salvarAsync(BacenDomain bacen, CancellationToken cancellationToken = default);
        Task<List<BacenDomain>> obterPorPeriodoAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, CancellationToken cancellationToken = default);
    }
}
