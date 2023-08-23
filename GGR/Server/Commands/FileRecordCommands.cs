using GGR.Server.Data;
using System.Net;
using GGR.Server.Commands.Contracts;
using GGR.Server.Errors;
using Microsoft.EntityFrameworkCore;
using GGR.Server.Data.Models;
using GGR.Server.Infrastructure.Contracts;

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

        if ( dbContext.FileRecords.Any(f => f.UploadedOn.Date == DateTime.Now.Date) )
        {
            _logger.LogWarning("File already uploaded today");
            throw new Exception(FileRecordError.FileAlreadyUploadedToday.ToString());
        }

        if ( file.ContentType.Split('/')[1] != "csv" )
        {
            _logger.LogWarning("File is not a csv file");
            throw new Exception(FileRecordError.FileIsNotCsv.ToString());
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
            key = path,
            IsProcessed = false
        };

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        try
        {
            await dbContext.FileRecords.AddAsync(fileRecord);
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Error saving file record to database");
            throw new Exception(FileRecordError.ErrorSavingFileRecordToDatabase.ToString());
        }

        try
        {
            await _emailSender.SendEmailAsync(null, $"Archivo de ventas cargado {DateTime.Now.ToString("dd-MM-yyyy")}", $"Se ha subido el archivo de ventas {DateTime.Now}");
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Error sending email");
            throw new Exception(FileRecordError.ErrorSendingEmail.ToString());
        }

        return (trustedFileNameForDisplay, trustedFileNameForFileStorage);
    }

    public async Task<string?> GetFileByDate(DateTime date)
    {
        _logger.LogInformation("Getting file by date {Date}", date);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var fileRecord = await dbContext.FileRecords.FirstOrDefaultAsync(f => f.UploadedOn.Date == date.Date);

        return fileRecord?.FileStorageName;
    }

    public async Task CheckTicketsFromFiles()
    {
        _logger.LogInformation($"CheckTickets from files {DateTime.UtcNow}");
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var fileRecords = await dbContext.FileRecords
            .Where(fileRecord => !fileRecord.IsProcessed).ToListAsync();

        var saleRecords = new List<SaleRecord>();

        foreach ( var fileRecord in fileRecords )
        {
            var readCsv = File.ReadAllText(fileRecord.key);
            var csvFileRecord = readCsv.Split("\n").ToList();
            csvFileRecord.RemoveAt(0);

            foreach ( var row in csvFileRecord )
            {
                Console.Write(row + "aqui mero");
                if ( !string.IsNullOrEmpty(row) )
                {
                    var cells = row.Split(',');
                    _logger.LogInformation("Saving file record for ticket {Folio}", cells[0]);

                    var saleRecord = new SaleRecord
                    {
                        Id = Guid.NewGuid(),
                        Amount = Decimal.Parse(cells[1]),
                        Folio = cells[0]
                    };

                    saleRecords.Add(saleRecord);
                }
            }

            fileRecord.IsProcessed = true;
        }

        await dbContext.SaleRecords.AddRangeAsync(saleRecords);
        await dbContext.SaveChangesAsync();

        var saleTicketsToRemove = await dbContext.SaleTickets
            .Where(ticket => ticket.Status == Data.Models.Utils.SaleTicketStatus.Unchecked
            && ticket.CreatedAt.AddDays(3) < DateTime.Now).ToListAsync();

        dbContext.SaleTickets.RemoveRange(saleTicketsToRemove);

        var saleTickets = await dbContext.SaleTickets.Include(ticket => ticket.User)
            .Where(ticket => ticket.Status == Data.Models.Utils.SaleTicketStatus.Unchecked
            && ticket.CreatedAt.AddDays(3) > DateTime.Now).ToListAsync();

        foreach ( var ticket in saleTickets )
        {
            _logger.LogInformation("Register information for ticket folio {Folio} for client {UserId}",
                ticket.Folio, ticket.User.Id);

            var saleRecord = await dbContext.SaleRecords
                .FirstOrDefaultAsync(record => record.Folio == ticket.Folio);

            if ( saleRecord == null )
                continue;

            ticket.Points = (int) saleRecord.Amount * 20;
            ticket.Amount = saleRecord.Amount;
            ticket.Liters = saleRecord.Amount / 20;
            ticket.Status = Data.Models.Utils.SaleTicketStatus.Checked;

            ticket.User.Points = ticket.Points;
        }

        dbContext.SaleRecords.RemoveRange(dbContext.SaleRecords);
        await dbContext.SaveChangesAsync();
    }
}
