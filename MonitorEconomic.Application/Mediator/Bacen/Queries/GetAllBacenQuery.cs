using MediatR;
using MonitorEconomic.Application.Dto;

namespace MonitorEconomic.Application.Mediator.Bacen.Queries;
public class GetAllBacenQuery : IRequest<List<BacenDto>>{}
