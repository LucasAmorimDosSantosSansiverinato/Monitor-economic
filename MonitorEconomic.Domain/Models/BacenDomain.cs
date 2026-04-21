using MonitorEconomic.Domain.Exceptions;
using MonitorEconomic.Domain.Enums;

namespace MonitorEconomic.Domain.Entities;

public class BacenDomain
{
    public Guid Id { get; private set; }
    public BacenSerie Serie { get; private set; }
    public DateTime Data { get; private set; }
    public decimal Valor { get; private set; }

    public BacenDomain(BacenSerie serie, DateTime data, decimal valor)
        : this(Guid.NewGuid(), serie, data, valor)
    {
    }

    public BacenDomain(Guid id, BacenSerie serie, DateTime data, decimal valor)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Id não pode ser vazio");
        }

        if (!Enum.IsDefined(serie))
        {
            throw new DomainException("Serie inválida");
        }

        if (data == DateTime.MinValue)
        {
            throw new DomainException("Data não pode ser vazia");
        }
        if (data > DateTime.UtcNow)
        { 
            throw new DomainException("A data não pode ser futura.");
        }

        Id = id;
        Serie = serie;
        Data = data;
        Valor = valor;
    }
}