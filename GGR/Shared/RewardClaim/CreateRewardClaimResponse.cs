using GGR.Shared.RewardClaim.Definitions;

namespace GGR.Shared.RewardClaim;

public class CreateRewardClaimResponse
{
    public RewardClaimDefinition RewardClaim { get; set; } = new();
}
