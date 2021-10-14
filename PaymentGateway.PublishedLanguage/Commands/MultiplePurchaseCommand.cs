using System.Collections.Generic;

namespace PaymentGateway.PublishedLanguage.Command
{
    public class MultiplePurchaseCommand : MediatR.IRequest
    {
        public List<CommandDetails> Details { get; set; }
        public int AccountId { get; set; }
        public class CommandDetails
        {
            public int ProductId { get; set; }
            public double Quantity { get; set; }
        }
    }
}
