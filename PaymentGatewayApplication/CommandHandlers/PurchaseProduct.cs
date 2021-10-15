using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Command;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class PurchaseProduct : IRequestHandler<AddProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;

        public PurchaseProduct(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public Task<Unit> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            Transaction transaction = new();

            Account account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.Iban);

            if (account == null)
            {
                throw new Exception("Invalid Account");
            }
            double total = 0;
            foreach (var item in request.Details)
            {
                Product product = _database.Products.FirstOrDefault(x => x.Id == item.ProductId);

                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Product not in stock");
                }
                total += product.Value * item.Quantity;

                if (account.Balance < total)
                {
                    throw new Exception("Insufficient funds");
                }

                ProductsXTransaction pxt = new()
                {
                    ProductId = product.Id,
                    TransactionId = transaction.Id,
                    Quantity = item.Quantity
                };
                product.Limit -= item.Quantity;


                _database.ProductsXTransactions.Add(pxt);
            }

            Database.SaveChanges();
            return Unit.Task;

        }
    }
}
