using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Exceptions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MonitorEconomic.Tests.Web;

public class BacenApiIntegrationTests
{
    [Fact]
    public async Task GetBacen_WithoutSerie_ReturnsBadRequest()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/bacen?dataInicial=01/01/2024&dataFinal=31/01/2024");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBacen_WhenMediatorThrowsDomainException_ReturnsProblemDetails()
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<GetBacenQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DomainException("Data inválida"));

        using var factory = CreateFactory(mediator.Object);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/bacen?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Violação de regra de domínio", problem!.Title);
        Assert.Equal("Data inválida", problem.Detail);
    }

    [Fact]
    public async Task GetBacen_WithValidRequest_ReturnsDtos()
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<GetBacenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BacenDto> { new() { data = "2024-01-01", valor = "0.65" } });

        using var factory = CreateFactory(mediator.Object);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/bacen?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024");
        var payload = await response.Content.ReadFromJsonAsync<List<BacenDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Single(payload!);
        Assert.Equal("2024-01-01", payload[0].data);
    }

    private static WebApplicationFactory<Program> CreateFactory(IMediator? mediator = null)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    if (mediator is null)
                    {
                        return;
                    }

                    services.RemoveAll<IMediator>();
                    services.AddSingleton(mediator);
                });
            });
    }
}