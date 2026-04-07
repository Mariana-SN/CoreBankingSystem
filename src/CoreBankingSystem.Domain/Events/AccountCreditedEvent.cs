using CoreBankingSystem.Domain.Common;

namespace CoreBankingSystem.Domain.Events;

public record AccountCreditedEvent(Guid AccountId, decimal Amount) : IDomainEvent;