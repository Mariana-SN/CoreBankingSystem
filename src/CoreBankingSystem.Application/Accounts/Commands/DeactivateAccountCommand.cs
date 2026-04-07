using CoreBankingSystem.Application.Common;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record DeactivateAccountCommand(string Document): IRequest<Result>;