namespace GGR.Shared.User;

public class UserRestoreVerifyTokenRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
