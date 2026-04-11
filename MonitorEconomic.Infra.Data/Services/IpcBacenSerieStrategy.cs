using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Infra.Data.Services;

public class IpcBacenSerieStrategy : IBacenSerieStrategy
{
    public BacenSerie Serie => BacenSerie.Ipc;

    public int Codigo => 7463;
}