using CoreBankingSystem.Domain.Common;

namespace CoreBankingSystem.Domain.Events;

public record AccountCreatedEvent(Guid AccountId, string Document) : IDomainEvent;