using MediatR;
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
        private readonly IMediator _mediator;
        private readonly Database _database;

        public WithdrawMoney(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }
        public async Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
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

            var transaction = new Transaction
            {
                Amount = request.Amount,
                Currency = account.Currency,
                Date = DateTime.UtcNow,
                Type = "Withdraw"
            };

            if ((account.Balance -= transaction.Amount) < 0)
            {
                throw new Exception("Transaction invalid");
            }
            else
            {
                account.Balance -= transaction.Amount;
            }


            _database.Transactions.Add(transaction);
            Database.TransactionCreated();


            TransactionCreated eventMoneyWithdraw = new()
            {
                Amount = request.Amount,
                Iban = account.IbanCode
            };
            await _mediator.Publish(eventMoneyWithdraw, cancellationToken);
            return Unit.Value;

        }
    }
}
