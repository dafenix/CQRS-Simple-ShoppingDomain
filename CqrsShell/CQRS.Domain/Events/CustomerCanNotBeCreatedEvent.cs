using CQRS.Infrastructure;

namespace CQRS.Domain.Events
{
    public class CustomerCanNotBeCreatedEvent : Event
    {
        public string Reason { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CustomerCanNotBeCreatedEvent(string firstName, string lastName, string reason)
        {
            Reason = reason;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}