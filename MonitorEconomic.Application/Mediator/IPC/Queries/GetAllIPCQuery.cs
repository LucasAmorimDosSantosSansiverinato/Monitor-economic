using MediatR;
using MonitorEconomic.Application.Dto;

namespace MonitorEconomic.Application.Mediator.IPC.Queries;
public class GetAllIPCQuery : IRequest<List<IPCDto>>{}
