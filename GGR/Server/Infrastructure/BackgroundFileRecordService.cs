using GGR.Server.Commands.Contracts;
using GGR.Server.Infrastructure.Contracts;
using Quartz;

namespace GGR.Server.Infrastructure;

[DisallowConcurrentExecution]
public class BackgroundFileRecordService : IBackgroundFileRecordService
{
    private readonly ILogger<BackgroundFileRecordService> _logger;
    private readonly IFileRecordCommands _fileRecordCommands;

    public BackgroundFileRecordService(
        ILogger<BackgroundFileRecordService> logger,
        IFileRecordCommands fileRecordCommands)
    {
        _fileRecordCommands = fileRecordCommands;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Reading file records to fill sale tickets {UtcNow}", DateTime.UtcNow);
        await _fileRecordCommands.CheckTicketsFromFiles();
    }
}
