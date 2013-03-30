using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Commands
{
    public class RemoveProductFromShoppingCardCommand : Command
    {
        public Guid IdVisitor { get; set; }
        public Guid IdProduct { get; set; }

        public RemoveProductFromShoppingCardCommand(Guid idVisitor, Guid idProduct)
        {
            IdVisitor = idVisitor;
            IdProduct = idProduct;
        }
    }
}