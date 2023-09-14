using GGR.Server.Commands.Contracts;
using GGR.Server.Data;
using GGR.Server.Data.Models;
using GGR.Server.Data.Models.Utils;
using GGR.Shared.Reward;
using GGR.Server.Errors;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.IO;

namespace GGR.Server.Commands;

public class RewardCommands : IRewardCommands
{
    private readonly IDbContextFactory<GlobalDbContext> _dbContextFactory;
    private readonly ILogger<RewardCommands> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly Cloudinary _cloudinary;

    public RewardCommands(
        IDbContextFactory<GlobalDbContext> dbContextFactory,
        ILogger<RewardCommands> logger,
        IWebHostEnvironment env)
    {
        _cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _env = env;
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

        var fileName = $"{rewardId}.jpeg";
        var path = Path.Combine(_env.ContentRootPath, "../ImageRewards", fileName);

        var reward = new Reward
        {
            Id = rewardId,
            Name = request.Name,
            Description = request.Description,
            PricePoints = request.PricePoints,
            Status = request.IsActive ? RewardStatus.Available : RewardStatus.NotAvailable,
            UnitsAvailable = request.UnitsAvailable
        };

        using (var stream = new FileStream(path, FileMode.Create))
        {
            var bytes = Convert.FromBase64String(request.Base64Photo);
            var content = new StreamContent(new MemoryStream(bytes));
            await content.CopyToAsync(stream);
        }

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(path)
        };

        try
        {
            var uploadResult = _cloudinary.Upload(uploadParams);
            reward.PhotoUrl = uploadResult.Url.AbsoluteUri;

        } catch (Exception ex)
        {
            _logger.LogError("Something went wrong while saving reward {ErrorMessage}", ex.Message);
            throw new Exception(RewardError.SavingDataError.ToString());
        }


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

        if (!string.IsNullOrEmpty(request.Base64Photo))
        {
            var fileName = $"{reward.Id}.jpeg";
            var path = Path.Combine(_env.ContentRootPath, "out/ImageRewards", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                var bytes = Convert.FromBase64String(request.Base64Photo);
                var content = new StreamContent(new MemoryStream(bytes));
                await content.CopyToAsync(stream);
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(path)
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            reward.PhotoUrl = uploadResult.Url.AbsoluteUri;
        }

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

        File.Delete(reward.PhotoUrl);

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
