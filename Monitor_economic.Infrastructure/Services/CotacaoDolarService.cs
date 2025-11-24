using Monitor_economic.Application.Dto;
using Monitor_economic.Monitor_economic.Application.Interfaces.Service;
using System.Net.Http;

namespace Monitor_economic.Monitor_economic.Infrastructure.Services
{
    public class CotacaoDolarService : ICotacaoService
    {
        private readonly HttpClient _httpClient;

        public CotacaoDolarService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CotacaoDto?> ObterCotacaoAsync(string dataInicial, String dataFinal)
        {
            String url = 
                $"https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/" +
                $"CotacaoDolarPeriodo(dataInicial=@dataInicial,dataFinalCotacao=@dataFinalCotacao)?" +
                $"@dataInicial='{dataInicial}'&@dataFinalCotacao='{dataFinal}'&$format=json";

            try
            {
                return await _httpClient.GetFromJsonAsync<CotacaoDto>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter cotação do dólar:  + {ex.Message}");
                return null;
            }
        }

    }
    
}
