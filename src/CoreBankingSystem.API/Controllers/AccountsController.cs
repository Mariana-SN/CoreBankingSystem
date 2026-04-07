using CoreBankingSystem.API.Extensions;
using CoreBankingSystem.Application.Accounts.Commands;
using CoreBankingSystem.Application.Accounts.Queries;
using CoreBankingSystem.Application.Accounts.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Cria uma nova conta bancária.
    /// </summary>
    /// <param name="command">Dados da conta a ser criada.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Dados da conta criada.</returns>
    /// <response code="201">Conta criada com sucesso com saldo inicial de R$ 1.000,00.</response>
    /// <response code="400">Dados inválidos ou CPF inválido.</response>
    /// <response code="409">Já existe uma conta cadastrada com este CPF.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.ToActionResult(201);
    }

    /// <summary>
    /// Retorna todas as contas cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de todas as contas.</returns>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllAccountsQuery(), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Busca contas por nome ou documento.
    /// </summary>
    /// <param name="name">Nome parcial ou completo do titular.</param>
    /// <param name="document">CPF do titular (com ou sem pontuação).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de contas que correspondem ao filtro.</returns>
    /// <response code="200">Lista retornada com sucesso.</response>
    /// <response code="400">Nenhum filtro informado.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<GetAccountsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAll(
        [FromQuery] string? name,
        [FromQuery] string? document,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAccountsQuery(name, document), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Desativa uma conta bancária pelo CPF do titular.
    /// </summary>
    /// <param name="document">CPF do titular (com ou sem pontuação).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Sem conteúdo.</returns>
    /// <response code="204">Conta desativada com sucesso.</response>
    /// <response code="400">CPF inválido.</response>
    /// <response code="404">Conta não encontrada.</response>
    /// <response code="422">Conta já está inativa.</response>
    [HttpPatch("{document}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Deactivate([FromRoute] string document, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeactivateAccountCommand(document), ct);
        return result.ToActionResult();
    }
}