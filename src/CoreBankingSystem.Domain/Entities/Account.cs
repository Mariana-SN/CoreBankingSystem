using CoreBankingSystem.Domain.Common;
using CoreBankingSystem.Domain.Events;
using CoreBankingSystem.Shared.Exceptions;

namespace CoreBankingSystem.Domain.Entities;

public class Account : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public bool IsActive { get; private set; }

    private Account() { }

    public static Account Create(string name, string document)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Document = document.Trim(),
            Balance = 1000m,
            OpenedAt = DateTime.UtcNow,
            IsActive = true
        };

        account.AddDomainEvent(new AccountCreatedEvent(account.Id, account.Document));

        return account;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new AccountInactiveException();

        IsActive = false;

        AddDomainEvent(new AccountDeactivatedEvent(Id, Document));
    }

    public void Debit(decimal amount)
    {
        if (!IsActive)
            throw new AccountInactiveException();

        if (amount <= 0)
            throw new InvalidAmountException();

        if (Balance < amount)
            throw new InsufficientBalanceException();

        Balance -= amount;

        AddDomainEvent(new AccountDebitedEvent(Id, amount));
    }

    public void Credit(decimal amount)
    {
        if (!IsActive)
            throw new AccountInactiveException();

        if (amount <= 0)
            throw new InvalidAmountException();

        Balance += amount;

        AddDomainEvent(new AccountCreditedEvent(Id, amount));
    }
}