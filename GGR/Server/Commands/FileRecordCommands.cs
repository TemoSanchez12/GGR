using GGR.Server.Data;
using System.Net;
using GGR.Server.Commands.Contracts;
using GGR.Server.Errors;
using Microsoft.EntityFrameworkCore;
using GGR.Server.Data.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace GGR.Server.Commands;

public class FileRecordCommands : IFileRecordCommands
{
    private readonly IWebHostEnvironment _env;
    private readonly IDbContextFactory<GlobalDbContext> _dbContextFactory;
    private readonly ILogger<FileRecordCommands> _logger;
    private readonly IEmailSender _emailSender;

    public FileRecordCommands(
        IWebHostEnvironment env,
        IDbContextFactory<GlobalDbContext> dbContextFactory,
        ILogger<FileRecordCommands> logger,
        IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _env = env;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<(string, string)> UploadFile(IFormFile file)
    {
        _logger.LogInformation("Uploading file {FileName} date: {UploadDate}", file.FileName, DateTime.Now.Date);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if (dbContext.FileRecords.Any(f => f.UploadedOn.Date == DateTime.Now.Date))
        {
            _logger.LogWarning("File already uploaded today");
            throw new Exception(FileRecordError.FileAlreadyUploadedToday.ToString());
        }

        var trustedFileNameForFileStorage = $"{DateTime.Now.ToString("dd-MM-yyyy")}.{file.ContentType.Split('/')[1]}";
        var untrustedFileName = file.FileName;
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
        var path = Path.Combine(_env.ContentRootPath, "FileRecords", trustedFileNameForFileStorage);

        var recordId = Guid.NewGuid();
        var fileRecord = new FileRecord
        {
            Id = recordId,
            FileName = trustedFileNameForDisplay,
            FileStorageName = trustedFileNameForFileStorage,
            UploadedOn = DateTime.Now,
            key = path
        };

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        try
        {
            await dbContext.FileRecords.AddAsync(fileRecord);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file record to database");
            throw new Exception(FileRecordError.ErrorSavingFileRecordToDatabase.ToString());
        }

        try
        {
            await _emailSender.SendEmailAsync(null, $"Archivo de ventas cargado {DateTime.Now.ToString("dd-MM-yyyy")}", $"Se ha subido el archivo de ventas {DateTime.Now}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            throw new Exception(FileRecordError.ErrorSendingEmail.ToString());
        }

        return (trustedFileNameForDisplay, trustedFileNameForFileStorage);
    }

    public async Task<string> GetFileByDate(DateTime date)
    {
        _logger.LogInformation("Getting file by date {Date}", date);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var fileRecord = await dbContext.FileRecords.FirstOrDefaultAsync(f => f.UploadedOn.Date == date.Date);

        if (fileRecord == null)
        {
            _logger.LogWarning("File not found");
            throw new Exception(FileRecordError.FileNotFound.ToString());
        }

        return fileRecord.FileStorageName;
    }
}
