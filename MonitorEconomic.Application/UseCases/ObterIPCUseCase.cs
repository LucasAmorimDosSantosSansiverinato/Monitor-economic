using MonitorEconomic.Domain.Entities;

using MonitorEconomic.Application.Interfaces.Service;
using MonitorEconomic.Domain.Interfaces.IRepository;


namespace MonitorEconomic.Application.UseCases;

    public class ObterIPCUseCase
    {
        private readonly IIPCService _ipcService;
        private readonly IIPCRepository _ipcRepository;
        public ObterIPCUseCase(IIPCService ipcService, IIPCRepository ipcRepository)
        {
            _ipcService = ipcService;
            _ipcRepository = ipcRepository;
        }

        public async Task<List<IPCBaseModel>> criaModel(string dataInicial, string dataFinal)    
        {
            var dtos = await _ipcService.obterIPCAsync(dataInicial, dataFinal);
        if (dtos == null)
        {
            Console.WriteLine("Nenhum dado retornado da API IPC");
            return new();
        }

        Console.WriteLine($"Quantidade de registros retornados: {dtos.Count}");

        foreach (var dto in dtos)
        {
            Console.WriteLine($"Data: {dto.data}, Valor: {dto.valor}"); 
        }

        var listaModels = new List<IPCBaseModel>();

            // foreach (var dto in dtos)
            // {

            //     var data = DateTime.Parse(dto.data);
            //     var valor = decimal.Parse(dto.valor, System.Globalization.CultureInfo.InvariantCulture);
                
            //     var model = new IPCBaseModel(data, valor);

            //     listaModels.Add(model);
            //     await _ipcRepository.salvarAsync(model);
            // }

            foreach (var dto in dtos)
{
    var data = DateTime.SpecifyKind(DateTime.Parse(dto.data), DateTimeKind.Utc);
    var valor = decimal.Parse(dto.valor, System.Globalization.CultureInfo.InvariantCulture);

    var model = new IPCBaseModel(data, valor);
    listaModels.Add(model);
    await _ipcRepository.salvarAsync(model); // já vai salvar em UTC
}

            return listaModels;
        }
    }


