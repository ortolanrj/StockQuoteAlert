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

    private const string MessageGenericErrorEmail = "\nHouve um problema ao enviar o email, revise suas configurações.\nPor favor, pressione qualquer tecla para encerrar o programa, e tente novamente.";

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

    public void SendEmail(bool overSellPrice, Stock stock)
    {
        _log.LogInformation($"Ticker: {stock.Ticker} Sell Price: {stock.SellPrice} Buy Price: ${stock.BuyPrice}");

        var sellOrBuyPrice = overSellPrice
                                    ? "preço de venda"
                                    : "preço de compra";

        var email = BuildEmailMessage(overSellPrice, stock, sellOrBuyPrice);

        try
        {
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_smtpOptions.Host, _smtpOptions.Port, false);
                smtp.Authenticate(_smtpOptions.Username, _smtpOptions.Password);
                smtp.Send(email);

                Console.WriteLine($"Um email para {Receiver.EmailAdress} acabou de ser enviado! O preço da ação {stock.Ticker} " +
                                  $"ficou {(overSellPrice ? "acima" : "abaixo")} do {sellOrBuyPrice}.");

                _log.LogInformation($"An email was sent to {Receiver.EmailAdress}.");

                smtp.Disconnect(true);
            }
        } catch (Exception ex) {
            _log.LogError(ex, $"Ocurred an error sending an email to {Receiver.EmailAdress}.");
            Console.WriteLine(MessageGenericErrorEmail);
        }
    }

    private MimeMessage BuildEmailMessage(bool overSellPrice, Stock stock, string sellOrBuyPrice)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(Sender.Name, Sender.EmailAdress));
        email.To.Add(new MailboxAddress(Receiver.Name, Receiver.EmailAdress));

        var subject = $"Atenção! A ação {stock.Ticker} rompeu o {sellOrBuyPrice}.";

        // Formatting the price to BRL representation
        var formattedAmount = stock.ActualPrice.ToString("C2", new CultureInfo("pt-BR"));

        email.Subject = subject;

        var htmlContent = Utils.emailTemplate;

        var mailMessage = $@"<p>Olá!</p> <br>Percebemos que a ação {stock.Ticker} {(overSellPrice ? "ultrapassou o" : "ficou abaixo do")} {sellOrBuyPrice}.<br>
                             <p>Recomendamos a respectiva {(overSellPrice ? "venda" : "compra")} do ativo.<br>
                             O preço da ação agora está em <b>{formattedAmount}</b>.</p><br>
                             <p>Tenha um bom dia!</p>";

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = htmlContent.Replace("{mailMessage}", mailMessage)
        };

        return email;
    }
}

