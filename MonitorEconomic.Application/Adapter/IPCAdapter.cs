using MonitorEconomic.Application.Dto;
using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Domain.Exceptions;
using System.Globalization;

namespace MonitorEconomic.Infra.Data.Adapter;

    public class IPCAdapter // sem nescessidade de ser estática
{
        public IPCBaseDomain ToDomain(ItemIPCDto dto)
        {
            return new IPCBaseDomain(data, valor);
        }
    }


// procurar usar automapper, só baixar o pacote e colocar ele na injeção de dependencias