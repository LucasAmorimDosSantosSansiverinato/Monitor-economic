using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Exceptions;
using System.Globalization;

namespace MonitorEconomic.Infra.Data.Adapter;

    public static class IPCAdapter
    {
        public static IPCBaseModel ToDomain(ItemIPCDto dto)
        {
            if (!DateTime.TryParse(dto.data, out var data))
                throw new DomainException($"Data invalida: {dto.data}");

            if (!decimal.TryParse(dto.valor, System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var valor))
                    throw new DomainException($"Valor invalido: {dto.valor}");

            return new IPCBaseModel(data, valor);
        }
    }
