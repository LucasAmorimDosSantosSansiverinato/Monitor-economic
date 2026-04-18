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
                Description = "API para consultar dados do Bacen com fluxo cache, banco e fonte externa, persistindo os resultados localmente quando necessario.",
                Contact = new OpenApiContact
                {
                    Name = "MonitorEconomic"
                }
            });

            c.OperationFilter<BacenDateQueryOperationFilter>();
        });
        return services;
    }
}