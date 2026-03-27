using AutoMapper;
using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.IPC.Queries;
using MonitorEconomic.Domain.Interfaces.IRepository;

public class GetAllIPCHandler : IRequestHandler<GetAllIPCQuery, List<IPCDto>>
{
    private readonly IIPCRepository _ipcRepository;
    private readonly IMapper _mapper;
    public GetAllIPCHandler(IIPCRepository ipcRepository, IMapper mapper)
    {
        _ipcRepository = ipcRepository;
        _mapper = mapper;
    }

    public async Task<List<IPCDto>> Handle(GetAllIPCQuery request, CancellationToken cancellationToken)
    {
        var registros = await _ipcRepository.obterTodosAsync();
        return _mapper.Map<List<IPCDto>>(registros);
    }
}