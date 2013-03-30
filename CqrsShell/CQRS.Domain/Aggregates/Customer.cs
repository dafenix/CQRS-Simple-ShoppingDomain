using System;
using CQRS.Domain.Events;
using CQRS.Infrastructure;

namespace CQRS.Domain.Aggregates
{
    public class Customer : AggregateRoot
    {
        private Guid _Id;
        private string _FirstName;
        private string _LastName;

        public Customer()
        {
        }

        public Customer(Guid id, string firstName, string lastName)
        {
            ApplyChange(new CustomerCreatedEvent(id, firstName, lastName));
        }

        private void Apply(CustomerCreatedEvent @event)
        {
            _Id = @event.Id;
            _FirstName = @event.FirstName;
            _LastName = @event.LastName;
        }

        public void Rename(string firstName, string lastName)
        {
            ApplyChange(new CustomerRenamedEvent(_Id, firstName, lastName));
        }

        private void Apply(CustomerRenamedEvent @event)
        {
            _FirstName = @event.FirstName;
            _LastName = @event.LastName;
        }

        public override Guid Id
        {
            get { return _Id; }
        }

        public override Snapshot Snapshot
        {
            get { throw new NotImplementedException(); }
        }
    }
}