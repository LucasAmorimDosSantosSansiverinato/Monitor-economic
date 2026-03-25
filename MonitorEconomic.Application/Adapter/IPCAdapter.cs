using AutoMapper;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Infra.Data.Adapter;

public class IPCAdapter
{
    private readonly IMapper _mapper;

    public IPCAdapter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public IPCBaseDomain ToDomain(ItemIPCDto dto)
    {
        return _mapper.Map<IPCBaseDomain>(dto);
    }

    public ItemIPCDto ToDto(IPCBaseDomain domain)
    {
        return _mapper.Map<ItemIPCDto>(domain);
    }
}