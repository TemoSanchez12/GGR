using GGR.Shared.User;
using GGR.Shared;

namespace GGR.Client.Areas.Users.Services.Contract;

public interface IUserClientService
{
    Task<ServiceResponse<UserLoginResponse>> UserLogin(UserLoginRequest request);
    Task UserLogout();
}
