using GGR.Server.Commands.Contracts;
using GGR.Server.Data;
using GGR.Server.Data.Models;
using GGR.Server.Data.Models.Utils;
using GGR.Shared.Reward;
using GGR.Server.Errors;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace GGR.Server.Commands;

public class RewardCommands : IRewardCommands
{
    private readonly IDbContextFactory<GlobalDbContext> _dbContextFactory;
    private readonly ILogger<RewardCommands> _logger;

    public RewardCommands(IDbContextFactory<GlobalDbContext> dbContextFactory, ILogger<RewardCommands> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<List<Reward>> GetAllRewards()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var rewards = await dbContext.Rewards.Where(r => !string.IsNullOrWhiteSpace(r.Name)).ToListAsync();
        return rewards;
    }

    public async Task<Reward> GetReward(Guid rewardId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var reward = await dbContext.Rewards.FirstOrDefaultAsync(r => r.Id == rewardId && r.Name != null);

        if (reward == null)
            throw new Exception(RewardError.RewardNotFound.ToString());

        return reward;
    }


    public async Task<Reward> CreateReward(CreateRewardRequest request)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if (dbContext.Rewards.Any(reward => reward.Name == request.Name))
            throw new Exception(RewardError.RewardAlreadyExists.ToString());

        var rewardId = Guid.NewGuid();
        var reward = new Reward
        {
            Id = rewardId,
            Name = request.Name,
            Description = request.Description,
            PricePoints = request.PricePoints,
            Status = request.IsActive ? RewardStatus.Available : RewardStatus.NotAvailable,
            UnitsAvailable = request.UnitsAvailable
        };

        try
        {
            dbContext.Rewards.Add(reward);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong while saving reward {ErrorMessage}", ex.Message);
            throw new Exception(RewardError.SavingDataError.ToString());
        }

        return reward;
    }


    public async Task<Reward> UpdateReward(UpdateRewardRequest request)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var reward = await dbContext.Rewards
            .FirstOrDefaultAsync(reward => reward.Id == Guid.Parse(request.RewardId));

        if (reward == null)
            throw new Exception(RewardError.RewardNotFound.ToString());

        reward.Name = request.Name;
        reward.Description = request.Description;
        reward.PricePoints = request.PricePoints;
        reward.Status = request.IsActive ? RewardStatus.Available : RewardStatus.NotAvailable;
        reward.UnitsAvailable = request.UnitsAvailable;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong while saving the update reward: {ErrorMessage}", ex.Message);
            throw new Exception(RewardError.SavingDataError.ToString());
        }
        return reward;
    }

    public async Task DeleteReward(DeleteRewardRequest request)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var reward = await dbContext.Rewards
          .FirstOrDefaultAsync(reward => reward.Id == Guid.Parse(request.RewardId));

        if (reward == null)
            throw new Exception(RewardError.RewardNotFound.ToString());

        dbContext.Rewards.Remove(reward);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong while removing reward {ErrorMessage}", ex.Message);
            throw new Exception(RewardError.SavingDataError.ToString());
        }
    }
}
