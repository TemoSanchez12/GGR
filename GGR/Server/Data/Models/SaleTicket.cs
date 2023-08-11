using GGR.Shared.SaleTicket.Definitions;

namespace GGR.Server.Data.Models;

public class SaleTicket
{
    public Guid Id { get; set; }
    public User User { get; set; } = new User();
    public decimal Amount { get; set; }
    public int Points { get; set; }
    public decimal Liters { get; set; }

    public SaleTicketDefinition ToDefinition()
    {
        return new SaleTicketDefinition
        {
            Id = Id,
            Amount = Amount,
            Points = Points,
            Liters = Liters,
            UserId = User.Id,
            UserEmail = User.Email,
        };
    }
}
