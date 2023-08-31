using GGR.Client.Areas.RewardClaim.Models;
using GGR.Shared.RewardClaim.Definitions;
using GGR.Client.Areas.Rewards.Utils;

namespace GGR.Client.Areas.RewardClaim.Utils;

public static class RewardClaimMapper
{
    public static Models.RewardClaim MapToEntity(RewardClaimDefinition definition)
    {
        var rewardClaimStatus = definition.RewardClaimStatus switch
        {
            RewardClaimStatusDefinition.Unclaimed => RewardClaimStatus.Unclaimed,
            RewardClaimStatusDefinition.Claimed => RewardClaimStatus.Claimed,
            RewardClaimStatusDefinition.Cancelled => RewardClaimStatus.Cancelled,
            _ => throw new NotImplementedException(),
        };

        return new Models.RewardClaim
        {
            Id = definition.Id,
            UserId = definition.UserId,
            UserEmail = definition.UserEmail,
            Reward = RewardMapper.MapToEntity(definition.Reward),
            RewardClaimStatus = rewardClaimStatus,
            ClaimCreated = definition.ClaimCreated,
            ClaimUpdated = definition.ClaimUpdated,
        };
    }
}
