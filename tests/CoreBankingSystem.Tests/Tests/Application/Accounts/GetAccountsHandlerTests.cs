using CoreBankingSystem.Application.Accounts.Queries;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CoreBankingSystem.Tests.Tests.Application.Accounts;

public class GetAccountsHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock = new();
    private readonly GetAccountsHandler _handler;

    public GetAccountsHandlerTests()
    {
        _handler = new GetAccountsHandler(
            _accountRepositoryMock.Object,
            _auditLogRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithNameFilter_ReturnsFilteredAccounts()
    {
        var accounts = new List<Account>
        {
            Account.Create("João Silva", "60014377004")
        };
        _accountRepositoryMock
            .Setup(r => r.SearchAsync("João", null, default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAccountsQuery("João", null), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Name.Should().Be("João Silva");
    }

    [Fact]
    public async Task Handle_WithDocumentFilter_ReturnsFilteredAccounts()
    {
        var accounts = new List<Account>
        {
            Account.Create("João Silva", "60014377004")
        };

        _accountRepositoryMock
            .Setup(r => r.SearchAsync(null, "600.143.770-04", default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAccountsQuery(null, "600.143.770-04"), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsFailure()
    {
        var result = await _handler.Handle(new GetAccountsQuery(null, null), default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithFilters_ReturnsCpfFormatted()
    {
        var accounts = new List<Account>
        {
            Account.Create("João Silva", "60014377004")
        };
        _accountRepositoryMock
            .Setup(r => r.SearchAsync("João", null, default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAccountsQuery("João", null), default);

        result.Value!.First().Document.Should().Be("600.143.770-04");
    }

    [Fact]
    public async Task Handle_NoResults_ReturnsEmptyList()
    {
        _accountRepositoryMock
            .Setup(r => r.SearchAsync("Inexistente", null, default))
            .ReturnsAsync(new List<Account>());

        var result = await _handler.Handle(new GetAccountsQuery("Inexistente", null), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
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
            .Setup(r => r.SearchAsync("João", null, default))
            .ReturnsAsync(new List<Account> { account });

        _auditLogRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(auditLogs);

        var result = await _handler.Handle(new GetAccountsQuery("João", null), default);

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
            .Setup(r => r.SearchAsync("João", null, default))
            .ReturnsAsync(accounts);

        var result = await _handler.Handle(new GetAccountsQuery("João", null), default);

        result.Value!.First().Deactivation.Should().BeNull();
    }
}
