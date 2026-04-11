using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MonitorEconomic.WebUi.Swagger;

public class BacenDateQueryOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var relativePath = context.ApiDescription.RelativePath ?? string.Empty;
        var httpMethod = context.ApiDescription.HttpMethod ?? string.Empty;

        if (httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) && relativePath.Equals("api/bacen", StringComparison.OrdinalIgnoreCase))
        {
            operation.Summary = "Consulta dados do Bacen na API externa";
            operation.Description = "Consulta dados do Bacen em um intervalo de datas na API externa do Banco Central.";
        }

        if (httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && relativePath.Equals("api/bacen/store", StringComparison.OrdinalIgnoreCase))
        {
            operation.Summary = "Consulta e persiste dados do Bacen";
            operation.Description = "Busca dados do Bacen na API externa e persiste os registros retornados no banco de dados.";
        }

        if (httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) && relativePath.Equals("api/bacen/db", StringComparison.OrdinalIgnoreCase))
        {
            operation.Summary = "Lista registros do Bacen persistidos";
            operation.Description = "Lista todos os registros do Bacen já armazenados no banco de dados.";
        }

        foreach (var parameter in operation.Parameters)
        {
            if (parameter.Name.Equals("dataInicial", StringComparison.OrdinalIgnoreCase))
            {
                parameter.Description = "Informe a data inicial no formato dd/MM/yyyy. Exemplo: 01/01/2024";
                parameter.Example = new OpenApiString("01/01/2024");
            }

            if (parameter.Name.Equals("dataFinal", StringComparison.OrdinalIgnoreCase))
            {
                parameter.Description = "Informe a data final no formato dd/MM/yyyy. Exemplo: 31/01/2024";
                parameter.Example = new OpenApiString("31/01/2024");
            }

            if (parameter.Name.Equals("serie", StringComparison.OrdinalIgnoreCase))
            {
                parameter.Required = true;
                parameter.Description = "Serie do Bacen a ser consultada. Exemplo atual: Ipc";
                parameter.Example = new OpenApiString("Ipc");
            }
        }

        if (operation.Responses.TryGetValue("200", out var okResponse))
        {
            okResponse.Description = "Requisição processada com sucesso.";
        }

        if (operation.Responses.TryGetValue("400", out var badRequestResponse))
        {
            badRequestResponse.Description = "Parâmetros inválidos ou falha ao consultar/persistir dados.";
        }

        if (operation.Responses.TryGetValue("404", out var notFoundResponse))
        {
            notFoundResponse.Description = "Nenhum registro encontrado para a consulta informada.";
        }
    }
}