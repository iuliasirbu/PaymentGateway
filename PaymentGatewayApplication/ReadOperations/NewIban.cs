using PaymentGateway.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.ReadOperations
{
    public class NewIban
    {
        private readonly Database _database;

        public NewIban(Database database)
        {
            _database = database;
        }

        public string GetNewIban()
        {
            List<string> ibans = _database.Accounts.Select(x => x.IbanCode).ToList();

            if (ibans.Count == 0)
                return "1";

            return (long.Parse(ibans.Last()) + 1).ToString();
        }
    }
}
