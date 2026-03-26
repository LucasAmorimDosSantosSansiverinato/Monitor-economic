using AutoMapper;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Application.Mapper;

public class IPCMappingProfile : Profile
{
    public IPCMappingProfile()
    {
        CreateMap<IPCDomain, IPCDto>()
            .ForMember(dest => dest.data, opt => opt.MapFrom(src => src.Data.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.valor, opt => opt.MapFrom(src => src.Valor.ToString("F2")));

        CreateMap<IPCDto, IPCDomain>()
            .ConstructUsing(dto => new IPCDomain(
                DateTime.Parse(dto.data),
                decimal.Parse(dto.valor)
            ));
    }
}