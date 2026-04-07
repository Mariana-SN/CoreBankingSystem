using CoreBankingSystem.Application.Common;
using CoreBankingSystem.Application.Transfers.Responses;
using MediatR;

namespace CoreBankingSystem.Application.Transfers.Commands;

public record CreateTransferCommand(
    string OriginDocument,
    string DestinationDocument,
    decimal Amount
) : IRequest<Result<CreateTransferResponse>>;