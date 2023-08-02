namespace GGR.Server.Data.Models;

public class SaleTicket
{
    public Guid Id { get; set; }
    public User User { get; set; } = new User();
    public decimal Amount { get; set; }
    public int Points { get; set; }
    public decimal Liters { get; set; }
}
