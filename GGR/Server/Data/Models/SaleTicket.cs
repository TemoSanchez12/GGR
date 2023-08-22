using GGR.Server.Data.Models.Utils;
using GGR.Shared.SaleTicket.Definitions;

namespace GGR.Server.Data.Models;

public class SaleTicket
{
    public Guid Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public User User { get; set; } = new User();
    public decimal Amount { get; set; }
    public int Points { get; set; }
    public decimal Liters { get; set; }
    public SaleTicketStatus Status { get; set; } = SaleTicketStatus.Unchecked;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public SaleTicketDefinition ToDefinition()
    {
        return new SaleTicketDefinition
        {
            Id = Id,
            Folio = Folio,
            Amount = Amount,
            Points = Points,
            Liters = Liters,
            UserId = User.Id,
            UserEmail = User.Email,
        };
    }
}
