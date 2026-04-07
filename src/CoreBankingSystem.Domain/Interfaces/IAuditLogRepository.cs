using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken ct = default);
    Task<IEnumerable<AuditLog>> GetByDocumentAsync(string document, CancellationToken ct = default);
}