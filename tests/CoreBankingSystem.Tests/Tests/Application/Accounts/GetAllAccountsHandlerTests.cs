using CoreBankingSystem.Application.Accounts.Queries;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CoreBankingSystem.Tests.Tests.Application.Accounts;

public class GetAllAccountsHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock = new();
    private readonly GetAllAccountsHandler _handler;

    public GetAllAccountsHandlerTests()
    {
        _handler = new GetAllAccountsHandler(
            _accountRepositoryMock.Object,
            _auditLogRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithAccounts_ReturnsAllAccounts()
    {
        var accounts = new List<Account>
        {
            Account.Create("João Silva", "60014377004"),
            Account.Create("Maria Souza", "59387085082")
        };

        _accountRepositoryMock
            .Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAllAccountsQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_NoAccounts_ReturnsEmptyList()
    {
        _accountRepositoryMock
            .Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<Account>());

        var result = await _handler.Handle(new GetAllAccountsQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithAccounts_ReturnsCpfFormatted()
    {
        var accounts = new List<Account>
        {
            Account.Create("João Silva", "60014377004")
        };
        _accountRepositoryMock
            .Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAllAccountsQuery(), default);

        result.Value!.First().Document.Should().Be("600.143.770-04");
    }

    [Fact]
    public async Task Handle_WithAccounts_ReturnsCorrectBalance()
    {
        var accounts = new List<Account>
        {
            Account.Create("João Silva", "60014377004")
        };
        _accountRepositoryMock
            .Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAllAccountsQuery(), default);

        result.Value!.First().Balance.Should().Be(1000m);
    }

    [Fact]
    public async Task Handle_InactiveAccount_ReturnsDeactivationInfo()
    {
        var account = Account.Create("João Silva", "60014377004");
        account.Deactivate();

        var auditLogs = new List<AuditLog>
        {
            AuditLog.Create("60014377004", "Gestor Master")
        };

        _accountRepositoryMock
            .Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<Account> { account });

        _auditLogRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(auditLogs);

        var result = await _handler.Handle(new GetAllAccountsQuery(), default);

        result.Value!.First().Deactivation.Should().NotBeNull();
        result.Value!.First().Deactivation!.ResponsibleUser.Should().Be("Gestor Master");
    }

    [Fact]
    public async Task Handle_ActiveAccount_ReturnsNullDeactivationInfo()
    {
        var accounts = new List<Account>
        {
            Account.Create("João Silva", "60014377004")
        };
        _accountRepositoryMock
            .Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAllAccountsQuery(), default);

        result.Value!.First().Deactivation.Should().BeNull();
    }
}
