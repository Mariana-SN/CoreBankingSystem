using CoreBankingSystem.Application.Accounts.Responses;
using CoreBankingSystem.Application.Common;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

public record GetAllAccountsQuery : IRequest<Result<IEnumerable<AccountResponse>>>;