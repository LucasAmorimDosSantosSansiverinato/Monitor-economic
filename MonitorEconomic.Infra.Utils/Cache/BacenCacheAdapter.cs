using MonitorEconomic.Application.Cache;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Infra.Data.Cache;

namespace MonitorEconomic.Infra.Utils.Cache;

public sealed class BacenCacheAdapter : IBacenCache
{
    private readonly InMemoryBacenCache _cache;

    public BacenCacheAdapter(InMemoryBacenCache cache)
    {
        _cache = cache;
    }

    public Task<IReadOnlyList<BacenDomain>?> obterAsync(
        BacenSerie serie,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken = default)
    {
        return _cache.obterAsync(serie, dataInicial, dataFinal, cancellationToken);
    }

    public Task salvarAsync(
        BacenSerie serie,
        IReadOnlyList<BacenDomain> registros,
        CancellationToken cancellationToken = default)
    {
        return _cache.salvarAsync(serie, registros, cancellationToken);
    }
}
