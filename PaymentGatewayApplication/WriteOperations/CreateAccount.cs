using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Application.Services;
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
    public class CreateAccount : IRequestHandler<CreateAccountCommand>
    {
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;
        private readonly Database _database;
        private readonly NewIban _ibanService;

        public CreateAccount(IMediator mediator, AccountOptions accountOptions, Database database, NewIban ibanService)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _database = database;
            _ibanService = ibanService;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var person = _database.Persons.FirstOrDefault(e => e.Cnp == request.Cnp);


            if (request.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.Id == request.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);

            }
            if (person==null)
            {
                throw new Exception("Person not found");
            }

            var account = new Account
            {
                IbanCode = _ibanService.GetNewIban(),
                Balance = 0,
                Type = request.Type,
                Currency = request.Currency,
                Limit = 500,
                Id = _database.Accounts.Count + 1
            };

            _database.Accounts.Add(account);
            _database.SaveChanges();

            AccountCreated eventAccountCreated = new(request.Iban, request.Type, request.Balance, request.Currency, request.Limit);
            await _mediator.Publish(eventAccountCreated, cancellationToken);

        
        return Unit.Value;
        }
    }
}
