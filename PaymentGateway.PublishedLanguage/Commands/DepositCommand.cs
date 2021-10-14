namespace PaymentGateway.PublishedLanguage.Command
{
    public class DepositCommand : MediatR.IRequest
    {
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string Iban { get; set; }
        public string Cnp { get; set; }
    }
}
