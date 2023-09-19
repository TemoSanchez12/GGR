namespace GGR.Client.Areas.SaleTickets.Models;

public class SaleTicket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Points { get; set; }
    public decimal Liters { get; set; }
    public string FolioDate { get; set; } = string.Empty;
}
