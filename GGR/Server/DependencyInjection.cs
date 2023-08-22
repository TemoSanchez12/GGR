using GGR.Server.Commands;
using GGR.Server.Commands.Contracts;

namespace GGR.Server;

public static class DependencyInjection
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        // Commands
        services.AddScoped<IUserCommands, UserCommands>();
        services.AddScoped<IRewardCommands, RewardCommands>();
        services.AddScoped<IRewardClaimCommands, RewardClaimCommands>();
        services.AddScoped<ISaleTicketCommands, SaleTicketCommands>();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddScoped<IFileRecordCommands, FileRecordCommands>();
    }
}
