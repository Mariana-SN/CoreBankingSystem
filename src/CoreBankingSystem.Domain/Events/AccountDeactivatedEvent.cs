using CoreBankingSystem.Domain.Common;

namespace CoreBankingSystem.Domain.Events;

public record AccountDeactivatedEvent(Guid AccountId, string Document) : IDomainEvent;
