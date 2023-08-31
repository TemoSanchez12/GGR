
using GGR.Shared.User.Definitions;

namespace GGR.Shared.User;

public class GetUserResponse
{
    public UserDefinition User { get; set; } = new UserDefinition();
}
