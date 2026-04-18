namespace MonitorEconomic.Application.Bacen.Parsing;

public static class BacenDateRangeParser
{
    public static (DateTime DataInicial, DateTime DataFinal) Parse(string dataInicial, string dataFinal)
    {
        if (!DateTime.TryParseExact(dataInicial, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dataInicialParsed))
            throw new ArgumentException("data Inicial deve estar com formato em dd/MM/yyyy", nameof(dataInicial));

        if (!DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dataFinalParsed))
            throw new ArgumentException("data Final deve estar com formato em dd/MM/yyyy", nameof(dataFinal));

        if (dataInicialParsed > dataFinalParsed)
            throw new ArgumentException("data Inicial não pode ser maior que data Final", nameof(dataInicial));

        return (dataInicialParsed, dataFinalParsed);
    }
}