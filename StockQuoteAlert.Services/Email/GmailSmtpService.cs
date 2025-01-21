using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace StockQuoteAlert.Services.Email;

public class GmailSmtpService : IEmailService
{
    private readonly ILogger<GmailSmtpService> _log;

    private readonly IOptions<SmtpOptions> _smtpOptions;

    private readonly IOptions<EmailOptions> _emailOptions;

    private readonly EmailAccount sender;

    private readonly EmailAccount receiver;

    public GmailSmtpService(ILogger<GmailSmtpService> log, 
                            IOptions<SmtpOptions> smtpOptions, 
                            IOptions<EmailOptions> emailOptions)
    {
        _log = log;
        _smtpOptions = smtpOptions;
        _emailOptions = emailOptions;
        sender = new EmailAccount(_emailOptions.Value.Sender.Name, _emailOptions.Value.Sender.Address);
        receiver = new EmailAccount(_emailOptions.Value.Receiver.Name, _emailOptions.Value.Receiver.Address);
    }

    public void SendEmail(bool isResistance)
    {
        var email = BuildEmailMessage(isResistance);

        using (var smtp = new SmtpClient())
        {
            smtp.Connect(_smtpOptions.Value.Host, _smtpOptions.Value.Port, false);
            smtp.Authenticate(_smtpOptions.Value.Username, _smtpOptions.Value.Password);

            smtp.Send(email);

            _log.LogInformation($"Um email para {receiver.EmailAdress} foi enviado às {DateTime.Now}.");

            smtp.Disconnect(true);
        }
    }

    private MimeMessage BuildEmailMessage(bool isResistance)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(sender.Name, sender.EmailAdress));
        email.To.Add(new MailboxAddress(receiver.Name, receiver.EmailAdress));

        var resistanceOrSupport = isResistance 
                                    ? "a resistência" 
                                    : "o suporte";

        var subject = $"Atenção! O preço da ação rompeu {resistanceOrSupport}.";

        email.Subject = subject;

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"<b>O preço da ação {} rompeu {resistanceOrSupport}.</b><br>" +
                   $"Recomendamos a respectiva {(isResistance ? "venda" : "compra")} do ativo."
        };

        return email;
    }
}

