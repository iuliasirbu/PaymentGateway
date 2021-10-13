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
    public class WithdrawMoney:IWriteOperation<WithdrawCommand>
    {
        public IEventSender eventSender;
        private readonly Database _database;

        public WithdrawMoney(IEventSender eventSender, Database database)
        {
            this.eventSender = eventSender;
            _database = database;
        }
        public void PerformOperation(WithdrawCommand command)
        {            
            var account = _database.Accounts.FirstOrDefault(x => x.IbanCode == command.Iban);
            if (account == null)
            {
                throw new Exception("Invalid account");
            }
            var person = _database.Persons.FirstOrDefault(pers => pers.Cnp == command.Cnp);
            if (person == null)
            {
                throw new Exception("User not found");
            }
            if (person.Accounts.FindIndex(r => r.IbanCode == account.IbanCode) == -1)
            {
                throw new Exception("invalid attempt");
            }
            if (account.Limit < command.Amount)
            {
                throw new Exception("cannot withdraw this amount");
            }
            if (account.Balance < command.Amount)
            {
                throw new Exception("insufficient funds");
            }

            var transaction = new Transaction();
            transaction.Amount = command.Amount;
            transaction.Currency = account.Currency;
            transaction.Date = DateTime.UtcNow;
            transaction.Type = "Withdraw";

            if ((account.Balance -= transaction.Amount) < 0)
            {
                throw new Exception("Transaction invalid");
            }
            else
            {
                account.Balance -= transaction.Amount;
            }


            _database.Transactions.Add(transaction);
            _database.TransactionCreated();


            TransactionCreated eventMoneyWithdraw = new TransactionCreated
            {
                Amount = command.Amount,
                Iban = account.IbanCode
            };
            eventSender.EventSender(eventMoneyWithdraw);
        }
    }
}
