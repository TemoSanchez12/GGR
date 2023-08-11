﻿namespace GGR.Server.Commands.Contracts;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string body);
}
