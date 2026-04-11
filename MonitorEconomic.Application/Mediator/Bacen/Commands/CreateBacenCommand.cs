using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Application.Mediator.Bacen.Commands;

public class CreateBacenCommand : IRequest<List<BacenDto>>
{
    public BacenSerie Serie { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }

    public CreateBacenCommand(BacenSerie serie, string dataInicial, string dataFinal)
    {
        Serie = serie;
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }
}