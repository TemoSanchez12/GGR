
using GGR.Shared.SaleTicket.Definitions;

namespace GGR.Shared.SaleTicket;

public class RegisterTicketResponse
{
    public SaleTicketDefinition SaleTicket { get; set; } = new SaleTicketDefinition();
}
