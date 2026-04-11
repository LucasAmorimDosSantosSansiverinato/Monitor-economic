using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Interfaces.Service;
using AutoMapper;
using MonitorEconomic.Domain.Entities;
namespace MonitorEconomic.Application.Mediator.Bacen.Handler;

public class GetBacenHandler : IRequestHandler<GetBacenQuery, List<BacenDto>>
{
    private readonly IBacenService _bacenService;
    private readonly IMapper _mapper;

    public GetBacenHandler(IBacenService bacenService, IMapper mapper)
    {
        _bacenService = bacenService;
        _mapper = mapper;
    }

    public async Task<List<BacenDto>> Handle(GetBacenQuery request, CancellationToken cancellationToken)
    {
        List<BacenDomain> registros = await _bacenService.obterBacenAsync(request.Serie, request.DataInicial, request.DataFinal, cancellationToken);
        return _mapper.Map<List<BacenDto>>(registros);
    }
}