using Monitor_economic.Application.Dto;

namespace Monitor_economic.Monitor_economic.Application.Interfaces.Service
{
    public interface ICotacaoService
    {
        Task<CotacaoDto?> ObterCotacaoAsync(string dataInicial, string dataFinal);
    }
}
