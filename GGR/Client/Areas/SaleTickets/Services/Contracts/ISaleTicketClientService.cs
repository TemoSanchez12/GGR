using GGR.Shared;
using GGR.Shared.SaleTicket;

namespace GGR.Client.Areas.SaleTickets.Services.Contracts;

public interface ISaleTicketClientService
{
    Task<ServiceResponse<GetSaleTicketsResponse>> GetSaleTicketsByUserEmail(string email);
    Task<ServiceResponse<GetTotalTicketsCount>> GetTotalTicketsCount();
}
