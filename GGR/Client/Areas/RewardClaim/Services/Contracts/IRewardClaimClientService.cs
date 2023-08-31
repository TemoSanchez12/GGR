using GGR.Shared;
using GGR.Shared.RewardClaim;

namespace GGR.Client.Areas.RewardClaim.Services.Contracts;

public interface IRewardClaimClientService
{
    Task<ServiceResponse<GetAllRewardClaimsResponse>> GetAllRewardClaims();
    Task<ServiceResponse<GetAllRewardClaimsResponse>> GetRewardClaimsByEmail(string email);
    Task<ServiceResponse<GetAllRewardClaimsResponse>> GetRewardClaimsById(string id);
    Task<ServiceResponse<UpdateRewardClaimStatusResponse>> UpdateRewardClaimStatus(UpdateRewardClaimStatusRequest request);
    Task<ServiceResponse<CreateRewardClaimResponse>> CreateRewardClaim(CreateRewardClaimRequest request);
}
