using Microsoft.OpenApi.Models;
using MonitorEconomic.WebUi.Swagger;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MonitorEconomic API",
                Version = "v1",
                Description = "API para consultar dados de IPC na fonte externa, persistir registros localmente e consultar o historico salvo no banco.",
                Contact = new OpenApiContact
                {
                    Name = "MonitorEconomic"
                }
            });

            c.OperationFilter<IpcDateQueryOperationFilter>();
        });
        return services;
    }
}