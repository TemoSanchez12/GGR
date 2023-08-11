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

    public async Task<SaleTicket> RegisterSaleTicket(RegisterSaleTicketRequest request)
    {
        _logger.LogInformation("Register sale ticket for user {UserEmail}", request.UserEmail);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.UserEmail);

        if ( user == null )
            throw new Exception(SaleTicketsError.UserNotFoundWithEmail.ToString());

        // Fetching ticket information for register ticket
        var saleTicket = new SaleTicket
        {
            Id = Guid.NewGuid(),
            Amount = 350,
            Liters = 17,
            User = user,
        };

        saleTicket.Points = Decimal.ToInt32(saleTicket.Amount / 10);

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
}
