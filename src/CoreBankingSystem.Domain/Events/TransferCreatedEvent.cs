using CoreBankingSystem.Domain.Common;

namespace CoreBankingSystem.Domain.Events;

public record TransferCreatedEvent(Guid TransferId, Guid OriginAccountId, Guid DestinationAccountId, decimal Amount) : IDomainEvent;