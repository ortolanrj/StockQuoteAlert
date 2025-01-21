namespace StockQuoteAlert.Services.Email
{
    public interface IEmailService
    {
        void SendEmail(bool overResistance, Stock stock);
    }
}