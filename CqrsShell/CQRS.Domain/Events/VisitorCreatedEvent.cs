using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Events
{
    public class VisitorCreatedEvent : Event
    {
        public Guid Id { get; set; }

        public VisitorCreatedEvent(Guid id)
        {
            Id = id;
        }
    }
}