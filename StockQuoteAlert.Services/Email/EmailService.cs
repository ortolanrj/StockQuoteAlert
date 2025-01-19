using Microsoft.Extensions.Logging;

namespace StockQuoteAlert.Services.Email;
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _log;

    public EmailService(ILogger<EmailService> log)
    {
        _log = log;
    }

    // TODO: Verify stock every 30 minutes and send an email

    public void SendEmail()
    {
        _log.LogInformation("Sending Email");
    }
}

