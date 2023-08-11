
using GGR.Shared.SaleTicket.Definitions;

namespace GGR.Shared.SaleTicket;

public class GetSaleTicketsResponse
{
    public List<SaleTicketDefinition> Tickets { get; set; } = new List<SaleTicketDefinition>();
}
