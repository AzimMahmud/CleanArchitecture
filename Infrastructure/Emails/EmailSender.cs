using Application.Configuration.Emails;

namespace Infrastructure.Emails;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(EmailMessage message)
    {
        // Integration with email service.

        return;
    }
}