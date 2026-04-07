using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByDocumentAsync(string document, CancellationToken ct = default);
    Task<IEnumerable<Account>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Account>> SearchAsync(string? name, string? document, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    Task UpdateAsync(Account account, CancellationToken ct = default);
    Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct = default);
}