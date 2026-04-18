using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Bacen.Parsing;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Application.Mediator.Bacen.Queries;

public class GetBacenQuery : IRequest<List<BacenDto>>
{
    public BacenSerie Serie { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }

    public GetBacenQuery(string serie, string dataInicial, string dataFinal)
    {
        Serie = BacenSerieParser.Parse(serie);
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }
}