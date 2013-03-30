using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Commands
{
    public class CreateVisitorCommand : Command
    {
        public Guid Id { get; set; }

        public CreateVisitorCommand(Guid id)
        {
            Id = id;
        }
    }
}