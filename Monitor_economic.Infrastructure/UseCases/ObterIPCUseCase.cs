using Monitor_economic.Monitor_economic.Application.Interfaces.Service;
using Monitor_economic.Monitor_economic.Domain.Models;

namespace Monitor_economic.Application.UseCases
{
    public class ObterIPCUseCase
    {
        private readonly IIPCService _ipcService;

        public ObterIPCUseCase(IIPCService ipcService)
        {
            _ipcService = ipcService;
        }

        public async Task<List<IPCModel>> criaModel(string dataInicial, string dataFinal)    
        {
            var dtos = await _ipcService.ObterIPCAsync(dataInicial, dataFinal);
            if (dtos == null) return new();

            var listaModels = new List<IPCModel>();

            foreach (var dto in dtos)
            {
                var model = new IPCModel();
                model.data = DateTime.Parse(dto.data);
                model.valor = decimal.Parse(dto.valor, System.Globalization.CultureInfo.InvariantCulture);

                listaModels.Add(model);
            }

            return listaModels;
        }
    }
}

