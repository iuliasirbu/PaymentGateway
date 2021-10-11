using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Models;
using PaymentGateway.Data;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class PurchaseProduct : IWriteOperation<AddProductCommand>
    {
        public IEventSender eventSender;
        public PurchaseProduct(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public void PerformOperation(AddProductCommand command)
        {
            Database database = Database.GetInstance();

            ProductsXTransaction pxt = new ProductsXTransaction();
            Transaction transaction = new Transaction();
            Product product = database.Products.FirstOrDefault(x => x.Name == command.Name);
            Account account = database.Accounts.FirstOrDefault(x => x.Id == command.AccountId);

            if (account == null)
            {
                throw new Exception("Invalid Account");
            }
            else if (product.Limit < command.Quantity)
            {
                throw new Exception("Product not in stock");
            }
            else if (account.Balance < command.Quantity * command.Value)
            {
                throw new Exception("Insufficient funds");
            }

            pxt.ProductId = product.Id;
            pxt.TransactionId = transaction.Id;
            pxt.Quantity = command.Quantity;

            product.Limit -= command.Quantity;

            database.ProductsXTransactions.Add(pxt);

            database.SaveChanges();



        }
    }
}
