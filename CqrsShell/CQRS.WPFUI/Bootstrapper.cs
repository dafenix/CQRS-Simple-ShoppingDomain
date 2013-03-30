using System;
using System.Collections.Generic;
using CQRS.Domain;
using CQRS.Domain.Aggregates;
using CQRS.Domain.Commands;
using CQRS.Domain.Events;
using CQRS.Infrastructure;
using CQRS.ReadModel;
using CQRS.ReadModel.Dto;
using CQRS.ReadModel.Reporting;
using CQRS.ReadModel.Repository;

namespace CQRS.WPFUI
{
    internal class Bootstrapper
    {
        public void Init()
        {
            BusFacade.MessageBus = new Bus();
            var connectionString = @"Server=localhost;Database=mycqrstest;Uid=root;Pwd=;";
            var msSqlStore = new MySqlStore(connectionString, BusFacade.MessageBus);
            var customerRepo = new Repository<Customer>(msSqlStore, msSqlStore);
            var commandHandlers = new CustomerCommandHandlers(customerRepo, BusFacade.MessageBus);
            BusFacade.MessageBus.RegisterHandler<CreateCustomerCommand>(commandHandlers.Handle);
            BusFacade.MessageBus.RegisterHandler<RenameCustomerCommand>(commandHandlers.Handle);
            var customerDenormalizer = new CustomerDenormalizer();
            BusFacade.MessageBus.RegisterHandler<CustomerCreatedEvent>(customerDenormalizer.Handle);
            BusFacade.MessageBus.RegisterHandler<CustomerRenamedEvent>(customerDenormalizer.Handle);
            BusFacade.MessageBus.RegisterHandler<CustomerCanNotBeCreatedEvent>(new CustomerHandlers().Handle);

            var visitorRepo = new Repository<Visitor>(msSqlStore, msSqlStore);
            var productRepo = new Repository<Product>(msSqlStore, msSqlStore);
            var visitorHandlers = new VisitorCommandHandlers(visitorRepo, productRepo);
            BusFacade.MessageBus.RegisterHandler<CreateVisitorCommand>(visitorHandlers.Handle);
            BusFacade.MessageBus.RegisterHandler<AddProductToShoppingCardCommand>(visitorHandlers.Handle);
            BusFacade.MessageBus.RegisterHandler<CreateProductCommand>(visitorHandlers.Handle);
            BusFacade.MessageBus.RegisterHandler<RemoveProductFromShoppingCardCommand>(visitorHandlers.Handle);
            var productDenomalizer = new ProductDenormalizer();
            BusFacade.MessageBus.RegisterHandler<ProductCreatedEvent>(productDenomalizer.Handle);

            var selectBuilder = new SqlSelectBuilder();
            var updateBuilder = new SqlUpdateBuilder();
            var deleteBuilder = new SqlDeleteBuilder();
            var insertbuilder = new SqlInsertBuilder();
            var createBuilder = new SqlCreateBuilder();
            var reportingRepo = new MySqlReportingRepository(selectBuilder, insertbuilder, updateBuilder, deleteBuilder,
                                                             createBuilder);
            BusFacade.ReportingRepository = reportingRepo;
            reportingRepo.UseConnectionString(connectionString);
            reportingRepo.Bootstrap(new List<Type>() {typeof (ShoppingCardDto)});
            var shoppingCardDenomalizer = new ShoppingCardDenormalizer(reportingRepo);
            BusFacade.MessageBus.RegisterHandler<ProductAddedToShoppingCardEvent>(shoppingCardDenomalizer.Handle);
            BusFacade.MessageBus.RegisterHandler<ProductRemovedFromShoppingCardEvent>(shoppingCardDenomalizer.Handle);
        }
    }
}