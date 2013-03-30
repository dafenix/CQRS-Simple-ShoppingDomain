using System;
using CQRS.Infrastructure;

namespace CQRS.Domain.Events
{
    public class ProductRemovedFromShoppingCardEvent : Event
    {
        public Guid IdProduct { get; set; }
        public Guid IdVisitor { get; set; }

        public ProductRemovedFromShoppingCardEvent(Guid idVisitor, Guid idProduct)
        {
            IdVisitor = idVisitor;
            IdProduct = idProduct;
        }
    }
}