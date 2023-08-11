using GGR.Server.Data.Models;
using GGR.Shared.SaleTicket;

namespace GGR.Server.Commands.Contracts;

public interface ISaleTicketCommands
{
    Task<List<SaleTicket>> GetTicketsByEmail(string email);
    Task<SaleTicket> RegisterSaleTicket(RegisterSaleTicketRequest request);
}
