using GGR.Client.Areas.Rewards.Models;

namespace GGR.Client.Areas.RewardClaim.Models;

public class RewardClaim
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public Reward Reward { get; set; } = new Reward();
    public RewardClaimStatus RewardClaimStatus { get; set; }
    public DateTime ClaimCreated { get; set; }
    public DateTime ClaimUpdated { get; set; }
}
