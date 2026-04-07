namespace CoreBankingSystem.Application.Transfers.Responses;

public record CreateTransferResponse(
    Guid TransferId,
    string OriginDocument,
    string DestinationDocument,
    decimal Amount,
    DateTime OccurredAt
);