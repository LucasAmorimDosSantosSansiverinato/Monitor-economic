using MediatR;
using AutoMapper;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Commands;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Interfaces.Service;
using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Application.Mediator.Bacen.Handler;

public class CreateBacenHandler : IRequestHandler<CreateBacenCommand, List<BacenDto>>
{
    private readonly IBacenService _bacenService;
    private readonly IBacenRepository _bacenRepository;
    private readonly IMapper _mapper;

    public CreateBacenHandler(IBacenService bacenService, IBacenRepository bacenRepository, IMapper mapper)
    {
        _bacenService = bacenService;
        _bacenRepository = bacenRepository;
        _mapper = mapper;
    }

    public async Task<List<BacenDto>> Handle(CreateBacenCommand request, CancellationToken cancellationToken)
    {
        List<BacenDomain> listaModels = await _bacenService.obterBacenAsync(request.Serie, request.DataInicial, request.DataFinal, cancellationToken);

        foreach (var model in listaModels)
        {
            await _bacenRepository.salvarAsync(model, cancellationToken);
        }

        return _mapper.Map<List<BacenDto>>(listaModels);
    }
}
