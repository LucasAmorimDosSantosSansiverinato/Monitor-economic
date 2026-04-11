using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Infra.Data.Services;

public interface IBacenSerieStrategy
{
    BacenSerie Serie { get; }
    int Codigo { get; }
}