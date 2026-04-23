using AutoMapper;
using Moq;
using MonitorEconomic.Application.Cache;
using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Mediator.Bacen.Commands;
using MonitorEconomic.Application.Mediator.Bacen.Handler;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Domain.Interfaces.IRepository;
using MonitorEconomic.Domain.Interfaces.Service;
using Xunit;

namespace MonitorEconomic.Tests.Application;

public class RefreshBacenHandlerTests
{
    [Fact]
    public async Task Handle_PersistsFetchedData_RefreshesCache_AndReturnsDtos()
    {
        var repository = new Mock<IBacenRepository>();
        var cache = new Mock<IBacenCache>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var command = new RefreshBacenCommand(BacenSerie.Ipc, new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var registros = CriarRegistros(BacenSerie.Ipc, 2);

        bacenService
            .Setup(service => service.obterBacenAsync(BacenSerie.Ipc, command.DataInicial, command.DataFinal, It.IsAny<CancellationToken>()))
            .ReturnsAsync(registros);

        repository
            .Setup(repo => repo.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Ipc, command.DataInicial, command.DataFinal, It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 2), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RefreshBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(2, result.Count);
        bacenService.VerifyAll();
        repository.Verify(repo => repo.salvarAsync(It.IsAny<BacenDomain>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        cache.VerifyAll();
    }

    [Fact]
    public async Task Handle_WhenServiceReturnsEmptyList_StillRefreshesCacheAndReturnsEmptyDtos()
    {
        var repository = new Mock<IBacenRepository>(MockBehavior.Strict);
        var cache = new Mock<IBacenCache>();
        var bacenService = new Mock<IBacenService>();
        var mapper = CriarMapper();
        var command = new RefreshBacenCommand(BacenSerie.Euro, new DateTime(2024, 2, 1), new DateTime(2024, 2, 29));
        var registros = new List<BacenDomain>();

        bacenService
            .Setup(service => service.obterBacenAsync(BacenSerie.Euro, command.DataInicial, command.DataFinal, It.IsAny<CancellationToken>()))
            .ReturnsAsync(registros);

        cache
            .Setup(c => c.salvarAsync(BacenSerie.Euro, command.DataInicial, command.DataFinal, It.Is<IReadOnlyList<BacenDomain>>(items => items.Count == 0), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RefreshBacenHandler(repository.Object, cache.Object, bacenService.Object, mapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Empty(result);
        bacenService.VerifyAll();
        repository.VerifyNoOtherCalls();
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

    private static List<BacenDomain> CriarRegistros(BacenSerie serie, int quantidade)
    {
        return Enumerable.Range(1, quantidade)
            .Select(indice => new BacenDomain(serie, new DateTime(2024, 1, indice), indice + 0.25m))
            .ToList();
    }
}