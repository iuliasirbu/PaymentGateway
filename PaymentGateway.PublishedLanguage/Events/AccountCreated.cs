using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountCreated
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
