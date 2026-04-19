using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Application.Mediator.Bacen.Queries;

public class GetBacenQuery : IRequest<List<BacenDto>>
{
    public BacenSerie Serie { get; set; }
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }

    public GetBacenQuery(BacenSerie serie, DateTime dataInicial, DateTime dataFinal)
    {
        Serie = serie;
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }
}