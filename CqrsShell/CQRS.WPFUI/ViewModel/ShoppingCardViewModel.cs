using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CQRS.Domain.Commands;

namespace CQRS.WPFUI.ViewModel
{
    public class ShoppingCardViewModel : INotifyPropertyChanged
    {
        private RelayCommand _CmdRemoveFromShoppingCard;
        public ICommand CmdRemoveFromShoppingCard
        {
            get
            {
                if (_CmdRemoveFromShoppingCard == null)
                {
                    _CmdRemoveFromShoppingCard = new RelayCommand(p => RemoveFromShoppingCard());
                }
                return _CmdRemoveFromShoppingCard;
            }
        }

        private void RemoveFromShoppingCard()
        {
            var cmd = new RemoveProductFromShoppingCardCommand(Session.VisitorId, new Guid(_ProductId));
            BusFacade.MessageBus.Send(cmd);
        }

        private string _ProductId;

        public string ProductId
        {
            get { return _ProductId; }
            set
            {
                if (value != null)
                {
                    _ProductId = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _Productname;

        public string Productname
        {
            get { return _Productname; }
            set
            {
                if (value != null)
                {
                    _Productname = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _Quantity;

        public string Quantity
        {
            get { return _Price; }
            set
            {
                if (value != null)
                {
                    _Price = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _Price;

        public string Price
        {
            get { return _Price; }
            set
            {
                if (value != null)
                {
                    _Price = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _PriceOverall;

        public string PriceOverall
        {
            get { return _PriceOverall; }
            set
            {
                if (value != null)
                {
                    _PriceOverall = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}