using CoreBankingSystem.API.Extensions;
using CoreBankingSystem.Application.Transfers.Commands;
using CoreBankingSystem.Application.Transfers.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransfersController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransfersController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Realiza uma transferência entre duas contas ativas.
    /// </summary>
    /// <param name="command">Dados da transferência.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Dados da transferência realizada.</returns>
    /// <response code="201">Transferência realizada com sucesso.</response>
    /// <response code="400">CPF inválido, valor inválido ou mesma conta de origem e destino.</response>
    /// <response code="404">Conta de origem ou destino não encontrada.</response>
    /// <response code="422">Conta inativa ou saldo insuficiente.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTransferResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Transfer([FromBody] CreateTransferCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.ToActionResult(201);
    }
}
