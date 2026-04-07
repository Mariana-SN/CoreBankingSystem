namespace CoreBankingSystem.Application.Accounts.Responses;

public record AccountDeactivationInfo(
    string ResponsibleUser,
    DateTime OccurredAt
);

public record AccountResponse(
    Guid Id,
    string Name,
    string Document,
    decimal Balance,
    DateTime OpenedAt,
    bool IsActive,
    AccountDeactivationInfo? Deactivation
);