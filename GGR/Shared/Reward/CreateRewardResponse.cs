
using GGR.Shared.Reward.Definitions;

namespace GGR.Shared.Reward;

public class CreateRewardResponse
{
    public RewardDefinition Reward { get; set; } = new();
}
