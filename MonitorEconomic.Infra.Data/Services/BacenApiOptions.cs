namespace MonitorEconomic.Infra.Data.Services;

public class BacenApiOptions
{
    public const string SectionName = "BacenApi";

    public string SeriesUrlTemplate { get; set; } = string.Empty;
}