using CoreBankingSystem.Domain.Common;
using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Infrastructure.Data;

public class CoreBankingSystemDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator? _mediator;

    public CoreBankingSystemDbContext(DbContextOptions<CoreBankingSystemDbContext> options, IMediator? mediator = null)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await SaveChangesAsync(ct);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await base.SaveChangesAsync(ct);
        await DispatchDomainEventsAsync(ct);
        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        if (_mediator is null) 
            return;

        while (true)
        {
            var aggregates = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            if (aggregates.Count == 0) 
                    break;

            var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
            aggregates.ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in events)
                await _mediator.Publish(domainEvent, ct);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreBankingSystemDbContext).Assembly);
    }
}
