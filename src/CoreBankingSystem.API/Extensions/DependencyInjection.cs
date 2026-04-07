using CoreBankingSystem.Application.Accounts.Commands;

namespace CoreBankingSystem.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateAccountHandler).Assembly));

        return services;
    }
}