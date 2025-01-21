namespace StockQuoteAlert.Services.Email
{
    public interface IEmailService
    {
        void SendEmail(bool isResistance);
    }
}