using MediatR;
using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Application.Mediator.IPC.Commands;

public class CreateIPCCommand : IRequest<List<IPCBaseDomain>>
{
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }

    public CreateIPCCommand(string dataInicial, string dataFinal)
    {
        DataInicial = dataInicial;
        DataFinal = dataFinal;
    }
}