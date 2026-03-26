using MediatR;
using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Application.Mediator.IPC.Commands;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Interfaces.IRepository;

namespace MonitorEconomic.Application.Mediator.IPC.Handler;

public class CreateIPCHandler : IRequestHandler<CreateIPCCommand, List<IPCBaseDomain>>
{
    private readonly IIPCService _ipcService;
    private readonly IIPCRepository _ipcRepository;

    public CreateIPCHandler(IIPCService ipcService, IIPCRepository ipcRepository)
    {
        _ipcService = ipcService;
        _ipcRepository = ipcRepository;
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

        var listaModels = new List<IPCBaseDomain>();

        foreach (var dto in dtos)
        {
            var data = DateTime.SpecifyKind(DateTime.Parse(dto.data), DateTimeKind.Utc);
            var valor = decimal.Parse(dto.valor, System.Globalization.CultureInfo.InvariantCulture);

            var model = new IPCBaseDomain(data, valor);
            listaModels.Add(model);
            await _ipcRepository.salvarAsync(model); // já vai salvar em UTC
        }

        return listaModels;
    }
}
