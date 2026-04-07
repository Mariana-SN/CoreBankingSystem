using CoreBankingSystem.Application.Accounts.Commands;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace CoreBankingSystem.Tests.Tests.Application.Accounts;

public class CreateAccountHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateAccountHandler _handler;

    public CreateAccountHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);
        _handler = new CreateAccountHandler(_accountRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        _accountRepositoryMock
            .Setup(r => r.ExistsByDocumentAsync("60014377004", default))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new CreateAccountCommand("João Silva", "600.143.770-04"), default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("João Silva");
        result.Value.Balance.Should().Be(1000m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsAddAndCommit()
    {
        _accountRepositoryMock
            .Setup(r => r.ExistsByDocumentAsync("60014377004", default))
            .ReturnsAsync(false);

        await _handler.Handle(new CreateAccountCommand("João Silva", "600.143.770-04"), default);

        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateDocument_ThrowsDuplicateDocumentException()
    {
        _accountRepositoryMock
            .Setup(r => r.ExistsByDocumentAsync("60014377004", default))
            .ReturnsAsync(true);

        var act = async () => await _handler.Handle(new CreateAccountCommand("João Silva", "600.143.770-04"), default);

        await act.Should().ThrowAsync<DuplicateDocumentException>();
    }

    [Fact]
    public async Task Handle_EmptyName_ReturnsFailure()
    {
        var result = await _handler.Handle(new CreateAccountCommand("", "600.143.770-04"), default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_EmptyDocument_ReturnsFailure()
    {
        var result = await _handler.Handle(new CreateAccountCommand("João Silva", ""), default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_InvalidCpf_ThrowsInvalidCpfException()
    {
        var act = async () => await _handler.Handle(new CreateAccountCommand("João Silva", "111.111.111-11"), default);

        await act.Should().ThrowAsync<InvalidCpfException>();
    }
}
