using Monitor_economic.Monitor_economic.Application.Dto;
using Monitor_economic.Monitor_economic.Application.Interfaces.Service;

namespace Monitor_economic.Monitor_economic.Infrastructure.Services
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
            string url = $"https://api.bcb.gov.br/dados/serie/bcdata.sgs." +
                $"{7463}/dados?formato=json&dataInicial={dataInicial}&dataFinal={dataFinal}";

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
