using GGR.Shared.SaleTicket.Definitions;
using GGR.Client.Areas.SaleTickets.Models;

namespace GGR.Client.Areas.SaleTickets.Utils;

public static class SaleTicketMapper
{
    public static SaleTicket MapToEntity(SaleTicketDefinition definition)
    {
        return new SaleTicket
        {
            Id = definition.Id,
            Amount = definition.Amount,
            Liters = definition.Liters,
            Points = definition.Points,
            UserEmail = definition.UserEmail,
            UserId = definition.UserId
        };
    }
}
