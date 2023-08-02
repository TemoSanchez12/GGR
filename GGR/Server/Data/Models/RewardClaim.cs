using GGR.Server.Data.Models.Utils;

namespace GGR.Server.Data.Models;

public class RewardClaim
{
    public Guid Id { get; set; }
    public User User { get; set; } = new();
    public Reward Reward { get; set; } = new();
    public RewardClaimStatus RewardClaimStatus { get; set; }
    public DateTime ClaimCreated { get; set; }
    public DateTime ClaimUpdated { get; set; }
}
