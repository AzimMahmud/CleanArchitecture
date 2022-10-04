namespace Application.Configuration.Emails;

public interface IEmailSender
{
    Task SendEmailAsync(EmailMessage message);
}