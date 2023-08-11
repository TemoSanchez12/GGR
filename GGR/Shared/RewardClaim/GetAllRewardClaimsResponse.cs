
using GGR.Shared.RewardClaim.Definitions;

namespace GGR.Shared.RewardClaim;

public class GetAllRewardClaimsResponse
{
    public IList<RewardClaimDefinition> RewardClaims { get; set; } = new List<RewardClaimDefinition>();
}
