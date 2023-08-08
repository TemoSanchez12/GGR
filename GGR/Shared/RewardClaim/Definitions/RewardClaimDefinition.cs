
namespace GGR.Shared.RewardClaim.Definitions;

public class RewardClaimDefinition
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } = new();
    public Guid RewardId { get; set; } = new();
    public RewardClaimStatusDefinition RewardClaimStatus { get; set; }
    public DateTime ClaimCreated { get; set; }
    public DateTime ClaimUpdated { get; set; }
}
