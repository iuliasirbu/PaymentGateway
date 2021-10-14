using MediatR;
using PaymentGateway.Abstractions;
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
        private readonly IEventSender _eventSender;
        private readonly Database _database;

        public PurchaseProduct(IEventSender eventSender, Database database)
        {
            _eventSender = eventSender;
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
                Product product = _database.Products.FirstOrDefault(x => x.Id == item.idProd);

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

            _database.SaveChanges();
            return Unit.Task;

        }
    }
}
