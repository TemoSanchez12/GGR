using GGR.Server.Data.Models;
using GGR.Shared.User;

namespace GGR.Server.Commands.Contracts;

public interface IUserCommands
{
    Task<User> CreateUser(UserRegisterRequest request);
    Task<(User, string)> LoginUser(UserLoginRequest request);
    Task VerifyUser(string token);
    Task ForgotUserPassword(string email);
    Task RestoreUserPassword(ResetPasswordRequest request);
    Task<List<User>> GetUsersByEmail(string email);
    Task RestoreVerifyToken(UserRestoreVerifyTokenRequest request);
    Task<int> GetTotalUsers();
    Task<int> GetTotalPoints();
}
