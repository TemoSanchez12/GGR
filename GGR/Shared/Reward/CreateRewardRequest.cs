
namespace GGR.Shared.Reward;

public class CreateRewardRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PricePoints { get; set; }
    public string Base64Photo { get; set; } = string.Empty;
    public int UnitsAvailable { get; set; }
    public bool IsActive { get; set; } = false;
}
