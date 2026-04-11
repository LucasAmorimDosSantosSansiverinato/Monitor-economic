using AutoMapper;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Entities;

namespace MonitorEconomic.Application.Mapper;

public class BacenMappingProfile : Profile
{
    public BacenMappingProfile()
    {
        CreateMap<BacenDomain, BacenDto>()
            .ForMember(dest => dest.data, opt => opt.MapFrom(src => src.Data.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.valor, opt => opt.MapFrom(src => src.Valor.ToString("F2")));
    }
}