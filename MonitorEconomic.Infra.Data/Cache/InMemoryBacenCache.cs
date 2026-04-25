using System.Collections.Concurrent;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Infra.Data.Cache;

public class InMemoryBacenCache
{
    private static readonly ConcurrentDictionary<BacenSerie, CacheEntry> Entries = new();
    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    private static string? _ultimoMesLimpo;

    public async Task<IReadOnlyList<BacenDomain>?> obterAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, CancellationToken cancellationToken = default)
    {
        await LimparSeNecessarioAsync(cancellationToken);

        if (!Entries.TryGetValue(serie, out var entry))
            return null;

        if (entry.ExpiraEmUtc <= DateTime.UtcNow)
        {
            Entries.TryRemove(serie, out _);
            return null;
        }

        var filtrado = entry.Registros
            .Where(r => r.Data.Date >= dataInicial.Date && r.Data.Date <= dataFinal.Date)
            .ToList();

        return filtrado.Count > 0 ? filtrado : null;
    }

    public async Task salvarAsync(BacenSerie serie, IReadOnlyList<BacenDomain> registros, CancellationToken cancellationToken = default)
    {
        await LimparSeNecessarioAsync(cancellationToken);

        var agora = DateTime.UtcNow;
        var expiraEmUtc = CalcularExpiracao(serie, agora);

        Entries.AddOrUpdate(
            serie,
            _ => new CacheEntry(registros.ToList(), expiraEmUtc),
            (_, existente) =>
            {
                var datasExistentes = existente.Registros.Select(r => r.Data.Date).ToHashSet();
                var novos = registros.Where(r => !datasExistentes.Contains(r.Data.Date));
                var merged = existente.Registros.Concat(novos).OrderBy(r => r.Data).ToList();
                return new CacheEntry(merged, expiraEmUtc);
            });
    }

    private static async Task LimparSeNecessarioAsync(CancellationToken cancellationToken)
    {
        await CacheLock.WaitAsync(cancellationToken);

        try
        {
            var agora = DateTime.UtcNow;
            var mesAtual = agora.ToString("yyyy-MM");

            if (agora.Day == 1 && !string.Equals(_ultimoMesLimpo, mesAtual, StringComparison.Ordinal))
            {
                Entries.Clear();
                _ultimoMesLimpo = mesAtual;
            }

            foreach (var entry in Entries)
            {
                if (entry.Value.ExpiraEmUtc <= agora)
                    Entries.TryRemove(entry.Key, out _);
            }
        }
        finally
        {
            CacheLock.Release();
        }
    }

    private static DateTime CalcularExpiracao(BacenSerie serie, DateTime agoraUtc)
    {
        if (serie is BacenSerie.Dolar or BacenSerie.Euro)
            return agoraUtc.Date.AddDays(1);

        var quinzeDias = agoraUtc.AddDays(15);
        var primeiroDiaDoProximoMes = new DateTime(agoraUtc.Year, agoraUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);

        return quinzeDias <= primeiroDiaDoProximoMes ? quinzeDias : primeiroDiaDoProximoMes;
    }

    private sealed record CacheEntry(IReadOnlyList<BacenDomain> Registros, DateTime ExpiraEmUtc);
}