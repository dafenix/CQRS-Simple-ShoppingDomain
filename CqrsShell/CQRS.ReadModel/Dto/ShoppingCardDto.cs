namespace CQRS.ReadModel.Dto
{
    public class ShoppingCardDto
    {
        public string IdVisitor { get; set; }
        public string IdProduct { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceOverAll { get; set; }
    }
}