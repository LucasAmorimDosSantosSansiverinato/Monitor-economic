using Microsoft.AspNetCore.Mvc;
using Monitor_economic.Monitor_economic.Infrastructure.Services;

namespace Monitor_economic.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotacaoDolarController : ControllerBase
    {
        private readonly CotacaoDolarService _service;

        public CotacaoDolarController(CotacaoDolarService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCotacoes([FromQuery] string dataInicial, [FromQuery] string dataFinal)
        {
            var resultado = await _service.ObterCotacaoAsync(dataInicial, dataFinal);

            if (resultado == null)
                return BadRequest("Erro ao buscar dados da API do Banco Central.");

            return Ok(resultado);
        }
    }
}
