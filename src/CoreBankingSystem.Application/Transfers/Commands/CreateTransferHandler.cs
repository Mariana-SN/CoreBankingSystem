using CoreBankingSystem.Application.Common;
using CoreBankingSystem.Application.Transfers.Responses;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.Exceptions;
using CoreBankingSystem.Shared.ValueObjects;
using MediatR;

namespace CoreBankingSystem.Application.Transfers.Commands;

public class CreateTransferHandler : IRequestHandler<CreateTransferCommand, Result<CreateTransferResponse>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransferRepository _transferRepository;
    private readonly ITransferDomainService _transferDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransferHandler(
        IAccountRepository accountRepository,
        ITransferRepository transferRepository,
        ITransferDomainService transferDomainService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transferRepository = transferRepository;
        _transferDomainService = transferDomainService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateTransferResponse>> Handle(CreateTransferCommand command, CancellationToken ct)
    {
        var originCpf = Cpf.Parse(command.OriginDocument);
        var destinationCpf = Cpf.Parse(command.DestinationDocument);

        var origin = await _accountRepository.GetByDocumentAsync(originCpf.Value, ct)
            ?? throw new AccountNotFoundException(originCpf.Value);

        var destination = await _accountRepository.GetByDocumentAsync(destinationCpf.Value, ct)
            ?? throw new AccountNotFoundException(destinationCpf.Value);

        var transfer = _transferDomainService.ExecuteTransfer(origin, destination, command.Amount);

        await _accountRepository.UpdateAsync(origin, ct);
        await _accountRepository.UpdateAsync(destination, ct);
        await _transferRepository.AddAsync(transfer, ct);
        await _unitOfWork.CommitAsync(ct);

        return Result<CreateTransferResponse>.Success(new CreateTransferResponse(
            transfer.Id,
            origin.Document,
            destination.Document,
            transfer.Amount,
            transfer.OccurredAt
        ));
    }
}