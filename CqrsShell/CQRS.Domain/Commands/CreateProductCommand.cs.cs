using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Commands
{
    public class CreateProductCommand : Command
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public CreateProductCommand(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }
}