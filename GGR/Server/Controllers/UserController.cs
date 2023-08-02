using GGR.Server.Commands.Contracts;
using GGR.Server.Errors;
using GGR.Shared;
using GGR.Shared.User;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace GGR.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private static string _genericErrorMessage = "Ocurrio un error inesperado";
    private static string _successUserRegisterMessage = "El usuario ha sido registrado correctamento";
    private static string _successUserLoginMessage = "El login de usuario ha sido correcto";

    private readonly ILogger<UserController> _logger;
    private readonly IUserCommands _userCommands;

    public UserController(ILogger<UserController> logger, IUserCommands userCommands)
    {
        _logger = logger;
        _userCommands = userCommands;
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
        catch ( Exception ex )
        {
            _logger.LogError("Error while creating user: {ErrorMessage}", ex.Message);
            var error = (UserError) Enum.Parse(typeof(UserError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                UserError.EmailAlreadyRegistered => UserErrorMessage.EmailAlreadyRegistered,
                UserError.UnspecifieRole => UserErrorMessage.UnspecifieRole,
                UserError.SavingDataError => UserErrorMessage.SavingUserError,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
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
        catch ( Exception ex )
        {
            _logger.LogError("Error while login user: {ErrorMessage}", ex.Message);
            var error = (UserError) Enum.Parse(typeof(UserError), ex.Message);
            response.Success = false;

            switch ( error )
            {
                case UserError.UserNotFound:
                    response.Message = UserErrorMessage.UserNotFound;
                    return BadRequest(response);
                case UserError.UserNotVerified:
                    response.Message = UserErrorMessage.UserNotVerified;
                    return BadRequest(response);
                case UserError.IncorrectPassword:
                    response.Message = UserErrorMessage.IncorrectPassword;
                    return BadRequest(response);
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
        catch ( Exception ex )
        {
            _logger.LogError("Error while verifying user: {ErrorMessage}", ex.Message);
            var error = (UserError) Enum.Parse(typeof(UserError), ex.Message);

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
        catch ( Exception ex )
        {
            _logger.LogError("Error while forgort password user: {ErrorMessage}", ex.Message);
            var error = (UserError) Enum.Parse(typeof(UserError), ex.Message);

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
        catch ( Exception ex )
        {
            _logger.LogError("Error while forgort password user: {ErrorMessage}", ex.Message);
            var error = (UserError) Enum.Parse(typeof(UserError), ex.Message);

            return error switch
            {
                UserError.RegistrationNotFound => NotFound(UserErrorMessage.RegistrationNotFound),
                UserError.RegistrationExpired => Unauthorized(UserErrorMessage.RegistrationExpiredToken),
                _ => BadRequest()
            };
        }
    }
}
