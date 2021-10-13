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
    public class CreateAccount : IWriteOperation<CreateAccountCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly AccountOptions _accountOptions;
        private readonly Database _database;
        private readonly NewIban _ibanService;

        public CreateAccount(IEventSender eventSender, AccountOptions accountOptions, Database database, NewIban ibanService)
        {
            _eventSender = eventSender;
            _accountOptions = accountOptions;
            _database = database;
            _ibanService = ibanService;
        }

        public void PerformOperation(CreateAccountCommand command)
        {
            var person = _database.Persons.FirstOrDefault(e => e.Cnp == command.Cnp);


            if (command.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.Id == command.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == command.Cnp);

            }
            if (person==null)
            {
                throw new Exception("Person not found");
            }

            var account = new Account
            {
                IbanCode = _ibanService.GetNewIban(),
                Balance = 0,
                Type = command.Type,
                Currency = command.Currency,
                Limit = 500,
                Id = _database.Accounts.Count + 1
            };

            _database.Accounts.Add(account);
            _database.SaveChanges();

            AccountCreated eventAccountCreated = new(command.Iban, command.Type, command.Balance, command.Currency, command.Limit);
            _eventSender.EventSender(eventAccountCreated);

        }
    }
}
