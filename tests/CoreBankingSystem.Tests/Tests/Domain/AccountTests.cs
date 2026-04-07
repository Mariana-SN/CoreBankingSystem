using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Events;
using CoreBankingSystem.Shared.Exceptions;
using FluentAssertions;

namespace CoreBankingSystem.Tests.Tests.Domain;

public class AccountTests
{
    [Fact]
    public void Create_ValidData_SetsInitialBalanceToOneThousand()
    {
        var account = Account.Create("João Silva", "12345678909");

        account.Balance.Should().Be(1000m);
    }

    [Fact]
    public void Create_ValidData_IsActiveByDefault()
    {
        var account = Account.Create("João Silva", "12345678909");

        account.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ValidData_SetsOpenedAtToUtcNow()
    {
        var before = DateTime.UtcNow;
        var account = Account.Create("João Silva", "12345678909");
        var after = DateTime.UtcNow;

        account.OpenedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Create_ValidData_TrimsNameAndDocument()
    {
        var account = Account.Create("  João Silva  ", "  12345678909  ");

        account.Name.Should().Be("João Silva");
        account.Document.Should().Be("12345678909");
    }

    [Fact]
    public void Create_ValidData_PublishesAccountCreatedEvent()
    {
        var account = Account.Create("João Silva", "12345678909");

        account.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AccountCreatedEvent>();
    }

    [Fact]
    public void Debit_ValidAmount_DecreasesBalance()
    {
        var account = Account.Create("João Silva", "12345678909");

        account.Debit(400m);

        account.Balance.Should().Be(600m);
    }

    [Fact]
    public void Debit_ExceedsBalance_ThrowsInsufficientBalanceException()
    {
        var account = Account.Create("João Silva", "12345678909");

        var act = () => account.Debit(2000m);

        act.Should().Throw<InsufficientBalanceException>();
    }

    [Fact]
    public void Debit_InactiveAccount_ThrowsAccountInactiveException()
    {
        var account = Account.Create("João Silva", "12345678909");
        account.Deactivate();

        var act = () => account.Debit(100m);

        act.Should().Throw<AccountInactiveException>();
    }

    [Fact]
    public void Debit_ExactBalance_SetsBalanceToZero()
    {
        var account = Account.Create("João Silva", "12345678909");

        account.Debit(1000m);

        account.Balance.Should().Be(0m);
    }

    [Fact]
    public void Credit_ValidAmount_IncreasesBalance()
    {
        var account = Account.Create("João Silva", "12345678909");

        account.Credit(500m);

        account.Balance.Should().Be(1500m);
    }

    [Fact]
    public void Credit_InactiveAccount_ThrowsAccountInactiveException()
    {
        var account = Account.Create("João Silva", "12345678909");
        account.Deactivate();

        var act = () => account.Credit(100m);

        act.Should().Throw<AccountInactiveException>();
    }

    [Fact]
    public void Deactivate_ActiveAccount_SetsIsActiveFalse()
    {
        var account = Account.Create("João Silva", "12345678909");

        account.Deactivate();

        account.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_AlreadyInactiveAccount_ThrowsAccountInactiveException()
    {
        var account = Account.Create("João Silva", "12345678909");
        account.Deactivate();

        var act = () => account.Deactivate();

        act.Should().Throw<AccountInactiveException>();
    }

    [Fact]
    public void Deactivate_ActiveAccount_PublishesAccountDeactivatedEvent()
    {
        var account = Account.Create("João Silva", "12345678909");
        account.ClearDomainEvents();

        account.Deactivate();

        account.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AccountDeactivatedEvent>();
    }
}
