using System.Data;
using System.Linq;
using CQRS.Domain.Events;
using CQRS.Infrastructure;
using CQRS.ReadModel.Dto;
using CQRS.ReadModel.Reporting;
using CQRS.ReadModel.Repository;
using MySql.Data.MySqlClient;

namespace CQRS.ReadModel
{
    public class ShoppingCardDenormalizer : Handles<ProductAddedToShoppingCardEvent>,
                                            Handles<ProductRemovedFromShoppingCardEvent>
    {
        private readonly IReportingRepository _Repository;

        public ShoppingCardDenormalizer(IReportingRepository repository)
        {
            _Repository = repository;
        }

        public void Handle(ProductAddedToShoppingCardEvent message)
        {
            var shoppingCardViewModel = new ShoppingCardDto();
            shoppingCardViewModel.IdVisitor = message.IdVisitor.ToString();
            shoppingCardViewModel.IdProduct = message.IdProduct.ToString();
            shoppingCardViewModel.Name = message.ProductName;
            shoppingCardViewModel.Quantity = message.Quantity;
            shoppingCardViewModel.Price = message.PricePerUnit;
            shoppingCardViewModel.PriceOverAll = message.PriceOverall;
            _Repository.Save(shoppingCardViewModel);
        }

        public void Handle(ProductRemovedFromShoppingCardEvent message)
        {
            _Repository.Delete<ShoppingCardDto>(
                new {IdVisitor = message.IdVisitor.ToString(), IdProduct = message.IdProduct.ToString()});
        }
    }
}