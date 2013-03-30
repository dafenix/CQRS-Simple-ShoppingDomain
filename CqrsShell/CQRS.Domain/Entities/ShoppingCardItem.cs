using System;

namespace CQRS.Domain.Entities
{
    internal class ShoppingCardItem
    {
        public Guid IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PriceOverall { get; set; }
    }
}