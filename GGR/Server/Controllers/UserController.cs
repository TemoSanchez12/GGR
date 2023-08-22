using GGR.Server.Commands.Contracts;
using GGR.Server.Errors;
using GGR.Shared;
using GGR.Shared.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GGR.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private static string _genericErrorMessage = "Ocurrio un error inesperado";
    private static string _successUserRegisterMessage = "El usuario ha sido registrado correctamento";
    private static string _successUserLoginMessage = "El login de usuario ha sido correcto";
    private static string _successGetUsersMessage = "Se han devuelto los usuarios con coincidencias";

    private readonly ILogger<UserController> _logger;
    private readonly IUserCommands _userCommands;

    public UserController(ILogger<UserController> logger, IUserCommands userCommands)
    {
        _logger = logger;
        _userCommands = userCommands;
    }

    [HttpGet("get-by-email/{email}")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<GetUsersResponse>>> GetUserListByEmail(string email)
    {
        var response = new ServiceResponse<GetUsersResponse>();
        try
        {
            var users = await _userCommands.GetUsersByEmail(email);
            response.Success = true;
            response.Message = _successGetUsersMessage;
            response.Data = new GetUsersResponse { Users = users.Select(u => u.ToDefinition()).ToList() };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching user by email: {ErrorMessage}", ex.Message);
            var error = (UserError)Enum.Parse(typeof(UserError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                UserError.NotUsersFoundByEmail => UserErrorMessage.NotUsersFoundByEmail,
                UserError.EmailIsNullWhenSearching => UserErrorMessage.EmailIsNullWhenSearching,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<UserRegisterResponse>>> RegisterUser(UserRegisterRequest request)
    {
        var response = new ServiceResponse<UserRegisterResponse>();
        try
        {
            var user = await _userCommands.CreateUser(request);
            response.Success = true;
            response.Message = _successUserRegisterMessage;
            response.Data = new UserRegisterResponse { UserCreated = user.ToDefinition() };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating user: {ErrorMessage}", ex.Message);
            var error = (UserError)Enum.Parse(typeof(UserError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                UserError.EmailAlreadyRegistered => UserErrorMessage.EmailAlreadyRegistered,
                UserError.UnspecifieRole => UserErrorMessage.UnspecifieRole,
                UserError.SavingDataError => UserErrorMessage.SavingUserError,
                UserError.ErrorSendingVerifycationEmail => UserErrorMessage.ErrorSendingVerifycationEmail,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
        }

    }

    [HttpPost("restore-verify-token")]
    public async Task<ActionResult> RestoreVerifyToken(UserRestoreVerifyTokenRequest request)
    {
        try
        {
            await _userCommands.RestoreVerifyToken(request);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while verifying user: {ErrorMessage}", ex.Message);
            var error = (UserError)Enum.Parse(typeof(UserError), ex.Message);

            return error switch
            {
                UserError.IncorrectPassword => Unauthorized(UserErrorMessage.IncorrectPassword),
                UserError.UserNotFound => NotFound(UserErrorMessage.UserNotFound),
                UserError.RegistrationNotFound => NotFound(UserErrorMessage.RegistrationNotFound),
                _ => BadRequest()
            };
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<UserLoginResponse>>> LoginUser(UserLoginRequest request)
    {
        var response = new ServiceResponse<UserLoginResponse>();
        try
        {
            var (user, token) = await _userCommands.LoginUser(request);

            response.Success = true;
            response.Message = _successUserLoginMessage;
            response.Data = new UserLoginResponse { User = user.ToDefinition(), Token = token };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while login user: {ErrorMessage}", ex.Message);
            var error = (UserError)Enum.Parse(typeof(UserError), ex.Message);
            response.Success = false;

            switch (error)
            {
                case UserError.UserNotFound:
                    response.Message = UserErrorMessage.UserNotFound;
                    return BadRequest(response);
                case UserError.UserNotVerified:
                    response.Message = UserErrorMessage.UserNotVerified;
                    return Unauthorized(response);
                case UserError.IncorrectPassword:
                    response.Message = UserErrorMessage.IncorrectPassword;
                    return Unauthorized(response);
                default:
                    response.Message = _genericErrorMessage;
                    return BadRequest(response);
            }

        }
    }

    [HttpPost("verify")]
    public async Task<ActionResult> VerifyUserAccount(string token)
    {
        try
        {
            await _userCommands.VerifyUser(token);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while verifying user: {ErrorMessage}", ex.Message);
            var error = (UserError)Enum.Parse(typeof(UserError), ex.Message);

            return error switch
            {
                UserError.RegistrationExpired => Unauthorized(UserErrorMessage.RegistrationExpiredToken),
                UserError.UserNotFound => NotFound(UserErrorMessage.UserNotFound),
                UserError.RegistrationNotFound => NotFound(UserErrorMessage.RegistrationNotFound),
                _ => BadRequest()
            };
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotUserPassword(string email)
    {
        try
        {
            await _userCommands.ForgotUserPassword(email);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while forgort password user: {ErrorMessage}", ex.Message);
            var error = (UserError)Enum.Parse(typeof(UserError), ex.Message);

            return error switch
            {
                UserError.UserNotFound => NotFound(UserErrorMessage.UserNotFound),
                UserError.RegistrationNotFound => NotFound(UserErrorMessage.RegistrationNotFound),
                _ => BadRequest()
            };
        }
    }

    [HttpPost("restore-password")]
    public async Task<ActionResult> RestoreUserPassword(ResetPasswordRequest request)
    {
        try
        {
            await _userCommands.RestoreUserPassword(request);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while forgort password user: {ErrorMessage}", ex.Message);
            var error = (UserError)Enum.Parse(typeof(UserError), ex.Message);

            return error switch
            {
                UserError.RegistrationNotFound => NotFound(UserErrorMessage.RegistrationNotFound),
                UserError.RegistrationExpired => Unauthorized(UserErrorMessage.RegistrationExpiredToken),
                _ => BadRequest()
            };
        }
    }
}
