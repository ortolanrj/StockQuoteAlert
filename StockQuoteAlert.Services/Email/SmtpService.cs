using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Globalization;

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

            Console.WriteLine($"Um email para {Receiver.EmailAdress} acabou de ser enviado! O preço da ação {stock.Ticker} ultrapassou o preço alvo.");
            _log.LogInformation($"An email was sent to {Receiver.EmailAdress}.");
            
            smtp.Disconnect(true);
        }
    }

    private MimeMessage BuildEmailMessage(bool overSellPrice, Stock stock)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(Sender.Name, Sender.EmailAdress));
        email.To.Add(new MailboxAddress(Receiver.Name, Receiver.EmailAdress));

        var sellOrBuyPrice = overSellPrice
                                    ? "o preço de venda" 
                                    : "o preço de compra";

        var subject = $"Atenção! A ação rompeu {sellOrBuyPrice}.";

        var formattedAmount = stock.ActualPrice.ToString("C2", new CultureInfo("pt-BR"));

        email.Subject = subject;

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"Olá! <br>Percebemos que a ação {stock.Ticker} ultrapassou {sellOrBuyPrice}.<br>" +
                   $"Recomendamos a respectiva {(overSellPrice ? "venda" : "compra")} do ativo.<br>" +
                   $"O preço da ação hoje está em {formattedAmount}.<br><br>" +
                   $"Tenha um bom dia!"
        };

        return email;
    }
}

