using MonitorEconomic.Domain.Exceptions;

namespace MonitorEconomic.Domain.Entities;

public class IPCBaseModel
{
    public int Id { get; private set; }
    public DateTime data { get; private set; }
    public decimal Valor { get; private set; }

    public IPCBaseModel(DateTime data, decimal valor)
    {
        if (data > DateTime.UtcNow)
            throw new DomainException("A data não pode ser futura.");

        data = data;
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

        data = novaData;
    }
}