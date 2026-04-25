using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Infra.Data.Bacen.Abstractions;

public interface IBacenSerieStrategy
{
    BacenSerie Serie { get; }
    int Codigo { get; }
}
