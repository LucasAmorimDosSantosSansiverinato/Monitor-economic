using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Infra.Utils.Bacen.Abstractions;

namespace MonitorEconomic.Infra.Data.Bacen.Strategies;

public class DolarBacenSerieStrategy : IBacenSerieStrategy
{
    public BacenSerie Serie => BacenSerie.Dolar;

    public int Codigo => 21619;
}