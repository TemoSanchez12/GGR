using GGR.Server.Data.Models.Utils;

namespace GGR.Server.Data.Models;

public class Reward
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PricePoints { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public int UnitsAvailable { get; set; }
    public RewardStatus Status { get; set; }
}
