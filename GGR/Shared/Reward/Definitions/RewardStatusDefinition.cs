using System.Text.Json.Serialization;

namespace GGR.Shared.Reward.Definitions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RewardStatusDefinition
{
    Available,
    NotAvailable
}
