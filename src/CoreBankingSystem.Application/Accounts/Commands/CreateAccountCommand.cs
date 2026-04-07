using CoreBankingSystem.Application.Accounts.Responses;
using CoreBankingSystem.Application.Common;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record CreateAccountCommand(string Name, string Document)
    : IRequest<Result<AccountResponse>>;