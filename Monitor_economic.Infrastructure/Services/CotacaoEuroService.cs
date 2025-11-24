using Monitor_economic.Application.Dto;
using Monitor_economic.Monitor_economic.Application.Interfaces.Service;

namespace Monitor_economic.Monitor_economic.Infrastructure.Services
{
    public class CotacaoEuroService : ICotacaoService
    {
        private readonly HttpClient _httpClient;

        public CotacaoEuroService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CotacaoDto?> ObterCotacaoAsync (string dataInicial, string dataFinal)
        {
            string url =
               $"https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/" +
               $"CotacaoMoedaPeriodo(moeda=@moeda,dataInicial=@dataInicial,dataFinalCotacao=@dataFinalCotacao)?" +
               $"@moeda='EUR'&@dataInicial='{dataInicial}'&@dataFinalCotacao='{dataFinal}'&$format=json";

            try
            {   
                return await _httpClient.GetFromJsonAsync<CotacaoDto>(url);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro ao obter cotação do euro:  + {ex.Message}");
                return null;
            }
        }
    }
}
