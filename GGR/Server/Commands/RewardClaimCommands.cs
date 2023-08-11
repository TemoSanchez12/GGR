using GGR.Server.Commands.Contracts;
using GGR.Server.Data;
using GGR.Server.Data.Models;
using GGR.Shared.RewardClaim;
using Microsoft.EntityFrameworkCore;
using GGR.Server.Errors;
using GGR.Server.Data.Models.Utils;
using GGR.Shared.RewardClaim.Definitions;

namespace GGR.Server.Commands;

public class RewardClaimCommands : IRewardClaimCommands
{
    private readonly IDbContextFactory<GlobalDbContext> _dbContextFactory;
    private readonly ILogger<RewardClaimCommands> _logger;

    public RewardClaimCommands(
        IDbContextFactory<GlobalDbContext> dbContextFactory,
        ILogger<RewardClaimCommands> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<List<RewardClaim>> GetAllRewardClaims()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return dbContext.RewardClaims.Include(r => r.User).Include(r => r.Reward).ToList();
    }

    public async Task<List<RewardClaim>> GetRewardClaimsByUserEmail(string? email)
    {
        _logger.LogInformation("Fetching all reward claims for user {Email}", email);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if ( email == null )
            throw new Exception();

        var rewardClaims = dbContext.RewardClaims.Include(r => r.User)
            .Include(r => r.Reward).Where(rewardClaim => rewardClaim.User.Email == email);

        return rewardClaims.ToList();
    }

    public async Task<RewardClaim> CreateRewardClaim(CreateRewardClaimRequest request)
    {
        _logger.LogInformation("Creating reward claim for user {UserId} for {RewardId}", request.UserId, request.RewardClaimId);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(request.UserId));
        if ( user == null )
            throw new Exception(RewardClaimError.UserNotFound.ToString());

        var userRegistration = await dbContext.Registrations.FirstOrDefaultAsync(r => r.User == user);
        if ( userRegistration == null || userRegistration.VerifiedAt == null )
            throw new Exception(RewardClaimError.UserNotVerified.ToString());

        var reward = await dbContext.Rewards.FirstOrDefaultAsync();
        if ( reward == null )
            throw new Exception(RewardClaimError.RewardNotFound.ToString());

        if ( user.Points < reward.PricePoints )
            throw new Exception(RewardClaimError.NotEnoughPoints.ToString());

        var rewardClaimId = Guid.NewGuid();
        var rewardClaim = new RewardClaim()
        {
            Id = rewardClaimId,
            User = user,
            Reward = reward,
            RewardClaimStatus = RewardClaimStatus.Unclaimed,
            ClaimCreated = DateTime.UtcNow,
        };

        user.Points -= reward.PricePoints;

        try
        {
            await dbContext.AddAsync(rewardClaim);
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while saving reward claim {ErrorMessage}", ex.Message);
            throw new Exception(RewardClaimError.ErrorSavingData.ToString());
        }

        return rewardClaim;
    }

    public async Task<RewardClaim> UpdateRewardClaimStatus(UpdateRewardClaimStatusRequest request)
    {
        _logger.LogInformation("Changing reward claim status for reward claim {RewardClaimId} to status {RewardClaimNewStatus}",
            request.RewardClaimId, request.NewStatus);

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var rewardClaim = await dbContext.RewardClaims.FirstOrDefaultAsync();

        if ( rewardClaim == null )
            throw new Exception(RewardClaimError.RewardClaimNotFound.ToString());

        var newStatus = request.NewStatus switch
        {
            RewardClaimStatusDefinition.Claimed => RewardClaimStatus.Claimed,
            RewardClaimStatusDefinition.Unclaimed => RewardClaimStatus.Unclaimed,
            RewardClaimStatusDefinition.Cancelled => RewardClaimStatus.Cancelled,
            _ => throw new Exception()
        };

        if ( newStatus == RewardClaimStatus.Unclaimed )
            throw new Exception(RewardClaimError.NoAllowToAssignStatus.ToString());

        rewardClaim.RewardClaimStatus = newStatus;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something gone wrong while updating reward claim status {ErrorMessage}", ex.Message);
            throw new Exception(RewardClaimError.ErrorSavingData.ToString());
        }

        return rewardClaim;
    }


    public async Task<RewardClaim> UpdateRewardClaimStatusAdmin(UpdateRewardClaimStatusRequest request)
    {
        _logger.LogInformation("Changing reward claim status for reward claim {RewardClaimId} to status {RewardClaimNewStatus}",
            request.RewardClaimId, request.NewStatus);

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var rewardClaim = await dbContext.RewardClaims.FirstOrDefaultAsync();

        if ( rewardClaim == null )
            throw new Exception(RewardClaimError.RewardClaimNotFound.ToString());

        var newStatus = request.NewStatus switch
        {
            RewardClaimStatusDefinition.Claimed => RewardClaimStatus.Claimed,
            RewardClaimStatusDefinition.Unclaimed => RewardClaimStatus.Unclaimed,
            RewardClaimStatusDefinition.Cancelled => RewardClaimStatus.Cancelled,
            _ => throw new Exception()
        };

        rewardClaim.RewardClaimStatus = newStatus;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something gone wrong while updating reward claim status {ErrorMessage}", ex.Message);
            throw new Exception(RewardClaimError.ErrorSavingData.ToString());
        }

        return rewardClaim;
    }
}
