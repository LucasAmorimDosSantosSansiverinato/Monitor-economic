using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Application.Mediator.Bacen.Queries;

public class GetBacenQuery : IRequest<List<BacenDto>>
{
    public BacenSerie Serie { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }

    public GetBacenQuery(BacenSerie serie, string dataInicial, string dataFinal)
    {
        Serie = serie;
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }
}