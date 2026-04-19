using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Application.Mediator.Bacen.Commands;

public class RefreshBacenCommand : IRequest<List<BacenDto>>
{
    public RefreshBacenCommand(BacenSerie serie, DateTime dataInicial, DateTime dataFinal)
    {
        Serie = serie;
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }

    public BacenSerie Serie { get; set; }
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
}   