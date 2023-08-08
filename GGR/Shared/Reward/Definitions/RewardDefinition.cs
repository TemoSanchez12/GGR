namespace GGR.Shared.Reward.Definitions;

public class RewardDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PricePoints { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public int UnitsAvailable { get; set; }
    public RewardStatusDefinition Status { get; set; }
}
