using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Application.UseCases;
// using MonitorEconomic.Infra.Data.Clients;
using MonitorEconomic.Infra.Data.Repository;
using MonitorEconomic.Infra.Data.Services;

namespace MonitorEconomic.Infra.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
       // services.AddSingleton<SupabaseClientFactory>();

        services.AddScoped<IIPCRepository, IPCRepository>();

        services.AddScoped<ObterIPCUseCase>();

        services.AddHttpClient<IIPCService, IPCService>();

        return services;
    }
}