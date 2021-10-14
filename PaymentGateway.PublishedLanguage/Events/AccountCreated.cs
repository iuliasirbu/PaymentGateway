using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountCreated : INotification
    {
        public string Iban { get; set; }
        public string Type { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }



        public AccountCreated(string iban, string type, double balance, string currency, double limit)
        {
            this.Iban = iban;
            this.Type = type;
            this.Balance = balance;
            this.Currency = currency;
            this.Limit = limit;

        }
    }
}
