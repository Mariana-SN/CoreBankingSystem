using CoreBankingSystem.Domain.Common;
using CoreBankingSystem.Domain.Events;

namespace CoreBankingSystem.Domain.Entities;

public class Transfer : AggregateRoot
{
    public Guid OriginAccountId { get; private set; }
    public Guid DestinationAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime OccurredAt { get; private set; }

    public Account? OriginAccount { get; private set; }
    public Account? DestinationAccount { get; private set; }

    private Transfer() { }

    public static Transfer Create(Guid originAccountId, Guid destinationAccountId, decimal amount)
    {
        var transfer = new Transfer
        {
            Id = Guid.NewGuid(),
            OriginAccountId = originAccountId,
            DestinationAccountId = destinationAccountId,
            Amount = amount,
            OccurredAt = DateTime.UtcNow
        };

        transfer.AddDomainEvent(new TransferCreatedEvent(
            transfer.Id,
            transfer.OriginAccountId,
            transfer.DestinationAccountId,
            transfer.Amount
        ));

        return transfer;
    }
}