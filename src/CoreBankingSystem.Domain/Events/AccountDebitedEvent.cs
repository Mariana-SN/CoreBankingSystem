using CoreBankingSystem.Domain.Common;

namespace CoreBankingSystem.Domain.Events;

public record AccountDebitedEvent(Guid AccountId, decimal Amount) : IDomainEvent;
