using GGR.Server.Commands.Contracts;
using System.Net;
using System.Net.Mail;

namespace GGR.Server.Commands;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private class EmailOptions
    {
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string PrincipalAdminEmail { get; set; } = string.Empty;
    }

    public Task SendEmailAsync(string? email, string subject, string message)
    {
        var emailOptions = _configuration.GetSection("EmailOptions").Get<EmailOptions>();
        if (emailOptions is null)
            throw new Exception("EmailOptions is null");

        var mail = emailOptions.Mail;
        var pass = emailOptions.Password;

        var client = new SmtpClient()
        {
            Host = emailOptions.Host,
            Port = emailOptions.Port,
            EnableSsl = emailOptions.EnableSsl,
            Credentials = new NetworkCredential(mail, pass),
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        return client.SendMailAsync(
            new MailMessage(from: mail, to: email ?? emailOptions.PrincipalAdminEmail, subject, message));
    }
}
