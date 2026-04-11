using AutoMapper;
using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Interfaces.IRepository;

namespace MonitorEconomic.Application.Mediator.Bacen.Handler;

public class GetAllBacenHandler : IRequestHandler<GetAllBacenQuery, List<BacenDto>>
{
    private readonly IBacenRepository _bacenRepository;
    private readonly IMapper _mapper;
    public GetAllBacenHandler(IBacenRepository bacenRepository, IMapper mapper)
    {
        _bacenRepository = bacenRepository;
        _mapper = mapper;
    }

    public async Task<List<BacenDto>> Handle(GetAllBacenQuery request, CancellationToken cancellationToken)
    {
        var registros = await _bacenRepository.obterTodosAsync(cancellationToken);
        return _mapper.Map<List<BacenDto>>(registros);
    }
}