using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Application.ReadOperations;
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
        private readonly IEventSender _eventSender;
        private readonly Database _database;
        private readonly NewIban _ibanService;
        public EnrollCustomerOperation(IEventSender eventSender, Database database, NewIban ibanService)
        {
            _eventSender = eventSender;
            _database = database;
            _ibanService = ibanService;
        }

        public void PerformOperation(EnrollCustomerCommand operation)
        {
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

            _database.Persons.Add(person);

            Account account = new Account();
            account.Type = operation.AccountType;
            account.Currency = operation.Currency;
            account.Balance = 0;
            account.IbanCode = _ibanService.GetNewIban();

            _database.Accounts.Add(account);
            _database.SaveChanges();

            CustomerEnrolled eventCustEnroll = new CustomerEnrolled(operation.Name, operation.Cnp, operation.ClientType);
            _eventSender.EventSender(eventCustEnroll);
        }
    }
}
