using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class DepositCommand 
    {
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string Iban { get; set; }
        public string Cnp { get; set; }
    }
}
