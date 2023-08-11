using GGR.Server.Commands.Contracts;
using System.Net;
using System.Net.Mail;

namespace GGR.Server.Commands;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var mail = "temosanchez4912@gmail.com";
        var pass = "hovhbuxiicidmtsd";

        var client = new SmtpClient()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pass),
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        return client.SendMailAsync(
            new MailMessage(from: mail, to: email, subject, message));
    }
}
