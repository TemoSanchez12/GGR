using GGR.Client.Areas.Rewards.Services;
using GGR.Client.Areas.Users.Services;
using GGR.Client.Areas.RewardClaim.Services;
using GGR.Client.Areas.SaleTickets.Services;

using GGR.Client.Areas.Rewards.Services.Contracts;
using GGR.Client.Areas.Users.Services.Contract;
using GGR.Client.Areas.RewardClaim.Services.Contracts;
using GGR.Client.Areas.SaleTickets.Services.Contracts;

namespace GGR.Client.Areas;

public static class DependencyInjection
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserClientService, UserClientService>();
        services.AddScoped<IRewardClientService, RewardClientService>();
        services.AddScoped<IRewardClaimClientService, RewardClaimClientService>();
        services.AddScoped<ISaleTicketClientService, SaleTicketClientService>();
    }
}
