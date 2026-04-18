namespace MonitorEconomic.Infra.Data.Bacen.Configuration;

public class BacenApiOptions
{
    public const string SectionName = "BacenApi";

    public string SeriesUrlTemplate { get; set; } = string.Empty;
}