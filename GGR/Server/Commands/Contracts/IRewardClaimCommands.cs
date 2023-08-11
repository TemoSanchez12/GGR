using GGR.Server.Data.Models;
using GGR.Shared.RewardClaim;

namespace GGR.Server.Commands.Contracts;

public interface IRewardClaimCommands
{
    Task<List<RewardClaim>> GetAllRewardClaims();
    Task<List<RewardClaim>> GetRewardClaimsByUserEmail(string? email);
    Task<RewardClaim> CreateRewardClaim(CreateRewardClaimRequest request);
    Task<RewardClaim> UpdateRewardClaimStatus(UpdateRewardClaimStatusRequest request);
    Task<RewardClaim> UpdateRewardClaimStatusAdmin(UpdateRewardClaimStatusRequest request);
}
