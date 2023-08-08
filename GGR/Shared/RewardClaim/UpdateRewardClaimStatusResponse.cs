
using GGR.Shared.RewardClaim.Definitions;

namespace GGR.Shared.RewardClaim;

public class UpdateRewardClaimStatusResponse
{
    public RewardClaimDefinition RewardClaim { get; set; } = new RewardClaimDefinition();
}
