using GGR.Client.Areas.Rewards.Models;
using GGR.Shared.Reward.Definitions;

namespace GGR.Client.Areas.Rewards.Utils;

public static class RewardMapper
{
    public static Reward MapToEntity(RewardDefinition definition)
    {
        return new Reward
        {
            Id = definition.Id,
            Description = definition.Description,
            Name = definition.Name,
            PhotoUrl = definition.PhotoUrl,
            PricePoints = definition.PricePoints,
            UnitsAvailable = definition.UnitsAvailable,
            Status = definition.Status == RewardStatusDefinition.Available ? RewardStatus.Available : RewardStatus.NotAvailable
        };
    }
}
