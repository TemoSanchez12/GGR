using GGR.Server.Commands.Contracts;
using GGR.Server.Data;
using GGR.Server.Data.Models;
using GGR.Server.Errors;
using GGR.Shared.SaleTicket;
using Microsoft.EntityFrameworkCore;

namespace GGR.Server.Commands;

public class SaleTicketCommands : ISaleTicketCommands
{
    private readonly IDbContextFactory<GlobalDbContext> _dbContextFactory;
    private readonly ILogger<SaleTicketCommands> _logger;
    private readonly HttpClient _httpClient;

    public SaleTicketCommands(IDbContextFactory<GlobalDbContext> dbContextFactory, ILogger<SaleTicketCommands> logger, HttpClient httpClient)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<SaleTicket> RegisterTicket(RegisterTicketRequest request)
    {
        _logger.LogInformation("Registering ticket for user {UserEmail}", request.UserEmail);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if ( string.IsNullOrEmpty(request.UserEmail) )
            throw new Exception(SaleTicketsError.EmailIsNullOrEmpty.ToString());

        if ( string.IsNullOrEmpty(request.Folio) )
            throw new Exception(SaleTicketsError.FolioIsNullOrEmpty.ToString());

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.UserEmail);
        var registration = await dbContext.Registrations.FirstOrDefaultAsync(registration => registration.User == user);

        if ( user == null )
            throw new Exception(SaleTicketsError.UserNotFoundWithEmail.ToString());

        if ( await dbContext.SaleTickets.AnyAsync(ticket => ticket.Folio == request.Folio) )
            throw new Exception(SaleTicketsError.FolioAlreadyRegistered.ToString());

        if ( registration == null || registration.VerifiedAt == null )
            throw new Exception(SaleTicketsError.UserNotRegistered.ToString());

        var latesTicket = await dbContext.SaleTickets
            .OrderByDescending(ticket => ticket.CreatedAt)
            .FirstOrDefaultAsync(t => t.User == user);

        if ( latesTicket != null && latesTicket.CreatedAt.AddMinutes(30) > DateTime.Now )
            throw new Exception(SaleTicketsError.TimeSpanBetweenTicketRegisterNotReached.ToString());



        var saleTicketId = Guid.NewGuid();
        var saleTicket = new SaleTicket
        {
            Id = saleTicketId,
            Folio = request.Folio,
            User = user
        };

        dbContext.SaleTickets.Add(saleTicket);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while saving ticket: {ErrorMessage}", ex.Message);
            throw new Exception(SaleTicketsError.ErrorWhileSavingTicket.ToString());
        }
        return saleTicket;
    }

    public async Task<List<SaleTicket>> GetTicketsByEmail(string email)
    {
        _logger.LogInformation("Fetching tickets by user email {UserEmail}", email);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if ( string.IsNullOrEmpty(email) )
            throw new Exception(SaleTicketsError.EmailIsNullOrEmpty.ToString());

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        if ( user == null )
            throw new Exception(SaleTicketsError.UserNotFoundWithEmail.ToString());

        var saleTickets = dbContext.SaleTickets.Include(ticket => ticket.User).Where(ticket => ticket.User.Email == email);

        return saleTickets.ToList();
    }

    public async Task<int> GetTotalTicketsCount()
    {
        _logger.LogInformation("Fetching total tickets count {DateUtc}", DateTime.UtcNow);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var totalTicketsCount = await dbContext.SaleTickets
            .Where(ticket => ticket.Status == Data.Models.Utils.SaleTicketStatus.Checked).CountAsync();

        return totalTicketsCount;
    }
}
