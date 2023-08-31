
using GGR.Shared.Reward.Definitions;

namespace GGR.Shared.RewardClaim.Definitions;

public class RewardClaimDefinition
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } = new();
    public string UserEmail { get; set; } = string.Empty;
    public RewardDefinition Reward { get; set; } = new();
    public RewardClaimStatusDefinition RewardClaimStatus { get; set; }
    public DateTime ClaimCreated { get; set; }
    public DateTime ClaimUpdated { get; set; }
}
