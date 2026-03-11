using MonitorEconomic.Domain.Exceptions;

namespace MonitorEconomic.Domain.Entities;

public class IPCBaseModel
{
    public int Id { get; private set; }
    public DateTime Data { get; private set; }
    public decimal Valor { get; private set; }

    public IPCBaseModel(){}
    public IPCBaseModel(DateTime data, decimal valor)
    {
        if (data > DateTime.UtcNow)
            throw new DomainException("A data não pode ser futura.");

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