﻿using GGR.Server.Data.Models;
using GGR.Server.Commands.Contracts;
using Microsoft.EntityFrameworkCore;
using GGR.Server.Data;
using GGR.Shared.User;
using System.Security.Cryptography;
using GGR.Server.Data.Models.Utils;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using GGR.Server.Errors;
using GGR.Server.Utils;
using GGR.Server.Infrastructure.Contracts;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;

namespace GGR.Server.Commands;

public class UserCommands : IUserCommands
{
    private readonly IDbContextFactory<GlobalDbContext> _dbContextFactory;
    private readonly ILogger<UserCommands> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private IHttpContextAccessor _context;

    public UserCommands(
        IDbContextFactory<GlobalDbContext> dbContextFactory,
        ILogger<UserCommands> logger,
        IConfiguration configuration,
        IEmailSender emailSender,
        IHttpContextAccessor context)
    {
        _context = context;
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _configuration = configuration;
        _emailSender = emailSender;
    }

    public async Task<List<User>> GetUsersByEmail(string email)
    {
        _logger.LogInformation("Fetching users that email contains {Email}", email);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if ( email == null )
            throw new Exception(UserError.EmailIsNullWhenSearching.ToString());

        var users = await dbContext.Users.Where(u => u.Email.Contains(email)).ToListAsync();

        if ( !users.Any() )
            throw new Exception(UserError.NotUsersFoundByEmail.ToString());

        return users;
    }

    public async Task<int> GetTotalUsers()
    {
        _logger.LogInformation("Fetching total users");
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var totalUsers = dbContext.Users.Count();
        return totalUsers;
    }

    public async Task<int> GetTotalPoints()
    {
        _logger.LogInformation("Fetching total users points");
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var totalPoints = dbContext.Users.Sum(user => user.Points);
        return totalPoints;
    }

    public async Task<User> CreateUser(UserRegisterRequest request)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        if ( dbContext.Users.Any(user => user.Email == request.Email) )
            throw new Exception(UserError.EmailAlreadyRegistered.ToString());

        CreatePasswordHash(
            request.Password,
            out byte[] passwordHash,
            out byte[] passwordSalt);

        var userRol = request.UserRol switch
        {
            "admin" => UserRole.Admin,
            "editor" => UserRole.Editor,
            "client" => UserRole.Client,
            _ => throw new Exception(UserError.UnspecifieRole.ToString())
        };

        var userId = Guid.NewGuid();
        var user = new User()
        {
            Id = userId,
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Rol = userRol,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Points = 0
        };

        var registration = new Registration()
        {
            Id = Guid.NewGuid(),
            User = user,
            VerificationToken = CreateRandomToken(),
            RegistrationDate = DateTime.UtcNow,
            ExpiryTime = DateTime.UtcNow.AddMinutes(60),
        };

        try
        {
            Console.WriteLine(request.PhoneRegister);
            if (request.UserRol != "client" || request.PhoneRegister == "email") {
              string? email = user.Rol == UserRole.Client ? user.Email : null;
              var subject = "Verificación de cuenta GGR Gasolinera";

              await _emailSender.SendEmailAsync(email, subject, EmailVerificationBuilder.BuildVerificationEmail(GetBaseUrl(), registration.VerificationToken));
            } else {
              var message = await MessageResource.CreateAsync(
                body: $"Para verificar su cuenta en GGR ingrese al siguiente enlace: https://{GetBaseUrl()}/verify-user/{registration.VerificationToken}",
                from: new Twilio.Types.PhoneNumber("+17274784891"),
                to: new Twilio.Types.PhoneNumber($"+52{request.Phone}")
              );
            }

        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while sending email verification: {ErrorMessage}", ex.Message);
            throw new Exception(UserError.ErrorSendingVerifycationEmail.ToString());
        }


        try
        {
            dbContext.Users.Add(user);
            dbContext.Registrations.Add(registration);
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while saving user: {ErrorMessage}", ex.Message);
            throw new Exception(UserError.SavingDataError.ToString());
        }

        return user;
    }

    public async Task RestoreVerifyToken(UserRestoreVerifyTokenRequest request)
    {

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        var userRegistration = await dbContext.Registrations.FirstOrDefaultAsync(r => r.User == user);

        if ( user == null )
            throw new Exception(UserError.UserNotFound.ToString());

        if ( !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt) )
            throw new Exception(UserError.IncorrectPassword.ToString());

        if ( userRegistration == null )
            throw new Exception(UserError.RegistrationNotFound.ToString());

        if ( userRegistration.VerifiedAt != null )
            throw new Exception(UserError.UserAlreadyVerified.ToString());

        userRegistration.VerificationToken = CreateRandomToken();
        userRegistration.ExpiryTime = DateTime.UtcNow.AddMinutes(60);

         try
        {
            string? email = user.Rol == UserRole.Client ? user.Email : null;
            var subject = "Verificación de cuenta GGR Gasolinera";

            await _emailSender.SendEmailAsync(email, subject, EmailVerificationBuilder.BuildVerificationEmail(GetBaseUrl(), userRegistration.VerificationToken));
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while sending email verification: {ErrorMessage}", ex.Message);
            throw new Exception(UserError.ErrorSendingVerifycationEmail.ToString());
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while saving new token verification: {ErrorMessage}", ex.Message);
            throw new Exception(UserError.SavingDataError.ToString());
        }

    }

    public async Task<(User, string)> LoginUser(UserLoginRequest request)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        var userRegistration = await dbContext.Registrations.FirstOrDefaultAsync(r => r.User == user);

        if ( user == null )
            throw new Exception(UserError.UserNotFound.ToString());

        if ( userRegistration == null || userRegistration.VerifiedAt == null )
            throw new Exception(UserError.UserNotVerified.ToString());

        if ( !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt) )
            throw new Exception(UserError.IncorrectPassword.ToString());


        var token = CreateJWT(user);

        return (user, token);
    }

    public async Task VerifyUser(string token)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var registration = dbContext.Registrations.Include(r => r.User).FirstOrDefault(r => r.VerificationToken == token);

        if ( registration == null )
            throw new Exception(UserError.RegistrationNotFound.ToString());

        if ( registration.ExpiryTime < DateTime.UtcNow )
            throw new Exception(UserError.RegistrationExpired.ToString());

        if ( registration.User == null )
            throw new Exception(UserError.UserNotFound.ToString());

        registration.VerifiedAt = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error saving registration data: {ErrorMessage} for user {UserId}", ex.Message, registration.User.Id);
            throw new Exception(UserError.SavingDataError.ToString());
        }
    }

    public async Task ForgotUserPassword(EmailToRetorePassRequest request)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if ( user == null )
            throw new Exception(UserError.UserNotFound.ToString());

        var registration = await dbContext.Registrations.FirstOrDefaultAsync(r => r.User == user);

        if ( registration == null )
            throw new Exception(UserError.RegistrationNotFound.ToString());

        registration.PasswordResetToken = CreateRandomToken();
        registration.ResetTokenExpires = DateTime.UtcNow.AddMinutes(45);

        try
        {
            var subject = "Restablecer la contraseña GGR Gasolinera";
            await _emailSender.SendEmailAsync(user.Email, subject, EmailVerificationBuilder.BuildEmailForRestorePassword(GetBaseUrl(), registration.PasswordResetToken));
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while sending email verification: {ErrorMessage}", ex.Message);
            throw new Exception(UserError.ErrorSendingVerifycationEmail.ToString());
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error saving restore password data: {ErrorMessage} for user {UserId}", ex.Message, user.Id);
            throw new Exception(UserError.SavingDataError.ToString());
        }
    }

    public async Task RestoreUserPassword(ResetPasswordRequest request)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var registration = await dbContext.Registrations.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.PasswordResetToken == request.ResetToken);

        if ( registration == null )
            throw new Exception(UserError.RegistrationNotFound.ToString());

        if ( registration.ResetTokenExpires < DateTime.UtcNow )
            throw new Exception(UserError.RegistrationExpired.ToString());

        CreatePasswordHash(
            request.NewPassword,
            out byte[] passwordHash,
            out byte[] passwordSalt);

        registration.User.PasswordHash = passwordHash;
        registration.User.PasswordSalt = passwordSalt;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while saving user: {ErrorMessage}", ex.Message);
            throw new Exception(UserError.SavingDataError.ToString());
        }
    }

    public async Task<User> GetUserById(Guid id)
    {
        _logger.LogInformation("Fetching user id {UserId}", id);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if ( user == null )
            throw new Exception(UserError.UserNotFound.ToString());

        return user;
    }

    public async Task<User> UpdateUser(UpdateUserRequest request)
    {
        _logger.LogInformation("Updating user with id {UserId}", request.Id);
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

        if ( user == null )
            throw new Exception(UserError.UserNotFound.ToString());

        user.Name = request.Name;
        user.LastName = request.LastName;
        user.Phone = request.Phone;

        if ( !string.IsNullOrEmpty(request.NewPassword) )
        {
            if ( request.NewPassword != request.ConfirmPassword )
                throw new Exception(UserError.PasswordsDoesNotMatch.ToString());

            CreatePasswordHash(
            request.NewPassword,
            out byte[] passwordHash,
            out byte[] passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
        }

        try
        {
            await dbContext.SaveChangesAsync();
            return user;
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while saving user: {ErrorMessage}", ex.Message);
            throw new Exception(UserError.SavingDataError.ToString());
        }
    }

    private static void CreatePasswordHash(
        string password,
        out byte[] passwordHash,
        out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPasswordHash(
        string password,
        byte[] passwordHash,
        byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    private string CreateJWT(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Rol.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.UserData, user.Id.ToString()),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt").GetSection("Key").Value!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(user.Rol == UserRole.Admin ? 60 : 30),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    private static string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    public string GetBaseUrl()
    {
        return _context.HttpContext!.Request.Host.ToString();
    }
}
