using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Domain.Interfaces.Service;

public interface IBacenService
{
    Task<List<BacenDomain>> obterBacenAsync(BacenSerie serie, string dataInicial, string dataFinal, CancellationToken cancellationToken = default);
}