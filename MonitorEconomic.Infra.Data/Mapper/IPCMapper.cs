using MonitorEconomic.Domain.Entities;
using MonitorEconomic.Infra.Data.Models;

public static class IPCMapper
{
    public static IPCBaseModel ToDomain(IPCModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        IPCBaseModel ipc = new IPCBaseModel(model.Data, model.Valor);

        return ipc;
    }

    public static IPCModel ToModel(IPCBaseModel entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        IPCModel model = new IPCModel();

        model.Data = entity.data;
        model.Valor = entity.Valor;

        return model;
    }

}