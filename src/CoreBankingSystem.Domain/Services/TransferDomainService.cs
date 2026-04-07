using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.Exceptions;

namespace CoreBankingSystem.Domain.Services;

public class TransferDomainService : ITransferDomainService
{
    public Transfer ExecuteTransfer(Account origin, Account destination, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidAmountException();

        if (origin.Id == destination.Id)
            throw new SameAccountTransferException();

        origin.Debit(amount);
        destination.Credit(amount);

        return Transfer.Create(origin.Id, destination.Id, amount);
    }
}