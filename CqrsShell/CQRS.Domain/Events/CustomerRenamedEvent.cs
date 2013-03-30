using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Events
{
    public class CustomerRenamedEvent : Event
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CustomerRenamedEvent(Guid id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}