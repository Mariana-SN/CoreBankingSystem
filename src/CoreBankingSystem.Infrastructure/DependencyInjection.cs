using CoreBankingSystem.Domain.Interfaces;
using CoreBankingSystem.Domain.Services;
using CoreBankingSystem.Infrastructure.Data;
using CoreBankingSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBankingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CoreBankingSystemDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CoreBankingSystemDbContext>());
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ITransferRepository, TransferRepository>();
        services.AddScoped<ITransferDomainService, TransferDomainService>();

        return services;
    }
}