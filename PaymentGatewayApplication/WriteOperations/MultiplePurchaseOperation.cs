using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class MultiplePurchaseOperation : IWriteOperation<MultiplePurchaseCommand>
    {
        public void PerformOperation(MultiplePurchaseCommand command)
        {
            Database database = Database.GetInstance();

            Transaction transaction = new Transaction();

            Account account = database.Accounts.FirstOrDefault(x => x.Id == command.AccountId);

            if (account == null)
            {
                throw new Exception("Invalid Account");
            }
            var total = 0d;
            foreach (var item in command.Details)
            {
                Product product = database.Products.FirstOrDefault(x => x.Id == item.ProductId);
                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Product not in stock");
                }
                total += product.Value * item.Quantity;
                transaction.Value = total;
                ProductsXTransaction pxt = new ProductsXTransaction
                {
                    ProductId = product.Id,
                    TransactionId = transaction.Id,
                    Quantity = item.Quantity
                };
                product.Limit -= item.Quantity;

                if(product.Limit < 0)
                {
                    throw new Exception("Out of stock");
                }
                database.ProductsXTransactions.Add(pxt);
            }

            if (account.Balance < total)
            {
                throw new Exception("Insufficient funds");
            }

            database.SaveChanges();
        }
    }
}
