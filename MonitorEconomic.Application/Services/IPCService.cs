using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Interfaces.Service;
using System.Net.Http.Json;

namespace MonitorEconomic.Infra.Data.Services
{
    public class IPCService : IIPCService
    {
        private readonly HttpClient _httpClient;

        public IPCService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ItemIPCDto>?> obterIPCAsync(string dataInicial, string dataFinal)
        {

            var dataInicialFormatada = DateTime
            .Parse(dataInicial)
            .ToString("dd/MM/yyyy");

            var dataFinalFormatada = DateTime
            .Parse(dataFinal)
            .ToString("dd/MM/yyyy");


            string url = $"https://api.bcb.gov.br/dados/serie/bcdata.sgs." +
                $"{7463}/dados?formato=json&dataInicial={dataInicialFormatada}&dataFinal={dataFinalFormatada}";

            try
            {
                return await _httpClient.GetFromJsonAsync<List<ItemIPCDto>>(url);
               
       
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter IPC:  + {ex.Message}");
                return null;
            }

        }
    }
}
