using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Enums;
using MonitorEconomic.Domain.Exceptions;
using Xunit;

namespace MonitorEconomic.Tests.Domain;

public class BacenDomainTests
{
    [Fact]
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

    [Fact]
    public void Constructor_WithFutureDate_ThrowsDomainException()
    {
        var futureDate = DateTime.UtcNow.AddDays(1);

        var action = () => new BacenDomain(BacenSerie.Ipc, futureDate, 0.65m);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Constructor_WithEmptyId_ThrowsDomainException()
    {
        var action = () => new BacenDomain(Guid.Empty, BacenSerie.Ipc, DateTime.UtcNow.Date, 0.65m);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Constructor_WithInvalidSerie_ThrowsDomainException()
    {
        var action = () => new BacenDomain((BacenSerie)999, DateTime.UtcNow.Date, 0.65m);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Constructor_WithMinValueDate_ThrowsDomainException()
    {
        var action = () => new BacenDomain(BacenSerie.Ipc, DateTime.MinValue, 0.65m);

        Assert.Throws<DomainException>(action);
    }
}