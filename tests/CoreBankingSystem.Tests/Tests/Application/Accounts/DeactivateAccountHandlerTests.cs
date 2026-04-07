using CoreBankingSystem.Application.Accounts.Commands;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBankingSystem.Tests.Tests.Application.Accounts;

public class DeactivateAccountHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly DeactivateAccountHandler _handler;

    public DeactivateAccountHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);
        _handler = new DeactivateAccountHandler(
            _accountRepositoryMock.Object,
            _auditLogRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ActiveAccount_ReturnsSuccess()
    {
        var account = Account.Create("João Silva", "60014377004");
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(account);

        var result = await _handler.Handle(new DeactivateAccountCommand("600.143.770-04"), default);

        result.IsSuccess.Should().BeTrue();
        account.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_AccountNotFound_ThrowsAccountNotFoundException()
    {
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync((Account?)null);

        var act = async () => await _handler.Handle(new DeactivateAccountCommand("600.143.770-04"), default);

        await act.Should().ThrowAsync<AccountNotFoundException>();
    }

    [Fact]
    public async Task Handle_InactiveAccount_ThrowsAccountInactiveException()
    {
        var account = Account.Create("João Silva", "60014377004");
        account.Deactivate();
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(account);

        var act = async () => await _handler.Handle(new DeactivateAccountCommand("600.143.770-04"), default);

        await act.Should().ThrowAsync<AccountInactiveException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAndCommit()
    {
        var account = Account.Create("João Silva", "60014377004");
        _accountRepositoryMock
            .Setup(r => r.GetByDocumentAsync("60014377004", default))
            .ReturnsAsync(account);

        await _handler.Handle(new DeactivateAccountCommand("600.143.770-04"), default);

        _accountRepositoryMock.Verify(r => r.UpdateAsync(account, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(default), Times.Once);
    }
}
