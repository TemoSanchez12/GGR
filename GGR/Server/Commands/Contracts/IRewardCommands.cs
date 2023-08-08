using GGR.Server.Data.Models;
using GGR.Shared.Reward;

namespace GGR.Server.Commands.Contracts;

public interface IRewardCommands
{
    Task<List<Reward>> GetAllRewards();
    Task<Reward> GetReward(Guid rewardId);
    Task<Reward> CreateReward(CreateRewardRequest request);
    Task<Reward> UpdateReward(UpdateRewardRequest request);
    Task DeleteReward(DeleteRewardRequest request);
}
