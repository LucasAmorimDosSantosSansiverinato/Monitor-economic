using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Application.Bacen.Parsing;

public static class BacenSerieParser
{
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    public static BacenSerie Parse(string? value)
    {
        if (TryParse(value, out var serie))
        {
            return serie;
        }

        throw new ArgumentException("O parâmetro serie é inválido.", nameof(value));
    }

    private static bool TryParse(string? value, out BacenSerie serie)
    {
        serie = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (Enum.TryParse<BacenSerie>(value, true, out var serieParseada) && Enum.IsDefined(serieParseada))
        {
            serie = serieParseada;
            return true;
        }

        return int.TryParse(value, out var serieNumerica)
            && Enum.IsDefined(typeof(BacenSerie), serieNumerica)
            && TryConverter(serieNumerica, out serie);
    }

    private static bool TryConverter(int serieNumerica, out BacenSerie serie)
    {
        serie = (BacenSerie)serieNumerica;
        return true;
    }
}