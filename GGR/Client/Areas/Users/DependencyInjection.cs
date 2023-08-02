using GGR.Client.Areas.Users.Services.Contract;
using GGR.Client.Areas.Users.Services;

namespace GGR.Client.Areas.Users;

public static class DependencyInjection
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserClientService, UserClientService>();
    }
}
