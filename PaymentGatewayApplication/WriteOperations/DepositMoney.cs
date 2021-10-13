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
    public class DepositMoney : IWriteOperation<DepositCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly Database _database;
        public DepositMoney(IEventSender eventSender, Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }
        public void PerformOperation(DepositCommand command)
        {
            var person = _database.Persons.FirstOrDefault(p => p.Cnp == command.Cnp);
            if (person == null)
            {
                throw new Exception("User Not Found");
            }

            var account = _database.Accounts.FirstOrDefault(acc => acc.IbanCode == command.Iban);
            if (account == null)
            {
                throw new Exception("Account Not Found");
            }

            Transaction transaction = new Transaction();
            transaction.Amount = command.Amount;
            transaction.Date = DateTime.UtcNow;

            if ((account.Balance += transaction.Amount) > account.Limit)
            {
                throw new Exception("Transaction invalid");
            }
            else
            {
                account.Balance += transaction.Amount;
            }
               
            _database.Transactions.Add(transaction);
            _database.TransactionCreated();


            TransactionCreated eventMoneyDeposit = new TransactionCreated
            {
                Currency = command.Currency,
                Amount = command.Amount,
                Iban = account.IbanCode
            };
            _eventSender.EventSender(eventMoneyDeposit);

        }
    }
}
