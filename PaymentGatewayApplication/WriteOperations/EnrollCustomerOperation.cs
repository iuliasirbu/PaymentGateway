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
    public class EnrollCustomerOperation : IWriteOperation<EnrollCustomerCommand>
    {
        public IEventSender eventSender;
        public EnrollCustomerOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public void PerformOperation(EnrollCustomerCommand operation)
        {
            var random = new Random();
            Database database = Database.GetInstance();
            Person person = new Person();
            person.Cnp = operation.Cnp;
            person.Name = operation.Name;
            if (operation.ClientType == "Company")
            {
                person.Type = PersonType.Company;
            } 
            else if (operation.ClientType == "Individual")
            {
                person.Type = PersonType.Individual;

            }
            else
            {
                throw new Exception("Unsupported person type");
            }

            database.Persons.Add(person);

            Account account = new Account();
            account.Type = operation.AccountType;
            account.Currency = operation.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            database.Accounts.Add(account);
            database.SaveChanges();

            CustomerEnrolled eventCustEnroll = new CustomerEnrolled(operation.Name, operation.Cnp, operation.ClientType);
            eventSender.EventSender(eventCustEnroll);
        }
    }
}
