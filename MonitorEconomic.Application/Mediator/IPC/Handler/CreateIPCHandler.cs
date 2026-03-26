using MediatR;
using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Application.Mediator.IPC.Commands;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Interfaces.IRepository;
using AutoMapper;

namespace MonitorEconomic.Application.Mediator.IPC.Handler;

public class CreateIPCHandler : IRequestHandler<CreateIPCCommand, List<IPCBaseDomain>>
{
    private readonly IIPCService _ipcService;
    private readonly IIPCRepository _ipcRepository;
    private readonly IMapper _mapper;

    public CreateIPCHandler(IIPCService ipcService, IIPCRepository ipcRepository, IMapper mapper)
    {
        _ipcService = ipcService;
        _ipcRepository = ipcRepository;
        _mapper = mapper;
    }

    public async Task<List<IPCBaseDomain>> Handle(CreateIPCCommand request, CancellationToken cancellationToken)
    {
        var dtos = await _ipcService.obterIPCAsync(request.DataInicial, request.DataFinal);
        
        if (dtos == null)
        {
            Console.WriteLine("Nenhum dado retornado da API IPC");
            return new();
        }

        Console.WriteLine($"Quantidade de registros retornados: {dtos.Count}");

        foreach (var dto in dtos)
        {
            Console.WriteLine($"Data: {dto.data}, Valor: {dto.valor}");
        }

        var listaModels = _mapper.Map<List<IPCBaseDomain>>(dtos);

        foreach (var model in listaModels)
        {
            await _ipcRepository.salvarAsync(model);
        }

        return listaModels;
    }
}
