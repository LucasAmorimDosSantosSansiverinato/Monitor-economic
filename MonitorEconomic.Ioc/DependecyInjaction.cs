using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Infra.Data.Repository;
using MonitorEconomic.Application.Services;
using MonitorEconomic.Application.Mediator.IPC.Handler;
using MonitorEconomic.Application.Mapper;
using MonitorEconomic.Infrastructure.Data.Context;

namespace MonitorEconomic.Infra.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetIPCHandler).Assembly));
        services.AddAutoMapper(typeof(IPCMappingProfile).Assembly);
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string is null");
            return new MonitorEconomicDbContext(connectionString);
        });
    
        services.AddTransient<IIPCRepository, IPCRepository>();   
        services.AddHttpClient<IIPCService, IPCService>(); 
        return services;
    }
}