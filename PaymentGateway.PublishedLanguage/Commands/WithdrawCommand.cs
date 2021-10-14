namespace PaymentGateway.PublishedLanguage.Command
{
    public class WithdrawCommand : MediatR.IRequest
    {
        public string Cnp { get; set; }
        public string Iban { get; set; }
        public double Amount { get; set; }
    }
}
