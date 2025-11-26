using Microsoft.AspNetCore.Mvc;
using Monitor_economic.Application.UseCases;

[ApiController]
[Route("api/ipc")]
public class IPCController : ControllerBase
{
    private readonly ObterIPCUseCase _useCase;

    public IPCController(ObterIPCUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet]
    public async Task<IActionResult> getIPC(string dataInicial, string dataFinal)
    {
        var resultado = await _useCase.criaModel(dataInicial, dataFinal);

        if (resultado == null)
            return BadRequest("Erro ao buscar dados da API IPC.");

        return Ok(resultado);
    }
}
