using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Application.UseCases;
using MonitorEconomic.Domain.Interfaces;
using MonitorEconomic.Infra.Data.Clients;
using MonitorEconomic.Infra.Data.Repository;
using MonitorEconomic.Infra.Data.Services;

namespace MonitorEconomic.Infra.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<SupabaseClientFactory>();
        services.AddScoped<IIPCRepository, IPCRepository>();

        // UseCase
        services.AddScoped<ObterIPCUseCase>();

        // ✅ Serviço HTTP usando HttpClientFactory
        services.AddHttpClient<IIPCService, IPCService>();

        return services;
    }
}