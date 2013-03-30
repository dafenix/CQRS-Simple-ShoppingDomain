using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Events
{
    public class ProductAddedToShoppingCardEvent : Event
    {
        public Guid IdProduct { get; set; }
        public string ProductName { get; set; }
        public Guid IdVisitor { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PriceOverall { get; private set; }

        public ProductAddedToShoppingCardEvent(Guid idProduct, string productName, Guid idVisitor, int quantity,
                                               decimal pricePerUnit)
        {
            IdProduct = idProduct;
            ProductName = productName;
            IdVisitor = idVisitor;
            Quantity = quantity;
            PricePerUnit = pricePerUnit;
            PriceOverall = PricePerUnit*Quantity;
        }
    }
}