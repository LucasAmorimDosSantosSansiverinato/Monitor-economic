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
}