using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Domain.Exceptions;
using Xunit;

namespace MonitorEconomic.Tests.Domain;

public class BacenDomainTests
{
    [Fact(DisplayName = "Construtor com ID explícito preserva o ID, série, data e valor informados")]
    public void Constructor_WithExplicitId_PreservesIdAndSerie()
    {
        var id = Guid.NewGuid();
        var data = DateTime.UtcNow.Date;

        var entity = new BacenDomain(id, BacenSerie.Ipc, data, 0.65m);

        Assert.Equal(id, entity.Id);
        Assert.Equal(BacenSerie.Ipc, entity.Serie);
        Assert.Equal(data, entity.Data);
        Assert.Equal(0.65m, entity.Valor);
    }

    [Fact(DisplayName = "Construtor com data futura lança DomainException")]
    public void Constructor_WithFutureDate_ThrowsDomainException()
    {
        var futureDate = DateTime.UtcNow.AddDays(1);

        var action = () => new BacenDomain(BacenSerie.Ipc, futureDate, 0.65m);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Construtor com ID vazio (Guid.Empty) lança DomainException")]
    public void Constructor_WithEmptyId_ThrowsDomainException()
    {
        var action = () => new BacenDomain(Guid.Empty, BacenSerie.Ipc, DateTime.UtcNow.Date, 0.65m);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Construtor com série inválida (valor fora do enum) lança DomainException")]
    public void Constructor_WithInvalidSerie_ThrowsDomainException()
    {
        var action = () => new BacenDomain((BacenSerie)999, DateTime.UtcNow.Date, 0.65m);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Construtor com DateTime.MinValue como data lança DomainException")]
    public void Constructor_WithMinValueDate_ThrowsDomainException()
    {
        var action = () => new BacenDomain(BacenSerie.Ipc, DateTime.MinValue, 0.65m);

        Assert.Throws<DomainException>(action);
    }
}