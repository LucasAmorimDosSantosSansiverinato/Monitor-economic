using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Application.Mediator.Bacen.Commands;

public class RefreshBacenCommand : IRequest<List<BacenDto>>
{
    public RefreshBacenCommand(BacenSerie serie, string dataInicial, string dataFinal)
    {
        Serie = serie;
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }

    public BacenSerie Serie { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }
}   