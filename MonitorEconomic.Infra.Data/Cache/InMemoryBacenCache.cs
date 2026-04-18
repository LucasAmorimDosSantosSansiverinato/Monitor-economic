using System.Collections.Concurrent;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Abstractions.Cache;

namespace MonitorEconomic.Infra.Data.Cache;

public class InMemoryBacenCache : IBacenCache
{
    private static readonly ConcurrentDictionary<string, CacheEntry> Entries = new(StringComparer.Ordinal);
    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    private static string? _ultimoMesLimpo;

    public async Task<IReadOnlyList<BacenDomain>?> obterAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, CancellationToken cancellationToken = default)
    {
        await LimparSeNecessarioAsync(cancellationToken);

        var key = CriarChave(serie, dataInicial, dataFinal);
        if (!Entries.TryGetValue(key, out var entry))
        {
            return null;
        }

        if (entry.ExpiraEmUtc <= DateTime.UtcNow)
        {
            Entries.TryRemove(key, out _);
            return null;
        }

        return entry.Registros;
    }

    public async Task salvarAsync(BacenSerie serie, DateTime dataInicial, DateTime dataFinal, IReadOnlyList<BacenDomain> registros, CancellationToken cancellationToken = default)
    {
        await LimparSeNecessarioAsync(cancellationToken);

        var agora = DateTime.UtcNow;
        var expiraEmUtc = CalcularExpiracao(serie, agora);
        var key = CriarChave(serie, dataInicial, dataFinal);
        Entries[key] = new CacheEntry(registros.ToList(), expiraEmUtc);
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
                {
                    Entries.TryRemove(entry.Key, out _);
                }
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
        {
            return agoraUtc.Date.AddDays(1);
        }

        var quinzeDias = agoraUtc.AddDays(15);
        var primeiroDiaDoProximoMes = new DateTime(agoraUtc.Year, agoraUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);

        return quinzeDias <= primeiroDiaDoProximoMes ? quinzeDias : primeiroDiaDoProximoMes;
    }

    private static string CriarChave(BacenSerie serie, DateTime dataInicial, DateTime dataFinal)
    {
        return string.Join(':', serie, dataInicial.ToString("yyyyMMdd"), dataFinal.ToString("yyyyMMdd"));
    }

    private sealed record CacheEntry(IReadOnlyList<BacenDomain> Registros, DateTime ExpiraEmUtc);
}