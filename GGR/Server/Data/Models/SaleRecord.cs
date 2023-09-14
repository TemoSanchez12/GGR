namespace GGR.Server.Data.Models;

public class SaleRecord
{
    public Guid Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public decimal Amount { get; set; } = 0;
    public decimal Liters { get; set; } = 0;
    public string Product { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
}
