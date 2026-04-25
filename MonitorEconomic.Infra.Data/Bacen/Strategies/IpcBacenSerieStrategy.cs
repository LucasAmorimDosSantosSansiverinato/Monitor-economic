using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Infra.Utils.Bacen.Abstractions;

namespace MonitorEconomic.Infra.Data.Bacen.Strategies;

public class IpcBacenSerieStrategy : IBacenSerieStrategy
{
    public BacenSerie Serie => BacenSerie.Ipc;

    public int Codigo => 7463;
}