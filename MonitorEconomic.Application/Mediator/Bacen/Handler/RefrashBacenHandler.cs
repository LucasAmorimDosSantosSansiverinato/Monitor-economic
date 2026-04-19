using AutoMapper;
using MediatR;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Commands;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Abstractions.Cache;
using MonitorEconomic.Domain.Interfaces.Service;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Application.Bacen.Parsing;

namespace MonitorEconomic.Application.Mediator.Bacen.Handler;

public class RefreshBacenHandler : IRequestHandler<RefreshBacenCommand, List<BacenDto>>
{
    private readonly IBacenRepository _bacenRepository;
    private readonly IBacenCache _bacenCache;
    private readonly IBacenService _bacenService;
    private readonly IMapper _mapper;

    public RefreshBacenHandler(IBacenRepository bacenRepository, IBacenCache bacenCache, IBacenService bacenService, IMapper mapper)
    {
        _bacenRepository = bacenRepository;
        _bacenCache = bacenCache;
        _bacenService = bacenService;
        _mapper = mapper;
    }

    public async Task<List<BacenDto>> Handle(RefreshBacenCommand request, CancellationToken cancellationToken)
    {
        var (dataInicial, dataFinal) = BacenDateRangeParser.Parse(request.DataInicial, request.DataFinal);

        List<BacenDomain> registros = await _bacenService.obterBacenAsync(
            request.Serie,
            request.DataInicial,
            request.DataFinal,
            cancellationToken
        );

        foreach (var registro in registros)
        {
            await _bacenRepository.salvarAsync(registro, cancellationToken);
        }

        await _bacenCache.salvarAsync(request.Serie, dataInicial, dataFinal, registros, cancellationToken);

        return _mapper.Map<List<BacenDto>>(registros);
    }
}
