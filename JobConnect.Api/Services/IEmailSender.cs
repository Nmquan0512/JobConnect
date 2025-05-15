namespace JobConnect.Api.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string subject, string body);
    }

}
