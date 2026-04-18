using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Bacen.Parsing;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Abstractions.Cache;
using AutoMapper;

namespace MonitorEconomic.Application.Mediator.Bacen.Handler;

public class GetBacenHandler : IRequestHandler<GetBacenQuery, List<BacenDto>>
{
    private readonly IBacenRepository _bacenRepository;
    private readonly IBacenCache _bacenCache;
    private readonly IMapper _mapper;

    public GetBacenHandler(IBacenRepository bacenRepository, IBacenCache bacenCache, IMapper mapper)
    {
        _bacenRepository = bacenRepository;
        _bacenCache = bacenCache;
        _mapper = mapper;
    }

    public async Task<List<BacenDto>> Handle(GetBacenQuery request, CancellationToken cancellationToken)
    {
        var (dataInicial, dataFinal) = BacenDateRangeParser.Parse(request.DataInicial, request.DataFinal);

        var registrosEmCache = await _bacenCache.obterAsync(request.Serie, dataInicial, dataFinal, cancellationToken);
        if (registrosEmCache is { Count: > 0 })
        {
            return _mapper.Map<List<BacenDto>>(registrosEmCache);
        }

        var registrosNoBanco = await _bacenRepository.obterPorPeriodoAsync(request.Serie, dataInicial, dataFinal, cancellationToken);
        if (registrosNoBanco.Count > 0)
        {
            await _bacenCache.salvarAsync(request.Serie, dataInicial, dataFinal, registrosNoBanco, cancellationToken);
            return _mapper.Map<List<BacenDto>>(registrosNoBanco);
        }
 
        return new List<BacenDto>();
    }
}