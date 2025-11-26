using Monitor_economic._2_Monitor_economic.Application.Entity;
using Monitor_economic.Monitor_economic.Domain.Models;

namespace Monitor_economic._2_Monitor_economic.Application.Mapper
{
    public static class IPCMapper
    {
        public static IPCEntity ToEntity(IPCModel model)
        {
            var entity = new IPCEntity();
            entity.Id = model.id;
            entity.Data = model.data;
            entity.Valor = model.valor;

            return entity;
        }

        public static IPCModel ToDomain(IPCEntity entity)
        {
            var model = new IPCModel();
            model.id = entity.Id;
            model.data = entity.Data;
            model.valor = entity.Valor;

            return model;
        }
    }
}
