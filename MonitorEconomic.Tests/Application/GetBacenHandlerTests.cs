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
    public async Task Handle_BancoCoberturaTotal_NaoConsultaBacen()
    {
        // DB cobre o range completo (primeiro = dataInicial, último = dataFinal) → Bacen não é chamado
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>(MockBehavior.Strict);
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registrosBanco = new List<BacenDomain>
        {
            new(BacenSerie.Dolar, new DateTime(2024, 1, 1), 5.00m),
            new(BacenSerie.Dolar, new DateTime(2024, 1, 31), 5.10m)
        };

        cache
            .Setup(c => c.obterAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Dolar, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBanco);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Dolar, It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 2), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, resultado.Count);
        bacenService.VerifyNoOtherCalls(); // Bacen não deve ser chamado
        cache.VerifyAll();
    }

    [Fact]
    public async Task Handle_BancoVazio_ConsultaBacenRangeCompleto()
    {
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registrosBacen = CriarRegistros(BacenSerie.Euro, new DateTime(2024, 1, 10));

        cache
            .Setup(c => c.obterAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BacenDomain>());

        bacenService
            .Setup(s => s.obterBacenAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBacen);

        repository
            .Setup(r => r.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Euro, It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 1), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Single(resultado);
        bacenService.Verify(s => s.obterBacenAsync(BacenSerie.Euro, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BancoParcialNoFinal_ConsultaBacenSomenteLacunaFinal()
    {
        // DB tem dados de 2024-01-01 a 2024-01-15, usuário pede até 2024-01-31
        // Bacen deve ser consultado apenas de 2024-01-16 até 2024-01-31
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registrosBanco = new List<BacenDomain>
        {
            new(BacenSerie.Selic, new DateTime(2024, 1, 1), 10.5m),
            new(BacenSerie.Selic, new DateTime(2024, 1, 15), 10.5m)
        };
        var registrosBacen = CriarRegistros(BacenSerie.Selic, new DateTime(2024, 1, 20));

        cache
            .Setup(c => c.obterAsync(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBanco);

        // Bacen consultado apenas para a lacuna final: 2024-01-16 a 2024-01-31
        bacenService
            .Setup(s => s.obterBacenAsync(BacenSerie.Selic, new DateTime(2024, 1, 16), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBacen);

        repository
            .Setup(r => r.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Selic, It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 3), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(3, resultado.Count);
        bacenService.VerifyAll();
        // Garante que Bacen não foi chamado para o range completo
        bacenService.Verify(s => s.obterBacenAsync(BacenSerie.Selic, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()), Times.Never);
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
        bacenService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ReturnsDatabaseDataWhenBacenIsUnavailable()
    {
        // DB tem dados mas Bacen está fora — deve retornar o que tem no banco
        var cache = new Mock<IBacenCache>();
        var repository = new Mock<IBacenRepository>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var query = new GetBacenQuery(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        // DB tem dados mas não cobre o final — vai tentar Bacen e falhar
        var registrosBanco = new List<BacenDomain>
        {
            new(BacenSerie.Ipc, new DateTime(2024, 1, 1), 0.42m),
            new(BacenSerie.Ipc, new DateTime(2024, 1, 15), 0.42m)
        };

        cache
            .Setup(c => c.obterAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<BacenDomain>?)null);

        repository
            .Setup(r => r.obterPorPeriodoAsync(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrosBanco);

        bacenService
            .Setup(s => s.obterBacenAsync(BacenSerie.Ipc, new DateTime(2024, 1, 16), new DateTime(2024, 1, 31), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BacenIntegrationException("Bacen indisponível"));

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Ipc, It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 2), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var resultado = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, resultado.Count);
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