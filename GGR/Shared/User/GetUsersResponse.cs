
using GGR.Shared.User.Definitions;

namespace GGR.Shared.User;

public class GetUsersResponse
{
    public List<UserDefinition> Users { get; set; } = new List<UserDefinition>();
}
