using GGR.Client.Areas.Rewards.Services.Contracts;
using GGR.Client.Areas.Rewards.Services;

namespace GGR.Client.Areas.Rewards;

public static class DependencyInjection
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRewardClientService, RewardClientService>();
    }
}
