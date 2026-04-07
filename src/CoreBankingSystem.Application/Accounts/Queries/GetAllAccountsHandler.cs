using CoreBankingSystem.Application.Accounts.Responses;
using CoreBankingSystem.Application.Common;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.ValueObjects;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

public class GetAllAccountsHandler : IRequestHandler<GetAllAccountsQuery, Result<IEnumerable<AccountResponse>>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAllAccountsHandler(
        IAccountRepository accountRepository,
        IAuditLogRepository auditLogRepository)
    {
        _accountRepository = accountRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<IEnumerable<AccountResponse>>> Handle(GetAllAccountsQuery query, CancellationToken ct)
    {
        var accounts = await _accountRepository.GetAllAsync(ct);

        var responses = new List<AccountResponse>();

        foreach (var account in accounts)
        {
            AccountDeactivationInfo? deactivationInfo = null;

            if (!account.IsActive)
            {
                var auditLogs = await _auditLogRepository.GetByDocumentAsync(account.Document, ct);
                var latest = auditLogs.FirstOrDefault();

                if (latest is not null)
                    deactivationInfo = new AccountDeactivationInfo(latest.ResponsibleUser, latest.OccurredAt);
            }

            responses.Add(new AccountResponse(
                account.Id,
                account.Name,
                Cpf.FromStored(account.Document).Formatted,
                account.Balance,
                account.OpenedAt,
                account.IsActive,
                deactivationInfo
            ));
        }

        return Result<IEnumerable<AccountResponse>>.Success(responses);
    }
}