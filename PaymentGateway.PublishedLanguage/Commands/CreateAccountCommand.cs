using MediatR;

namespace PaymentGateway.PublishedLanguage.Command
{
    public class CreateAccountCommand: IRequest
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public string Iban { get; set; }
        public string Type { get; set; }
        public double Limit { get; set; }
        public string Cnp { get; set; }
        public int? PersonId { get; set; }

    }
}
