using CoreBankingSystem.Domain.Common;

namespace CoreBankingSystem.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string Document { get; private set; } = string.Empty;
    public string ResponsibleUser { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(string document, string responsibleUser)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            Document = document,
            ResponsibleUser = responsibleUser,
            OccurredAt = DateTime.UtcNow
        };
    }
}