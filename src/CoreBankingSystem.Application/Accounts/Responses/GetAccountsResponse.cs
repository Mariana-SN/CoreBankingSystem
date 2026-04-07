namespace CoreBankingSystem.Application.Accounts.Responses;

public record GetAccountsResponse(
    Guid Id,
    string Name,
    string Document,
    decimal Balance,
    DateTime OpenedAt,
    bool IsActive
);