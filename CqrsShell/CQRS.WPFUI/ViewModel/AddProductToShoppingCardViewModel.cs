using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CQRS.Domain.Commands;
using CQRS.ReadModel;
using CQRS.ReadModel.Dto;
using CQRS.ReadModel.Reporting;
using CQRS.ReadModel.Repository;

namespace CQRS.WPFUI.ViewModel
{
    public class AddProductToShoppingCardViewModel : INotifyPropertyChanged
    {
        private readonly IReportingRepository _ProductRepository;

        public List<ProductDto> Products { get; set; }

        public AddProductToShoppingCardViewModel(IReportingRepository productRepository)
        {
            _ProductRepository = productRepository;
            Products = _ProductRepository.GetByExample<ProductDto>(new {}).ToList();
        }


        private RelayCommand _CmdAddProductToShoppingCard;

        public ICommand AddProductToShoppingCardCommand
        {
            get
            {
                if (_CmdAddProductToShoppingCard == null)
                {
                    _CmdAddProductToShoppingCard = new RelayCommand(p => AddProductToShoppingCard());
                }
                return _CmdAddProductToShoppingCard;
            }
        }

        private void AddProductToShoppingCard()
        {
            var pUid = new Guid(Product.Id);
            var cmd = new AddProductToShoppingCardCommand(Session.VisitorId, pUid, Quantity, 10.5m);
            BusFacade.MessageBus.Send(cmd);
        }

        private ProductDto _Product;

        public ProductDto Product
        {
            get { return _Product; }
            set
            {
                if (value != null)
                {
                    _Product = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _Quantity;

        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                _Quantity = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}