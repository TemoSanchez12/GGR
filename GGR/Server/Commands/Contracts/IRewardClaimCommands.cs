using GGR.Server.Data.Models;
using GGR.Shared.RewardClaim;

namespace GGR.Server.Commands.Contracts;

public interface IRewardClaimCommands
{
    Task<RewardClaim> CreateRewardClaim(CreateRewardClaimRequest request);
    Task<RewardClaim> UpdateRewardClaimStatus(UpdateRewardClaimStatusRequest request);
    Task<RewardClaim> UpdateRewardClaimStatusAdmin(UpdateRewardClaimStatusRequest request);
}
