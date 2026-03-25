using MediatR;
using MonitorEconomic.Application.Dto;

namespace MonitorEconomic.Application.Mediator.IPC.Queries;

public class GetIPCQuery : IRequest<List<ItemIPCDto>>
{
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }

    public GetIPCQuery(string dataInicial, string dataFinal)
    {
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }
}