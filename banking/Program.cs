using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.Application.Queries;
using PaymentGateway.Application.Services;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Data;
using PaymentGateway.ExternalServices;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Command;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway

{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
                
            // setup
            var services = new ServiceCollection();

            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            services.RegisterBusinessServices(Configuration);

            services.Scan(scan => scan
                            .FromAssemblyOf<ListOfAccounts>()
                            .AddClasses(classes => classes.AssignableTo<IValidator>())
                            .AsImplementedInterfaces()
                            .WithScopedLifetime());

            services.AddMediatR(new[] { typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly }); // get all IRequestHandler and INotificationHandler classes

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));

            services.AddScopedContravariant<INotificationHandler<INotification>, AllEventsHandler>(typeof(CustomerEnrolled).Assembly);

            services.AddMediatR(new[] { typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly }); // get all IRequestHandler and INotificationHandler classes

            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();
            var database = serviceProvider.GetRequiredService<Database>();
            var ibanService = serviceProvider.GetRequiredService<NewIban>();
            var mediator = serviceProvider.GetRequiredService<MediatR.IMediator>();

            // use
            var enrollCustomer = new EnrollCustomerCommand
            {
                ClientType = "Company",
                AccountType = "Debit",
                Name = "Gigi Popa",
                Currency = "Eur",
                Cnp = "23"
            };

            //var enrollCustomerOperation = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            //await enrollCustomerOperation.Handle(enrollCustomer, default);

            await mediator.Send(enrollCustomer, cancellationToken);


            var makeAccountDetails = new CreateAccountCommand
            {
                Cnp = "23",
                Type = "Debit",
                Currency = "Eur"
            };
            //var makeAccountOperation = serviceProvider.GetRequiredService<CreateAccount>();
            //makeAccountOperation.Handle(makeAccountDetails, default).GetAwaiter().GetResult();

            

            await mediator.Send(makeAccountDetails, cancellationToken);



            var depositDetails = new DepositCommand
            {
                Iban = (int.Parse(ibanService.GetNewIban()) - 1).ToString(),
                Cnp = "23",
                Currency = "Ron",
                Amount = 750
            };
            database.Accounts.ForEach(e => Console.WriteLine(e.IbanCode));

            //var makeDeposit = serviceProvider.GetRequiredService<DepositMoney>();
            //makeDeposit.Handle(depositDetails, default).GetAwaiter().GetResult();

            await mediator.Send(makeAccountDetails, cancellationToken);

            var withdrawDetails = new WithdrawCommand
            {
                Amount = 150,
                Cnp = "23",
                Iban = "2"
            };

            //var makeWithdraw = serviceProvider.GetRequiredService<WithdrawMoney>();
            //makeWithdraw.Handle(withdrawDetails, default).GetAwaiter().GetResult();

            await mediator.Send(withdrawDetails, cancellationToken);

            var produs = new Product
            {
                Id = 1,
                Limit = 10,
                Name = "Pantofi",
                Currency = "Eur",
                Value = 10
            };

            var produs1 = new Product
            {
                Id = 2,
                Limit = 5,
                Name = "pantaloni",
                Currency = "Eur",
                Value = 5
            };

            var produs2 = new Product
            {
                Id = 3,
                Limit = 3,
                Name = "Camasa",
                Currency = "Eur",
                Value = 3
            };

            database.Products.Add(produs);
            database.Products.Add(produs1);
            database.Products.Add(produs2);

            var listaProduse = new List<CommandDetails>();

            var prodCmd1 = new CommandDetails
            {
                ProductId = 1,
                Quantity = 2
            };
            listaProduse.Add(prodCmd1);

            var prodCmd2 = new CommandDetails
            {
                ProductId = 2,
                Quantity = 3
            };
            listaProduse.Add(prodCmd2);

            var comanda = new AddProductCommand
            {
                Details = listaProduse,
                Iban = (Int64.Parse(ibanService.GetNewIban()) - 1).ToString()
            };

            //var purchaseProduct = serviceProvider.GetRequiredService<PurchaseProduct>();
            //purchaseProduct.Handle(comanda, default).GetAwaiter().GetResult();

            await mediator.Send(comanda, cancellationToken);

            var query = new Application.Queries.ListOfAccounts.Query
            {
                PersonId = 1
            };

            //var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            //var result = handler.Handle(query, default).GetAwaiter().GetResult();

            var result = await mediator.Send(query, cancellationToken);

        }
    }
}
