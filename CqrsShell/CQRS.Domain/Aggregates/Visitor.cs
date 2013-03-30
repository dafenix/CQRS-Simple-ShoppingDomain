using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Domain.Commands;
using CQRS.Domain.Entities;
using CQRS.Domain.Events;
using CQRS.Infrastructure;

namespace CQRS.Domain.Aggregates
{
    public class Visitor : AggregateRoot
    {
        private Guid _Id;
        private List<ShoppingCardItem> _OrderItems = new List<ShoppingCardItem>();

        public Visitor()
        {
        }

        public void AddItemToShoppingCard(Guid idProduct, string productName, int quantity, decimal pricePerUnit)
        {
            if (_OrderItems.Any(o => o.IdProduct.ToString() == idProduct.ToString()))
            {
                return;
            }
            ApplyChange(new ProductAddedToShoppingCardEvent(idProduct, productName, Id, quantity, pricePerUnit));
        }

        private void Apply(ProductAddedToShoppingCardEvent @event)
        {
            var orderItem = new ShoppingCardItem();
            orderItem.IdProduct = @event.IdProduct;
            orderItem.PricePerUnit = @event.PricePerUnit;
            orderItem.Quantity = @event.Quantity;
            orderItem.PriceOverall = @event.PricePerUnit*@event.Quantity;
            _OrderItems.Add(orderItem);
        }

        public Visitor(Guid id)
        {
            ApplyChange(new VisitorCreatedEvent(id));
        }

        private void Apply(VisitorCreatedEvent @event)
        {
            _Id = @event.Id;
        }


        public override Guid Id
        {
            get { return _Id; }
        }

        public override Snapshot Snapshot
        {
            get { return null; }
        }

        public void RemoveProductFromShoppingCard(Guid idProduct)
        {
            ApplyChange(new ProductRemovedFromShoppingCardEvent(Id,idProduct));
        }

        private void Apply(ProductRemovedFromShoppingCardEvent @event)
        {
            _OrderItems.RemoveAll(p => p.IdProduct == @event.IdProduct);
        }

    }
}