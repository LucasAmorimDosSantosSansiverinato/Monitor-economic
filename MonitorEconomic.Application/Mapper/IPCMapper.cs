using AutoMapper;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Application.Mapper;

public class IPCMappingProfile : Profile
{
    public IPCMappingProfile()
    {
        CreateMap<IPCBaseDomain, ItemIPCDto>()
            .ForMember(dest => dest.data, opt => opt.MapFrom(src => src.Data.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.valor, opt => opt.MapFrom(src => src.Valor.ToString("F2")));

        CreateMap<ItemIPCDto, IPCBaseDomain>()
            .ConstructUsing(dto => new IPCBaseDomain(
                DateTime.Parse(dto.data),
                decimal.Parse(dto.valor)
            ));
    }
}