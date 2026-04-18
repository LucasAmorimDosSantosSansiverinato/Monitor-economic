using AutoMapper;
using Moq;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Handler;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Interfaces.Service;
using Xunit;

namespace MonitorEconomic.Tests.Application;

public class GetBacenHandlerTests
{
    [Fact]
    public async Task Handle_UsesCacheBeforeDatabaseAndExternalService()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>(MockBehavior.Strict);
        var service = new Mock<IBacenService>(MockBehavior.Strict);
        var mapper = CriarMapper();
        var query = new GetBacenQuery("Ipc", "01/01/2024", "31/01/2024");
        var registros = CriarRegistros(BacenSerie.Ipc);

        cache
            .Setup(c => c.obterAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registros);

        var handler = new GetBacenHandler(repository.Object, cache.Object, service.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Single(resultado);
        cache.VerifyAll();
        repository.VerifyNoOtherCalls();
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_UsesDatabaseWhenCacheMisses_AndRefreshesCache()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var service = new Mock<IBacenService>(MockBehavior.Strict);
        var mapper = CriarMapper();
        var query = new GetBacenQuery("Dolar", "01/01/2024", "31/01/2024");
        var registros = CriarRegistros(BacenSerie.Dolar);

        cache
            .Setup(c => c.obterAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registros);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 1), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, service.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Single(resultado);
        repository.VerifyAll();
        cache.VerifyAll();
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_UsesExternalServiceWhenCacheAndDatabaseMiss_AndPersistsResults()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var service = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery("Euro", "01/01/2024", "31/01/2024");
        var registros = CriarRegistros(BacenSerie.Euro);

        cache
            .Setup(c => c.obterAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BacenDomain>());

        service
            .Setup(s => s.obterBacenAsync(BacenSerie.Euro, "01/01/2024", "31/01/2024", It.IsAny<CancellationToken>()))
            .ReturnsAsync(registros);

        repository
            .Setup(r => r.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 1), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, service.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Single(resultado);
        service.VerifyAll();
        repository.Verify(r => r.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()), Times.Exactly(registros.Count));
        cache.VerifyAll();
    }

    private static Mock<IMapper> CriarMapper()
    {
        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<List<BacenDto>>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var registros = (IEnumerable<BacenDomain>)source;
                return registros.Select(item => new BacenDto
                {
                    data = item.Data.ToString("yyyy-MM-dd"),
                    valor = item.Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                }).ToList();
            });

        return mapper;
    }

    private static List<BacenDomain> CriarRegistros(BacenSerie serie)
    {
        return new List<BacenDomain>
        {
            new(serie, new DateTime(2024, 1, 2), 1.23m)
        };
    }
}