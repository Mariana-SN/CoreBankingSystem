using CoreBankingSystem.Application.Accounts.Responses;
using CoreBankingSystem.Application.Common;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

public record GetAccountsQuery(string? Name, string? Document)
    : IRequest<Result<IEnumerable<AccountResponse>>>;