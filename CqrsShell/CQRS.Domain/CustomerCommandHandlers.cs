using System;
using CQRS.Domain.Aggregates;
using CQRS.Domain.Commands;
using CQRS.Domain.Events;
using CQRS.Infrastructure;

namespace CQRS.Domain
{
    public class CustomerCommandHandlers
    {
        private readonly Repository<Customer> _CustomerRepo;
        private readonly IEventPublisher _Publisher;

        public CustomerCommandHandlers(Repository<Customer> customerRepo, IEventPublisher publisher)
        {
            _CustomerRepo = customerRepo;
            _Publisher = publisher;
        }

        public void Handle(CreateCustomerCommand message)
        {
            if (message.FirstName == "d" && message.LastName == "w")
            {
                _Publisher.Publish(new CustomerCanNotBeCreatedEvent(message.FirstName, message.LastName, "exists"));
                return;
            }
            var customerId = Guid.NewGuid();
            var firstName = message.FirstName;
            var lastName = message.LastName;
            var newCustomer = new Customer(customerId, firstName, lastName);
            _CustomerRepo.Save(newCustomer);
        }

        public void Handle(RenameCustomerCommand message)
        {
            var customer = _CustomerRepo.GetById(message.Id);
            customer.Rename(message.FirstName, message.LastName);
            _CustomerRepo.Save(customer);
        }
    }
}