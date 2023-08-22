using GGR.Server.Data.Models.Utils;
using GGR.Shared.RewardClaim.Definitions;

namespace GGR.Server.Data.Models;

public class RewardClaim
{
    public Guid Id { get; set; }
    public User User { get; set; } = new();
    public Reward Reward { get; set; } = new();
    public RewardClaimStatus RewardClaimStatus { get; set; }
    public DateTime ClaimCreated { get; set; }
    public DateTime ClaimUpdated { get; set; }

    public RewardClaimDefinition ToDefinition()
    {
        return new RewardClaimDefinition
        {
            Id = Id,
            ClaimCreated = ClaimCreated,
            ClaimUpdated = ClaimUpdated,
            RewardId = Reward.Id,
            UserId = User.Id,
            UserEmail = User.Email,
            RewardName = Reward.Name,
            RewardClaimStatus = RewardClaimStatus switch
            {
                RewardClaimStatus.Unclaimed => RewardClaimStatusDefinition.Unclaimed,
                RewardClaimStatus.Cancelled => RewardClaimStatusDefinition.Cancelled,
                RewardClaimStatus.Claimed => RewardClaimStatusDefinition.Claimed,
                _ => RewardClaimStatusDefinition.Unclaimed,
            }
        };
    }
}
