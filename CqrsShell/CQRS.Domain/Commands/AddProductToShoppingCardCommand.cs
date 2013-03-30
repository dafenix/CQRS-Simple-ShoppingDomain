using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Commands
{
    public class AddProductToShoppingCardCommand : Command
    {
        public Guid IdVisitor { get; set; }
        public Guid IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }

        public AddProductToShoppingCardCommand(Guid idVisitor, Guid idProduct, int quantity, decimal pricePerUnit)
        {
            IdVisitor = idVisitor;
            IdProduct = idProduct;
            Quantity = quantity;
            PricePerUnit = pricePerUnit;
        }
    }
}