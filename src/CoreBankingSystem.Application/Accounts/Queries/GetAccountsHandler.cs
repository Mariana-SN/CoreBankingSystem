using CoreBankingSystem.Application.Accounts.Responses;
using CoreBankingSystem.Application.Common;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.ValueObjects;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, Result<IEnumerable<AccountResponse>>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAccountsHandler(
        IAccountRepository accountRepository,
        IAuditLogRepository auditLogRepository)
    {
        _accountRepository = accountRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<IEnumerable<AccountResponse>>> Handle(GetAccountsQuery query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query.Name) && string.IsNullOrWhiteSpace(query.Document))
            return Result<IEnumerable<AccountResponse>>.Failure("Pelo menos um dos filtros é obrigatório.");
        
        var accounts = await _accountRepository.SearchAsync(query.Name, query.Document, ct);

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