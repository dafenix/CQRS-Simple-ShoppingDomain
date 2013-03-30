using System;
using CQRS.Domain.Aggregates;
using CQRS.Domain.Commands;
using CQRS.Infrastructure;

namespace CQRS.Domain
{
    public class VisitorCommandHandlers
    {
        private readonly Repository<Visitor> _VisitorRepo;
        private readonly Repository<Product> _ProductRepo;

        public VisitorCommandHandlers(Repository<Visitor> visitorRepo, Repository<Product> productRepo)
        {
            _VisitorRepo = visitorRepo;
            _ProductRepo = productRepo;
        }

        public void Handle(CreateVisitorCommand message)
        {
            var newVisitor = new Visitor(message.Id);
            _VisitorRepo.Save(newVisitor);
        }

        public void Handle(CreateProductCommand message)
        {
            var productId = Guid.NewGuid();
            var name = message.Name;
            var price = message.Price;
            var newProduct = new Product(productId, name);
            _ProductRepo.Save(newProduct);
        }

        public void Handle(AddProductToShoppingCardCommand message)
        {
            var visitor = _VisitorRepo.GetById(message.IdVisitor);
            var product = _ProductRepo.GetById(message.IdProduct);
            var quantity = message.Quantity;

            visitor.AddItemToShoppingCard(product.Id, product.Name, quantity, message.PricePerUnit);
            _VisitorRepo.Save(visitor);
        }

        public void Handle(RemoveProductFromShoppingCardCommand message)
        {
            var visitor = _VisitorRepo.GetById(message.IdVisitor);
            var product = _ProductRepo.GetById(message.IdProduct);
            visitor.RemoveProductFromShoppingCard(product.Id);
            _VisitorRepo.Save(visitor);
        }
    }
}