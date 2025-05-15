using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using JobConnect.Api.Services;
using JobConnect.Api.Models;

public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtp;

    public EmailSender(IOptions<SmtpSettings> smtp)
    {
        _smtp = smtp.Value;
    }

    public async Task SendEmailAsync(string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_smtp.From));
        email.To.Add(MailboxAddress.Parse(_smtp.To));
        email.Subject = subject;
        email.Body = new TextPart("plain") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_smtp.Host, _smtp.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_smtp.Username, _smtp.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
