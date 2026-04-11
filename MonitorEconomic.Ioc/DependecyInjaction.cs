using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Interfaces.Service;
using MonitorEconomic.Infra.Data.Repository;
using MonitorEconomic.Application.Mediator.Bacen.Handler;
using MonitorEconomic.Application.Mapper;
using MonitorEconomic.Infrastructure.Data.Context;
using MonitorEconomic.Infra.Data.Services;

namespace MonitorEconomic.Infra.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBacenHandler).Assembly));
        services.AddAutoMapper(typeof(BacenMappingProfile).Assembly);
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BacenApiOptions>(configuration.GetSection(BacenApiOptions.SectionName));

        services.AddScoped(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string is null");
            return new MonitorEconomicDbContext(connectionString);
        });
    
        services.AddTransient<IBacenRepository, BacenRepository>();   
        services.AddTransient<IBacenSerieStrategy, IpcBacenSerieStrategy>();
        services.AddHttpClient<IBacenService, BacenHttpService>(); 
        return services;
    }
}