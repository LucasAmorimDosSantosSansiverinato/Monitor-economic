using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Infra.Data.Bacen.Abstractions;

namespace MonitorEconomic.Infra.Data.Bacen.Strategies;

public class SelicBacenSerieStrategy : IBacenSerieStrategy
{
    public BacenSerie Serie => BacenSerie.Selic;

    public int Codigo => 433;
}