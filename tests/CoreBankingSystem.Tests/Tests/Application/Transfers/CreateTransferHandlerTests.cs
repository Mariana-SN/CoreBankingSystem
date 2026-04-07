using CoreBankingSystem.Application.Transfers.Commands;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Domain.Services;
using CoreBankingSystem.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace CoreBankingSystem.Tests.Tests.Application.Transfers;

public class CreateTransferHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<ITransferRepository> _transferRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateTransferHandler _handler;

    public CreateTransferHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);
        _handler = new CreateTransferHandler(
            _accountRepositoryMock.Object,
            _transferRepositoryMock.Object,
            new TransferDomainService(),
            _unitOfWorkMock.Object
        );
    }

    private static Account ActiveAccount(string name, string document) =>
        Account.Create(name, document);

    private static Account InactiveAccount(string name, string document)
    {
        var account = Account.Create(name, document);
        account.Deactivate();
        return account;
    }

    [Fact]
    public async Task Handle_ValidTransfer_ReturnsSuccess()
    {
        var origin = ActiveAccount("João Silva", "60014377004");
        var destination = ActiveAccount("Maria Souza", "59387085082");

        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(origin);
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("59387085082", default))
            .ReturnsAsync(destination);

        var result = await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "593.870.850-82", 300m), default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(300m);
        origin.Balance.Should().Be(700m);
        destination.Balance.Should().Be(1300m);
    }

    [Fact]
    public async Task Handle_InsufficientBalance_ThrowsInsufficientBalanceException()
    {
        var origin = ActiveAccount("João Silva", "60014377004");
        var destination = ActiveAccount("Maria Souza", "59387085082");

        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(origin);
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("59387085082", default))
            .ReturnsAsync(destination);

        var act = async () => await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "593.870.850-82", 9999m), default);

        await act.Should().ThrowAsync<InsufficientBalanceException>();
    }

    [Fact]
    public async Task Handle_OriginAccountNotFound_ThrowsAccountNotFoundException()
    {
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync((Account?)null);

        var act = async () => await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "593.870.850-82", 100m), default);

        await act.Should().ThrowAsync<AccountNotFoundException>();
    }

    [Fact]
    public async Task Handle_DestinationAccountNotFound_ThrowsAccountNotFoundException()
    {
        var origin = ActiveAccount("João Silva", "60014377004");

        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(origin);
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("59387085082", default))
            .ReturnsAsync((Account?)null);

        var act = async () => await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "593.870.850-82", 100m), default);

        await act.Should().ThrowAsync<AccountNotFoundException>();
    }

    [Fact]
    public async Task Handle_InactiveOrigin_ThrowsAccountInactiveException()
    {
        var origin = InactiveAccount("João Silva", "60014377004");
        var destination = ActiveAccount("Maria Souza", "59387085082");

        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(origin);
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("59387085082", default))
            .ReturnsAsync(destination);

        var act = async () => await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "593.870.850-82", 100m), default);

        await act.Should().ThrowAsync<AccountInactiveException>();
    }

    [Fact]
    public async Task Handle_InactiveDestination_ThrowsAccountInactiveException()
    {
        var origin = ActiveAccount("João Silva", "60014377004");
        var destination = InactiveAccount("Maria Souza", "59387085082");

        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(origin);
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("59387085082", default))
            .ReturnsAsync(destination);

        var act = async () => await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "593.870.850-82", 100m), default);

        await act.Should().ThrowAsync<AccountInactiveException>();
    }

    [Fact]
    public async Task Handle_SameAccount_ThrowsSameAccountTransferException()
    {
        var origin = ActiveAccount("João Silva", "60014377004");

        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(origin);

        var act = async () => await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "600.143.770-04", 100m), default);

        await act.Should().ThrowAsync<SameAccountTransferException>();
    }

    [Fact]
    public async Task Handle_ValidTransfer_PersistsTransferRecord()
    {
        var origin = ActiveAccount("João Silva", "60014377004");
        var destination = ActiveAccount("Maria Souza", "59387085082");

        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(origin);
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("59387085082", default))
            .ReturnsAsync(destination);

        await _handler.Handle(
            new CreateTransferCommand("600.143.770-04", "593.870.850-82", 100m), default);

        _transferRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transfer>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(default), Times.Once);
    }
}
