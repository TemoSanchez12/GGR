using GGR.Shared.RewardClaim.Definitions;

namespace GGR.Shared.RewardClaim;

public class UpdateRewardClaimStatusRequest
{
    public string RewardClaimId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public RewardClaimStatusDefinition NewStatus { get; set; }
}
