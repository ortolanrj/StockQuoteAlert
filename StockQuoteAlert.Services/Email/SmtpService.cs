using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace StockQuoteAlert.Services.Email;

public class SmtpService : IEmailService
{
    private readonly ILogger<SmtpService> _log;

    private readonly SmtpOptions _smtpOptions;

    private readonly EmailOptions _emailOptions;

    private readonly EmailAccount Sender;

    private readonly EmailAccount Receiver;

    public SmtpService(ILogger<SmtpService> log, 
                       IOptions<SmtpOptions> smtpOptions, 
                       IOptions<EmailOptions> emailOptions)
    {
        _log = log;
        _smtpOptions = smtpOptions.Value;
        _emailOptions = emailOptions.Value;
        Sender = new EmailAccount(_emailOptions.Sender.Name, _emailOptions.Sender.Address);
        Receiver = new EmailAccount(_emailOptions.Receiver.Name, _emailOptions.Receiver.Address);
    }

    public void SendEmail(bool overResistance, Stock stock)
    {
        var email = BuildEmailMessage(overResistance, stock);

        using (var smtp = new SmtpClient())
        {
            smtp.Connect(_smtpOptions.Host, _smtpOptions.Port, false);
            smtp.Authenticate(_smtpOptions.Username, _smtpOptions.Password);
            smtp.Send(email);

            _log.LogInformation($"Um email para {Receiver.EmailAdress} foi enviado às {DateTime.Now}.");
            
            smtp.Disconnect(true);
        }
    }

    private MimeMessage BuildEmailMessage(bool overResistance, Stock stock)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(Sender.Name, Sender.EmailAdress));
        email.To.Add(new MailboxAddress(Receiver.Name, Receiver.EmailAdress));

        var resistanceOrSupport = overResistance
                                    ? "a resistência" 
                                    : "o suporte";

        var subject = $"Atenção! O preço da ação rompeu {resistanceOrSupport}.";

        email.Subject = subject;

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"<b>O preço da ação {stock.Ticker} rompeu {resistanceOrSupport}.</b><br>" +
                   $"Recomendamos a respectiva {(overResistance ? "venda" : "compra")} do ativo."
        };

        return email;
    }
}

