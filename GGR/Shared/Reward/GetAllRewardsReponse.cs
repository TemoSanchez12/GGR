using GGR.Shared.Reward.Definitions;

namespace GGR.Shared.Reward;

public class GetAllRewardsReponse
{
    public List<RewardDefinition> Rewards { get; set; } = new List<RewardDefinition>();
}
