using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Application.UseCases;
using MonitorEconomic.Infra.Data.Repository;
using MonitorEconomic.Infra.Data.Services;
using MonitorEconomic.Application.Mediator.IPC.Handler;
using MonitorEconomic.Application.Mapper;
using MediatR;

namespace MonitorEconomic.Infra.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
    
        services.AddTransient<IIPCRepository, IPCRepository>();

        services.AddTransient<ObterIPCUseCase>();

        services.AddHttpClient<IIPCService, IPCService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetIPCHandler).Assembly));

        services.AddAutoMapper(typeof(IPCMappingProfile).Assembly);

        return services;
    }
}