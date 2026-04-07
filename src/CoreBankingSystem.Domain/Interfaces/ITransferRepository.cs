using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Domain.Interfaces;

public interface ITransferRepository
{
    Task AddAsync(Transfer transfer, CancellationToken ct = default);
}
