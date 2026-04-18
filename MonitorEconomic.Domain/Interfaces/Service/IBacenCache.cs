using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Domain.Interfaces.Service;

public interface IBacenCache
{
    Task<IReadOnlyList<BacenDomain>?> obterAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, CancellationToken cancellationToken = default);
    Task salvarAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, IReadOnlyList<BacenDomain> registros, CancellationToken cancellationToken = default);
}