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
        public IEventSender eventSender;
        public DepositMoney(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(DepositCommand command)
        {

            Database database = Database.GetInstance();
            Transaction transaction = new Transaction();
            transaction.Amount = command.Amount;
            transaction.Date = DateTime.UtcNow;

            var account = database.Accounts.FirstOrDefault(x => x.Id == command.AccountId);
            if (account == null)
            {
                throw new Exception("Invalid account");
            }
            if (account.Currency != command.Currency)
            {
                throw new Exception("The currency is not valid");

            }


            if ((account.Balance += transaction.Amount) > account.Limit)
            {
                throw new Exception("Transaction invalid");
            }
            else
            {
                account.Balance += transaction.Amount;
            }

            database.Transactions.Add(transaction);
            database.TransactionCreated();


            TransactionCreated eventMoneyDeposit = new TransactionCreated
            {
                AccountId = command.AccountId,
                Currency = command.Currency,
                Amount = command.Amount,
                Iban = account.IbanCode
            };
            eventSender.EventSender(eventMoneyDeposit);

        }
    }
}
