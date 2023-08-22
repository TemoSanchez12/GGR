using GGR.Shared;
using GGR.Shared.RewardClaim;

namespace GGR.Client.Areas.RewardClaim.Services.Contracts;

public interface IRewardClaimClientService
{
    Task<ServiceResponse<GetAllRewardClaimsResponse>> GetAllRewardClaims();
    Task<ServiceResponse<GetAllRewardClaimsResponse>> GetRewardClaimsByEmail(string email);
    Task<ServiceResponse<UpdateRewardClaimStatusResponse>> UpdateRewardClaimStatus(UpdateRewardClaimStatusRequest request);
}
