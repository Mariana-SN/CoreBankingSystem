using CoreBankingSystem.Application.Accounts.Responses;
using CoreBankingSystem.Application.Common;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.Exceptions;
using CoreBankingSystem.Shared.ValueObjects;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Result<AccountResponse>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccountResponse>> Handle(CreateAccountCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<AccountResponse>.Failure("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Document))
            return Result<AccountResponse>.Failure("Documento é obrigatório.");

        var cpf = Cpf.Parse(command.Document);

        if (await _accountRepository.ExistsByDocumentAsync(cpf.Value, ct))
            throw new DuplicateDocumentException(cpf.Value);

        var account = Account.Create(command.Name, cpf.Value);

        await _accountRepository.AddAsync(account, ct);
        await _unitOfWork.CommitAsync(ct);

        return Result<AccountResponse>.Success(new AccountResponse(
            account.Id,
            account.Name,
            Cpf.FromStored(account.Document).Formatted,
            account.Balance,
            account.OpenedAt,
            account.IsActive,
            null
        ));
    }
}