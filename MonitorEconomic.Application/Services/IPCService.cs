using MonitorEconomic.Application.Dto;
using MonitorEconomic.Application.Interfaces.Service;
using System.Net.Http.Json;

namespace MonitorEconomic.Application.Services
{
    public class IPCService : IIPCService
    {
        private readonly HttpClient _httpClient;

        public IPCService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<IPCDto>?> obterIPCAsync(string dataInicial, string dataFinal)
        {

            if (!DateTime.TryParseExact(dataInicial, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dataInicialParsed))
                throw new ArgumentException("data Inicial deve estar com formato em dd/MM/yyyy", nameof(dataInicial));

            if (!DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dataFinalParsed))
                throw new ArgumentException("data Final deve estar com formato em dd/MM/yyyy", nameof(dataFinal));

            var dataInicialFormatada = dataInicialParsed.ToString("dd/MM/yyyy");
            var dataFinalFormatada = dataFinalParsed.ToString("dd/MM/yyyy");


            string url = $"https://api.bcb.gov.br/dados/serie/bcdata.sgs." +
                $"{7463}/dados?formato=json&dataInicial={dataInicialFormatada}&dataFinal={dataFinalFormatada}";

            try
            {
                return await _httpClient.GetFromJsonAsync<List<IPCDto>>(url);
               
       
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter IPC:  + {ex.Message}");
                return null;
            }

        }
    }
}
