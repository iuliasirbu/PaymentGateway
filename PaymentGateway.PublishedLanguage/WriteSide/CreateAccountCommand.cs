using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class CreateAccountCommand
    {
        public int Id { get; set; }
        public double Balance { get; set; } = 0;
        public string Currency { get; set; }
        public string Iban { get; set; }
        public string Type { get; set; }
        public double Limit { get; set; }
        public string Cnp { get; set; }
        public int? PersonId { get; set; }

    }
}
