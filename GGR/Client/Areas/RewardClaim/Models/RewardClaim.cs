using static GGR.Client.Routes;

namespace GGR.Client.Areas.RewardClaim.Models;

public class RewardClaim
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid RewardId { get; set; }
    public string RewardName { get; set; } = string.Empty;
    public RewardClaimStatus RewardClaimStatus { get; set; }
    public DateTime ClaimCreated { get; set; }
    public DateTime ClaimUpdated { get; set; }
}
