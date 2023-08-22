namespace GGR.Server.Data.Models;

public class SaleRecord
{
    public Guid Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public decimal Amount { get; set; } = 0;
}
