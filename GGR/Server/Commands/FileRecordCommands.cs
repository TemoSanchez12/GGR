using GGR.Server.Data;
using System.Net;
using GGR.Server.Commands.Contracts;
using GGR.Server.Errors;
using Microsoft.EntityFrameworkCore;
using GGR.Server.Data.Models;
using GGR.Server.Infrastructure.Contracts;
using GGR.Shared.FileRecord;
using Twilio.TwiML.Voice;

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

        if ( dbContext.FileRecords.Any(f => f.UploadedOn.Date == DateTime.UtcNow.Date) )
        {
            _logger.LogWarning("File already uploaded today");
            throw new Exception(FileRecordError.FileAlreadyUploadedToday.ToString());
        }

        if ( file.ContentType.Split('/')[1] != "csv" )
        {
            _logger.LogWarning("File is not a csv file");
            throw new Exception(FileRecordError.FileIsNotCsv.ToString());
        }


        var trustedFileNameForFileStorage = $"{Guid.NewGuid()}-{DateTime.Now.ToString("dd-MM-yyyy")}.{file.ContentType.Split('/')[1]}";
        var untrustedFileName = file.FileName;
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
        var path = Path.Combine(_env.ContentRootPath, "../FileRecords", trustedFileNameForFileStorage);

        var recordId = Guid.NewGuid();
        var fileRecord = new FileRecord
        {
            Id = recordId,
            FileName = trustedFileNameForDisplay,
            FileStorageName = trustedFileNameForFileStorage,
            UploadedOn = DateTime.UtcNow,
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

    public async Task<FileRecord> CheckTicketFromFile(Guid FileRecordId)
    {
        _logger.LogInformation($"CheckTickets from files {DateTime.UtcNow}");
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var fileRecord = await dbContext.FileRecords
            .FirstOrDefaultAsync(fileRecord => fileRecord.Id == FileRecordId);

        if ( fileRecord == null )
            throw new Exception(FileRecordError.FileNotFound.ToString());

        var saleRecords = new List<SaleRecord>();

        var readCsv = File.ReadAllText(fileRecord.key);
        var csvFileRecord = readCsv.Split("\n").ToList();
        csvFileRecord.RemoveAt(0);

        foreach ( var row in csvFileRecord )
        {
            if ( !string.IsNullOrEmpty(row) )
            {
                var cells = row.Split(',');

                _logger.LogInformation("Saving file record for ticket {Folio}", cells[1]);

                var saleRecord = new SaleRecord
                {
                    Id = Guid.NewGuid(),
                    Amount = Math.Floor(Decimal.Parse(cells[8].Replace("\"", ""))),
                    Folio = cells[14],
                    Liters = Math.Floor(Decimal.Parse(cells[7])),
                    Product = cells[6],
                    StartDate = cells[1] + cells[2]
                };

                saleRecords.Add(saleRecord);
            }
        }

        fileRecord.IsProcessed = true;

        await dbContext.SaleRecords.AddRangeAsync(saleRecords);
        await dbContext.SaveChangesAsync();

        var saleTicketsToRemove = await dbContext.SaleTickets
            .Where(ticket => ticket.Status == Data.Models.Utils.SaleTicketStatus.Unchecked
            && ticket.CreatedAt.AddDays(3) < DateTime.UtcNow).ToListAsync();

        dbContext.SaleTickets.RemoveRange(saleTicketsToRemove);

        var saleTickets = await dbContext.SaleTickets.Include(ticket => ticket.User)
            .Where(ticket => ticket.Status == Data.Models.Utils.SaleTicketStatus.Unchecked
            && ticket.CreatedAt.AddDays(3) > DateTime.UtcNow).ToListAsync();

        foreach ( var ticket in saleTickets )
        {
            _logger.LogInformation("Register information for ticket folio {Folio} for client {UserId}",
                ticket.Folio, ticket.User.Id);

            var saleRecord = await dbContext.SaleRecords
                .FirstOrDefaultAsync(record => record.Folio == ticket.Folio.Remove(0, 1) || record.Folio == ticket.Folio || record.Folio.Contains(ticket.Folio));

            Console.WriteLine("Comparing: " + ticket.Folio);

            if ( saleRecord == null )
                continue;

            Console.WriteLine("Comparing: " + ticket.Folio + saleRecord.Folio);

            if ( !saleRecord.StartDate.Contains(ticket.HourAndMinutesRegister) )
                continue;

            ticket.Points = (int) (saleRecord.Product.Contains("87") ? saleRecord.Liters * 10 : saleRecord.Liters * 15);
            ticket.Amount = saleRecord.Amount;
            ticket.Liters = saleRecord.Liters;
            ticket.Status = Data.Models.Utils.SaleTicketStatus.Checked;

            ticket.User.Points += ticket.Points;
        }

        dbContext.SaleRecords.RemoveRange(dbContext.SaleRecords);
        await dbContext.SaveChangesAsync();
        return fileRecord;
    }

    public async Task<(string, string)> UploadFileRecord(UploadFileRecordRequest request)
    {
        _logger.LogInformation("Uploading file date: {UploadDate}", DateTime.Now.Date);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if ( request.DateForRecord > DateTime.UtcNow )
            throw new Exception(FileRecordError.FileDatePassToday.ToString());

        var trustedFileNameForFileStorage = $"{Guid.NewGuid()}-{request.DateForRecord.ToString("dd-MM-yyyy")}.csv";
        var untrustedFileName = request.DateForRecord.ToString("dd-MM-yyyy");
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
        var path = Path.Combine(_env.ContentRootPath, "../FileRecords", trustedFileNameForFileStorage);

        var recordId = Guid.NewGuid();
        var fileRecord = new FileRecord
        {
            Id = recordId,
            FileName = trustedFileNameForDisplay,
            FileStorageName = trustedFileNameForFileStorage,
            UploadedOn = DateTime.UtcNow,
            key = path,
            IsProcessed = false
        };

        await using var stream = new FileStream(path, FileMode.Create);
        var bytes = Convert.FromBase64String(request.FileContentBase64);
        var content = new StreamContent(new MemoryStream(bytes));
        await content.CopyToAsync(stream);

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

    public async Task<List<FileRecord>> GetFileRecordsWithoutProcessing()
    {
        _logger.LogInformation("Fetching files records without processing: {Date}", DateTime.Now.Date);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var fileRecords = await dbContext.FileRecords.Where(f => !f.IsProcessed).ToListAsync();

        return fileRecords;
    }

    public async Task CheckTicketsFromFiles()
    {
        _logger.LogInformation($"CheckTickets from files records in date  {DateTime.Now}");
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var fileRecordsWithoutProcessing = await dbContext.FileRecords
            .Where(f => !f.IsProcessed).ToArrayAsync();

        foreach ( var fileRecord in fileRecordsWithoutProcessing )
        {
            _logger.LogInformation("Checking tickets from file {FileId} {FileName}", fileRecord.Id, fileRecord.FileName);
            await CheckTicketFromFile(fileRecord.Id);
        }
    }
}
