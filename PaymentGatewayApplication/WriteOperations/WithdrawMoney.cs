using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Command;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace PaymentGateway.Application.WriteOperations
{
    public class WithdrawMoney:IRequestHandler<WithdrawCommand>
    {
        public IEventSender eventSender;
        private readonly Database _database;

        public WithdrawMoney(IEventSender eventSender, Database database)
        {
            this.eventSender = eventSender;
            _database = database;
        }
        public Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {            
            var account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.Iban);
            if (account == null)
            {
                throw new Exception("Invalid account");
            }
            var person = _database.Persons.FirstOrDefault(pers => pers.Cnp == request.Cnp);
            if (person == null)
            {
                throw new Exception("User not found");
            }
            if (person.Accounts.FindIndex(r => r.IbanCode == account.IbanCode) == -1)
            {
                throw new Exception("invalid attempt");
            }
            if (account.Limit < request.Amount)
            {
                throw new Exception("cannot withdraw this amount");
            }
            if (account.Balance < request.Amount)
            {
                throw new Exception("insufficient funds");
            }

            var transaction = new Transaction();
            transaction.Amount = request.Amount;
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
                Amount = request.Amount,
                Iban = account.IbanCode
            };
            eventSender.EventSender(eventMoneyWithdraw);
            return Unit.Task;

        }
    }
}
