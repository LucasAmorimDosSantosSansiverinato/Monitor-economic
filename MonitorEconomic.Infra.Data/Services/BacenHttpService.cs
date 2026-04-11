using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Domain.Exceptions;
using MonitorEconomic.Domain.Interfaces.Service;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Globalization;

namespace MonitorEconomic.Infra.Data.Services;

public class BacenHttpService : IBacenService
{
    private readonly HttpClient _httpClient;
    private readonly BacenApiOptions _bacenApiOptions;
    private readonly IReadOnlyDictionary<BacenSerie, IBacenSerieStrategy> _serieStrategies;

    public BacenHttpService(HttpClient httpClient, IOptions<BacenApiOptions> bacenApiOptions, IEnumerable<IBacenSerieStrategy> serieStrategies)
    {
        _httpClient = httpClient;
        _bacenApiOptions = bacenApiOptions.Value;
        _serieStrategies = serieStrategies.ToDictionary(strategy => strategy.Serie);
    }

    public async Task<List<BacenDomain>> obterBacenAsync(BacenSerie serie, string dataInicial, string dataFinal, CancellationToken cancellationToken = default)
    {
        if (!DateTime.TryParseExact(dataInicial, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dataInicialParsed))
            throw new ArgumentException("data Inicial deve estar com formato em dd/MM/yyyy", nameof(dataInicial));

        if (!DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dataFinalParsed))
            throw new ArgumentException("data Final deve estar com formato em dd/MM/yyyy", nameof(dataFinal));

        if (string.IsNullOrWhiteSpace(_bacenApiOptions.SeriesUrlTemplate))
            throw new InvalidOperationException("A configuração BacenApi:SeriesUrlTemplate não foi informada.");

        var dataInicialFormatada = dataInicialParsed.ToString("dd/MM/yyyy");
        var dataFinalFormatada = dataFinalParsed.ToString("dd/MM/yyyy");
        var codigoSerie = ObterCodigoSerie(serie);

        var url = _bacenApiOptions.SeriesUrlTemplate
            .Replace("{codigo}", codigoSerie.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{dataInicial}", dataInicialFormatada, StringComparison.Ordinal)
            .Replace("{dataFinal}", dataFinalFormatada, StringComparison.Ordinal);

        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BacenApiItem>>(url, cancellationToken) ?? new List<BacenApiItem>();

            return response
                .Select(item => new BacenDomain(
                    serie,
                    ParseData(item.data),
                    ParseValor(item.valor)
                ))
                .ToList();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BacenIntegrationException($"Falha ao consultar o Bacen para a serie {serie}.", ex);
        }
    }

    private int ObterCodigoSerie(BacenSerie serie)
    {
        if (_serieStrategies.TryGetValue(serie, out var strategy))
            return strategy.Codigo;

        throw new InvalidOperationException($"Nao existe strategy configurada para a serie {serie}.");
    }

    private static DateTime ParseData(string data)
    {
        if (DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataConvertida))
        {
            return dataConvertida;
        }

        throw new FormatException($"Data retornada pelo Bacen em formato inválido: {data}");
    }

    private static decimal ParseValor(string valor)
    {
        if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out var valorConvertido))
        {
            return valorConvertido;
        }

        throw new FormatException($"Valor retornado pelo Bacen em formato inválido: {valor}");
    }
}