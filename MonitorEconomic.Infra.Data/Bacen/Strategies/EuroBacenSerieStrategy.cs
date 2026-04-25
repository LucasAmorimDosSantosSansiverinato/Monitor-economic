using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Infra.Data.Bacen.Abstractions;

namespace MonitorEconomic.Infra.Data.Bacen.Strategies;

public class EuroBacenSerieStrategy : IBacenSerieStrategy
{
    public BacenSerie Serie => BacenSerie.Euro;

    public int Codigo => 21627;
}