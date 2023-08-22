
namespace GGR.Shared.SaleTicket.Definitions;

public class SaleTicketDefinition
{
    public Guid Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Points { get; set; }
    public decimal Liters { get; set; }
}
