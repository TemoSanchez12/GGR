using GGR.Shared.Reward.Definitions;

namespace GGR.Shared.Reward;

public class GetRewardResponse
{
    public RewardDefinition Reward { get; set; } = new RewardDefinition();
}
