using MediatR;
using MonitorEconomic.Application.Dto;

public class GetIPCQuery : IRequest<List<ItemIPCDto>>{}