﻿using FluentValidation;
using MediatR;
using PaymentGateway.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Queries
{
    public class ListOfAccounts
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator(Database _database)
            {
                // cod corect 
                RuleFor(q => q).Must(query =>
                {
                    var person = query.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.Id == query.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.Cnp == query.Cnp);

                    return person != null;
                }).WithMessage("Customer not found");

                //cod gresit
            //    RuleFor(q => q.PersonId).Must(personId =>
            //    {
            //        return personId.HasValue;
            //    }).WithMessage("Customer data is invalid - personid");

            //    RuleFor(q => q.Cnp).Must(cnp =>
            //    {
            //        return !string.IsNullOrEmpty(cnp);
            //    }).WithMessage("CNP is empty");

            //    RuleFor(q => q.PersonId).Must(personId =>
            //    {
            //        var exists = database.Persons.Any(x => x.Id == personId);
            //        return exists;
            //    }).WithMessage("Customer does not exist");
            //}
            }
        }

        public class Validator2 : AbstractValidator<Query>
        {
            public Validator2(Database _database)
            {
                //this.Include<Validator>(r=> r.Cnp);

                RuleFor(q => q).Must(query =>
                {
                    return query.PersonId.HasValue || !string.IsNullOrEmpty(query.Cnp);
                }).WithMessage("Customer data is invalid");
            }
        }

        
        public class Query : IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly Database _database;

            public QueryHandler(Database database)
            {
                _database = database;
            }

            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                   var person = request.PersonId.HasValue ?
                   _database.Persons.FirstOrDefault(x => x.Id == request.PersonId) :
                   _database.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);

                var db = _database.Accounts.Where(x => x.PersonId == person.Id);
                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    Iban = x.IbanCode,
                    Id = x.Id,
                    Limit = x.Limit,
                    Status = x.Status,
                    Type = x.Type
                }).ToList();
                return Task.FromResult(result); ;
            }

            //public List<Model> Handle(Query message)
            //{
            //    throw new NotImplementedException();
            //}
        }

        public class Model
        {
            public int Id { get; set; }
            public double Balance { get; set; }
            public string Currency { get; set; }
            public string Iban { get; set; }
            public string Status { get; set; }
            public double Limit { get; set; }
            public string Type { get; set; }
        }

    }
}
