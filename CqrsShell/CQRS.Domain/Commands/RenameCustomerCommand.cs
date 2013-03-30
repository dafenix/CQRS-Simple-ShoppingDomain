using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Commands
{
    public class RenameCustomerCommand : Command
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public RenameCustomerCommand(Guid id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}