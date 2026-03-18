using MediatR;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/ipc")]
public class IPCController : ControllerBase
{
    //private readonly ObterIPCUseCase _useCase;

    //public IPCController(ObterIPCUseCase useCase)
    //{
    //    _useCase = useCase;
    //}

    private readonly IMediator _mediator;

    public IPCController(IMediator mediator)
    {
        _mediator = mediator;
    }


    //[HttpGet]
    //public async Task<IActionResult> getIPC(string dataInicial, string dataFinal)
    //{
    //    var resultado = await _useCase.criaModel(dataInicial, dataFinal);

    //    if (resultado == null)
    //        return BadRequest("Erro ao buscar dados da API IPC.");

    //    return Ok(resultado);
    //}

    [HttpGet]
    public async Task<IActionResult> getIPC(string dataInicial, string dataFinal)
    {
        var query = new GetIPCQuery(dataInicial, dataFinal);

        var resultado = await _mediator.Send(query);

        if (resultado == null)
            return BadRequest("Erro ao buscar dados da API IPC.");

        return Ok(resultado);
    }
}
