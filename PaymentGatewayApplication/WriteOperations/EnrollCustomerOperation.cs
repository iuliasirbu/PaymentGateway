using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Application.Services;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Command;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
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

        public Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
            var person = new Person
            {
                Cnp = request.Cnp,
                Name = request.Name
            };
            if (request.ClientType == "Company")
            {
                person.Type = PersonType.Company;
            } 
            else if (request.ClientType == "Individual")
            {
                person.Type = PersonType.Individual;

            }
            else
            {
                throw new Exception("Unsupported person type");
            }

            person.Id = _database.Persons.Count + 1;

            _database.Persons.Add(person);

            Account account = new()
            {
                Type = request.AccountType,
                Currency = request.Currency,
                Balance = 0,
                IbanCode = _ibanService.GetNewIban()
            };

            _database.Accounts.Add(account);
            _database.SaveChanges();

            CustomerEnrolled eventCustEnroll = new(request.Name, request.Cnp, request.ClientType);
            _eventSender.EventSender(eventCustEnroll);
            return Unit.Task;

        }
    }
}
