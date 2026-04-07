using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBankingSystem.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly CoreBankingSystemDbContext _context;

    public AccountRepository(CoreBankingSystemDbContext context) => _context = context;

    public Task<Account?> GetByDocumentAsync(string document, CancellationToken ct = default) =>
        _context.Accounts
            .FirstOrDefaultAsync(a => a.Document == document.Trim(), ct);

    public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Accounts.OrderBy(a => a.Name).ToListAsync(ct);

    public async Task<IEnumerable<Account>> SearchAsync(string? name, string? document, CancellationToken ct = default)
    {
        var query = _context.Accounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{name.Trim()}%"));

        if (!string.IsNullOrWhiteSpace(document))
            query = query.Where(a => a.Document == document.Trim());

        return await query.OrderBy(a => a.Name).ToListAsync(ct);
    }

    public Task AddAsync(Account account, CancellationToken ct = default)
    {
        _context.Accounts.Add(account);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        _context.Accounts.Update(account);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct = default) =>
        _context.Accounts.AnyAsync(a => a.Document == document.Trim(), ct);
}