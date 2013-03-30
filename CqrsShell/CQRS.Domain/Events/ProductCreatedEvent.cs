using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Events
{
    public class ProductCreatedEvent : Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ProductCreatedEvent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}