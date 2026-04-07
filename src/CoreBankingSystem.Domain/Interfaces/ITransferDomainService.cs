using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Domain.Interfaces;

public interface ITransferDomainService
{
    Transfer ExecuteTransfer(Account origin, Account destination, decimal amount);
}
