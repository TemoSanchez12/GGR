using GGR.Shared.Reward.Definitions;

namespace GGR.Client.Areas.Rewards.Models;

public class Reward
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PricePoints { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public int UnitsAvailable { get; set; }
    public RewardStatus Status { get; set; }

    public RewardDefinition ToDefinition()
    {
        return new RewardDefinition
        {
            Id = Id,
            Name = Name,
            Description = Description,
            PricePoints = PricePoints,
            PhotoUrl = PhotoUrl,
            UnitsAvailable = UnitsAvailable,
            Status = Status switch
            {
                RewardStatus.Available => RewardStatusDefinition.Available,
                RewardStatus.NotAvailable => RewardStatusDefinition.NotAvailable,
                _ => RewardStatusDefinition.NotAvailable,
            }
        };
    }
}

