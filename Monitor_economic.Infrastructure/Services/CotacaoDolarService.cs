using Monitor_economic.Application.Dto;
using System.Net.Http;

namespace Monitor_economic.Monitor_economic.Infrastructure.Services
{
    public class CotacaoDolarService
    {
        private readonly HttpClient _httpClient;

        public CotacaoDolarService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CotacaoDolarDto> ObterCotacaoAsync(string dataInicial, string dataFinal)
        {
            string url = $"https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/" +
                         $"CotacaoDolarPeriodo(dataInicial=@dataInicial,dataFinalCotacao=@dataFinalCotacao)?" +
                         $"@dataInicial='{dataInicial}'&@dataFinalCotacao='{dataFinal}'&$format=json";

            try
            {
                var response = await _httpClient.GetFromJsonAsAsync<CotacaoDolarDto>(url);
                return response;
            }
            catch (Exception ex)
            {
                
               Console.WriteLine($"Erro ao obter cotação do dólar: {ex.Message}");
                return null;
            }
        }
    }
}
