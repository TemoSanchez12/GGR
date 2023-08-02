using GGR.Server.Commands;
using GGR.Server.Commands.Contracts;

namespace GGR.Server;

public static class DependencyInjection
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        // Commands
        services.AddSingleton<IUserCommands, UserCommands>();
    }
}
