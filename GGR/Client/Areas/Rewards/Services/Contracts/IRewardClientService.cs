using GGR.Shared;
using GGR.Shared.Reward;

namespace GGR.Client.Areas.Rewards.Services.Contracts;

public interface IRewardClientService
{
    Task<ServiceResponse<GetAllRewardsReponse>> GetAllRewards();
    Task<ServiceResponse<GetRewardResponse>> GetReward(string rewardId);
    Task<ServiceResponse<CreateRewardResponse>> CreateReward(CreateRewardRequest request);
    Task<ServiceResponse<UpdateRewardResponse>> UpdateReward(UpdateRewardRequest request);
    Task DeleteReward(DeleteRewardRequest request);
}
