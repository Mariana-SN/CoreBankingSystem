using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBankingSystem.Infrastructure.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly CoreBankingSystemDbContext _context;

    public TransferRepository(CoreBankingSystemDbContext context) => _context = context;

    public Task AddAsync(Transfer transfer, CancellationToken ct = default)
    {
        _context.Transfers.Add(transfer);
        return Task.CompletedTask;
    }
}