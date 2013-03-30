using CQRS.Infrastructure;

namespace CQRS.Domain.Commands
{
    public class CreateCustomerCommand : Command
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CreateCustomerCommand(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}