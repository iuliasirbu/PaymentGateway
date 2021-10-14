using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class TransactionCreated:INotification
    {
        public int AccountId { get; set; }
        public string Iban { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }

        
    }
}
