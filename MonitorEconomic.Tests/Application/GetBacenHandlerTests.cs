using AutoMapper;
using Moq;
using MonitorEconomic.Application.Cache;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Handler;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Domain.Exceptions;
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
        var bacenService = new Mock<IBacenService>(MockBehavior.Strict);
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registros = CriarRegistros(BacenSerie.Ipc, new DateTime(2024, 1, 2));

        cache
            .Setup(c => c.obterAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registros);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Single(resultado);
        cache.VerifyAll();
        repository.VerifyNoOtherCalls();
        bacenService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_UsesDatabaseWhenCacheMisses_ComplementsWithBacenAndRefreshesCache()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registrosBanco = CriarRegistros(BacenSerie.Dolar, new DateTime(2024, 1, 2));

        cache
            .Setup(c => c.obterAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBanco);

        bacenService
            .Setup(s => s.obterBacenAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BacenDomain>());

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 1), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Single(resultado);
        repository.VerifyAll();
        cache.VerifyAll();
        bacenService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ReturnsEmptyWhenCacheAndDatabaseAndBacenMiss()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        cache
            .Setup(c => c.obterAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BacenDomain>());

        bacenService
            .Setup(s => s.obterBacenAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BacenDomain>());

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Empty(resultado);
        repository.VerifyAll();
        bacenService.VerifyAll();
    }

    [Fact]
    public async Task Handle_MergesDatabaseAndBacenResultsByDate()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registrosBanco = CriarRegistros(BacenSerie.Selic, new DateTime(2024, 1, 2));
        var registrosBacen = CriarRegistros(BacenSerie.Selic, new DateTime(2024, 1, 3));

        cache
            .Setup(c => c.obterAsync(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBanco);

        bacenService
            .Setup(s => s.obterBacenAsync(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBacen);

        repository
            .Setup(r => r.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 2), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, resultado.Count);
        repository.Verify(r => r.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()), Times.Once);
        cache.VerifyAll();
    }

    [Fact]
    public async Task Handle_ReturnsDatabaseDataWhenBacenIsUnavailable()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registrosBanco = CriarRegistros(BacenSerie.Ipc, new DateTime(2024, 1, 2));

        cache
            .Setup(c => c.obterAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBanco);

        bacenService
            .Setup(s => s.obterBacenAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BacenIntegrationException("Bacen indisponível"));

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 1), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Single(resultado);
        repository.Verify(r => r.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()), Times.Never);
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

    private static List<BacenDomain> CriarRegistros(BacenSerie serie, DateTime data)
    {
        return new List<BacenDomain>
        {
            new(serie, data, 1.23m)
        };
    }
}