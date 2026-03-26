using MediatR;
using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.IPC.Queries;

namespace MonitorEconomic.Application.Mediator.IPC.Handler;

public class GetIPCHandler : IRequestHandler<GetIPCQuery, List<IPCDto>>
{
    private readonly IIPCService _ipcService;

    public GetIPCHandler(IIPCService ipcService)
    {
        _ipcService = ipcService;
    }

    public async Task<List<IPCDto>> Handle(GetIPCQuery request, CancellationToken cancellationToken)
    {
        return await _ipcService.obterIPCAsync(request.DataInicial, request.DataFinal) ?? new List<IPCDto>();
    }
}