using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Infra.Utils.Bacen.Abstractions;

public interface IBacenSerieStrategy
{
    BacenSerie Serie { get; }
    int Codigo { get; }
}