using PaymentGateway.Abstractions;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.ExternalServices;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;

namespace PaymentGateway

{
    class Program
    {
        static void Main(string[] args)
        {
            Account firstAcount = new Account();
            firstAcount.Balance = 100;

            EnrollCustomerCommand enrollCustomerCommand = new EnrollCustomerCommand();
            enrollCustomerCommand.Name = "Ion Popescu";
            enrollCustomerCommand.Cnp = "1872345667788";
            enrollCustomerCommand.ClientType = "Individual";
            enrollCustomerCommand.AccountType = "Depozit";
            enrollCustomerCommand.Currency = "EUR";

            IEventSender eventSender = new EventSender();

            EnrollCustomerOperation enrollCustomerOperation2 = new EnrollCustomerOperation(eventSender);
            enrollCustomerOperation2.PerformOperation(enrollCustomerCommand);

            CreateAccountCommand account = new CreateAccountCommand();
            account.Currency = "EUR";
            account.Type = "Credit";
            account.Limit = 100000000;

            IEventSender eventSender1 = new EventSender();
            CreateAccount createAccountOperation = new CreateAccount(eventSender);
            createAccountOperation.PerformOperation(account);

            DepositCommand depozit = new DepositCommand();
            depozit.Amount = 50;
            depozit.Currency = "EUR";
            depozit.AccountId = 123;

        }
    }
}
