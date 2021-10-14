using PaymentGateway.Models;
using System.Collections.Generic;

namespace PaymentGateway.PublishedLanguage.Command
{
    public class AddProductCommand : MediatR.IRequest
    {
        public List<CommandDetails> Details { get; set; } = new List<CommandDetails>();
        public string Cnp { get; set; }
        public string Iban { get; set; }
    }
}
