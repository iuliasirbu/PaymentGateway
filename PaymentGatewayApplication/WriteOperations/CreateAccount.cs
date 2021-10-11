using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateAccount : IWriteOperation<CreateAccountCommand>
    {
        public IEventSender eventSender;
        public CreateAccount(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public void PerformOperation(CreateAccountCommand command)
        {
            var random = new Random();
            Database database = Database.GetInstance();
            Account account = new Account();
            account.IbanCode = random.Next(100000).ToString();
            account.Balance = 0;
            account.Type = command.Type;
            account.Currency = command.Currency;
            account.Limit = command.Limit;
            account.Id = database.Accounts.Count + 1;

            var person = database.Persons.FirstOrDefault(x => x.Cnp == command.Cnp);

            if (command.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.Id == command.PersonId);
            }
            else
            {
                person = database.Persons.FirstOrDefault(x => x.Cnp == command.Cnp);

            }
            if (person==null)
            {
                throw new Exception("Person not found");
            }

            database.Accounts.Add(account);
            database.SaveChanges();

            AccountCreated eventAccountCreated = new(command.Iban, command.Type, command.Balance, command.Currency, command.Limit);
            eventSender.EventSender(eventAccountCreated);

        }
    }
}
