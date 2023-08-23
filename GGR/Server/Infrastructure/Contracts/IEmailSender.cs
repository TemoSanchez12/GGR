namespace GGR.Server.Infrastructure.Contracts;

public interface IEmailSender
{
    Task SendEmailAsync(string? email, string subject, string body);
}
