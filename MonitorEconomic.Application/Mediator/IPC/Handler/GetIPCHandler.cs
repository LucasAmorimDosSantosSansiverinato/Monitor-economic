using MediatR;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Application.Dto;

public class GetIPCHandler : IRequestHandler<GetIPCQuery, List<ItemIPCDto>>{
    private readonly IIPCRepository _repository;

    public GetIPCHandler(IIPCRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ItemIPCDto>> Handler(GetIPCQuery request, CancellationToken cancellationToken)
    {
        var dados = await _repository.ObterIPC();

        var lista = new List<ItemIPCDto>();

        foreach (var item in dados)
        {
            var dto = new ItemIPCDto();
            dto.valor = item.valor;
            dto.data = item.data;

            lista.Add(dto);
        }
    } 


}