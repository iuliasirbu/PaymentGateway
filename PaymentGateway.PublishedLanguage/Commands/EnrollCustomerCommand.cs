namespace PaymentGateway.PublishedLanguage.Command
{
    public class EnrollCustomerCommand : MediatR.IRequest
    {
        public string Name { get; set; }
        public string Cnp { get; set; }
        public string ClientType { get; set; }
        public string AccountType { get; set; }
        public string Currency { get; set; }



    }
}
