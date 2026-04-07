using CoreBankingSystem.Application.Common;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.Exceptions;
using CoreBankingSystem.Shared.ValueObjects;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public class DeactivateAccountHandler : IRequestHandler<DeactivateAccountCommand, Result>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateAccountHandler(
        IAccountRepository accountRepository,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateAccountCommand command, CancellationToken ct)
    {
        var cpf = Cpf.Parse(command.Document);

        var account = await _accountRepository.GetByDocumentAsync(cpf.Value, ct)
            ?? throw new AccountNotFoundException(cpf.Value);

        account.Deactivate();

        var auditLog = AuditLog.Create(cpf.Value, "Gestor Master");

        await _accountRepository.UpdateAsync(account, ct);
        await _auditLogRepository.AddAsync(auditLog, ct);
        await _unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}