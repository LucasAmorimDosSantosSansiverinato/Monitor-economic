using MonitorEconomic.Domain.Exceptions;
using System.Data;

namespace MonitorEconomic.Domain.Entities;

public class IPCBaseDomain
{
    public DateTime Data { get; private set; }
    public decimal Valor { get; private set; }

    public IPCBaseDomain(DateTime data, decimal valor)
    {
        if (data == DateTime.MinValue)
        {
            throw new DomainException("Data não pode ser vazia");
        }
        if (data > DateTime.UtcNow)
        { 
            throw new DomainException("A data não pode ser futura.");
        }
        
        Data = data;
        Valor = valor;
    }

    public void AtualizarValor(decimal novoValor)
    {
        Valor = novoValor;
    }

    public void AtualizarData(DateTime novaData)
    {
        if (novaData > DateTime.UtcNow)
            throw new DomainException("A data não pode ser futura.");

        Data = novaData;
    }
}