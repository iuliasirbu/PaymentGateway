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
    public class DepositMoney : IRequestHandler<DepositCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public DepositMoney(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }
        public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var person = _database.Persons.FirstOrDefault(p => p.Cnp == request.Cnp);
            if (person == null)
            {
                throw new Exception("User Not Found");
            }

            var account = _database.Accounts.FirstOrDefault(acc => acc.IbanCode == request.Iban);
            if (account == null)
            {
                throw new Exception("Account Not Found");
            }

            Transaction transaction = new Transaction();
            transaction.Amount = request.Amount;
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
                Currency = request.Currency,
                Amount = request.Amount,
                Iban = account.IbanCode
            };
            await _mediator.Publish(eventMoneyDeposit, cancellationToken);
            return Unit.Value;
        }
    }
}
