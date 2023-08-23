using GGR.Server.Commands;
using GGR.Server.Commands.Contracts;
using GGR.Server.Infrastructure;
using GGR.Server.Infrastructure.Contracts;
using Quartz;

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


        // Background services register
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            var fileRecordJobKey = JobKey.Create(nameof(BackgroundFileRecordService));

            options.AddJob<BackgroundFileRecordService>(fileRecordJobKey)
                .AddTrigger(trigger => trigger.ForJob(fileRecordJobKey)
                .WithCronSchedule("* * 23 ? * * *"));

        });


        services.AddQuartzHostedService();
    }
}
