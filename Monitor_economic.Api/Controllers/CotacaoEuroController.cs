using Microsoft.AspNetCore.Mvc;
using Monitor_economic.Monitor_economic.Application.Interfaces.Service;
using Monitor_economic.Monitor_economic.Infrastructure.Services;

namespace Monitor_economic.Monitor_economic.Api.Controllers
{
    public class CotacaoEuroController
    {
        [ApiController]
        [Route("api/cotacao/euro")]
        public class CotacaoDolarController : ControllerBase
        {
            private readonly ICotacaoService _service;

            public CotacaoDolarController(ICotacaoService service)
            {
                _service = service;
            }

            [HttpGet]
            public async Task<IActionResult> GetCotacoes(string dataInicial, string dataFinal)
            {
                var resultado = await _service.ObterCotacaoAsync(dataInicial, dataFinal);

                if (resultado == null)
                    return BadRequest("Erro ao buscar dados da API do Banco Central.");

                return Ok(resultado);
            }
        }
    }
}
