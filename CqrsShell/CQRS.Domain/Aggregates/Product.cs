using System;
using CQRS.Domain.Events;
using CQRS.Infrastructure;

namespace CQRS.Domain.Aggregates
{
    public class Product : AggregateRoot
    {
        private Guid _Id;
        private string _Name;

        public Product()
        {
        }

        public Product(Guid id, string name)
        {
            ApplyChange(new ProductCreatedEvent(id, name));
        }

        private void Apply(ProductCreatedEvent @event)
        {
            _Id = @event.Id;
            _Name = @event.Name;
        }

        public override Guid Id
        {
            get { return _Id; }
        }

        public override Snapshot Snapshot
        {
            get { return null; }
        }

        public string Name
        {
            get { return _Name; }
        }
    }
}