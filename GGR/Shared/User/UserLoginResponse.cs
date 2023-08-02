using GGR.Shared.User.Definitions;

namespace GGR.Shared.User;

public class UserLoginResponse
{
    public UserDefinition User { get; set; } = new UserDefinition();
    public string Token { get; set; } = string.Empty;
}
