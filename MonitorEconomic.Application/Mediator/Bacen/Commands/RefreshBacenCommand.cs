using MediatR;
using MonitorEconomic.Application.Dto;

namespace MonitorEconomic.Application.Mediator.Bacen.Commands;

public class RefreshBacenCommand : IRequest<List<BacenDto>>
{
    public RefreshBacenCommand(string serie, string dataInicial, string dataFinal)
    {
        SerieTexto = serie;
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }

    public string SerieTexto { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }
}   