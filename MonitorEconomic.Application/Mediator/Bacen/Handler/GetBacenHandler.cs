using MediatR;
using MonitorEconomic.Application.Cache;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Exceptions;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Interfaces.Service;
using AutoMapper;

namespace MonitorEconomic.Application.Mediator.Bacen.Handler;

public class GetBacenHandler : IRequestHandler<GetBacenQuery, List<BacenDto>>
{
    private readonly IBacenRepository _bacenRepository;
    private readonly IBacenCache _bacenCache;
    private readonly IBacenService _bacenService;
    private readonly IMapper _mapper;

    public GetBacenHandler(IBacenRepository bacenRepository, IBacenCache bacenCache, IBacenService bacenService, IMapper mapper)
    {
        _bacenRepository = bacenRepository;
        _bacenCache = bacenCache;
        _bacenService = bacenService;
        _mapper = mapper;
    }

    public async Task<List<BacenDto>> Handle(GetBacenQuery request, CancellationToken cancellationToken)
    {
        var registrosEmCache = await _bacenCache.obterAsync(request.Serie, request.DataInicial, request.DataFinal, cancellationToken);
        if (registrosEmCache is { Count: > 0 })
        {
            return _mapper.Map<List<BacenDto>>(registrosEmCache);
        }

        var registrosNoBanco = await _bacenRepository.obterPorPeriodoAsync(request.Serie, request.DataInicial, request.DataFinal, cancellationToken);

        var primeiraDataNoBanco = registrosNoBanco.FirstOrDefault()?.Data.Date;
        var ultimaDataNoBanco = registrosNoBanco.LastOrDefault()?.Data.Date;

        List<BacenDomain> registrosDoBacen = new();
        try
        {
            if (primeiraDataNoBanco is null)
            {
                registrosDoBacen = await _bacenService.obterBacenAsync(request.Serie, request.DataInicial, request.DataFinal, cancellationToken);
            }
            else
            {
                if (request.DataInicial.Date < primeiraDataNoBanco.Value)
                {
                    var antes = await _bacenService.obterBacenAsync(request.Serie, request.DataInicial, primeiraDataNoBanco.Value.AddDays(-1), cancellationToken);
                    registrosDoBacen.AddRange(antes);
                }

                if (request.DataFinal.Date > ultimaDataNoBanco!.Value)
                {
                    var depois = await _bacenService.obterBacenAsync(request.Serie, ultimaDataNoBanco.Value.AddDays(1), request.DataFinal, cancellationToken);
                    registrosDoBacen.AddRange(depois);
                }
            }

            foreach (var registro in registrosDoBacen)
            {
                await _bacenRepository.salvarAsync(registro, cancellationToken);
            }
        }
        catch (BacenIntegrationException)
        {
            // Bacen indisponível — retorna o que existe no banco
        }

        var datasExistentes = registrosNoBanco.Select(r => r.Data.Date).ToHashSet();
        var novosRegistros = registrosDoBacen.Where(r => !datasExistentes.Contains(r.Data.Date));
        var merged = registrosNoBanco.Concat(novosRegistros).OrderBy(r => r.Data).ToList();

        if (merged.Count > 0)
        {
            await _bacenCache.salvarAsync(request.Serie, merged, cancellationToken);
        }

        return _mapper.Map<List<BacenDto>>(merged);
    }
}