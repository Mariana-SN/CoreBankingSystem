using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly CoreBankingSystemDbContext _context;

    public AuditLogRepository(CoreBankingSystemDbContext context) => _context = context;

    public Task AddAsync(AuditLog auditLog, CancellationToken ct = default)
    {
        _context.AuditLogs.Add(auditLog);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<AuditLog>> GetByDocumentAsync(string document, CancellationToken ct = default) =>
        await _context.AuditLogs
            .Where(a => a.Document == document)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync(ct);
}