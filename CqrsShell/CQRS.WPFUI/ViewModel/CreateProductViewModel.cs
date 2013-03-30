using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CQRS.Domain.Commands;

namespace CQRS.WPFUI.ViewModel
{
    public class CreateProductViewModel : INotifyPropertyChanged
    {
        private RelayCommand _CmdCreateProduct;

        public ICommand CreateProductCommand
        {
            get
            {
                if (_CmdCreateProduct == null)
                {
                    _CmdCreateProduct = new RelayCommand(p => CreateProduct());
                }
                return _CmdCreateProduct;
            }
        }

        private void CreateProduct()
        {
            var cmd = new CreateProductCommand(Name, Price);
            BusFacade.MessageBus.Send(cmd);
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (value != null)
                {
                    _Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private decimal _Price;

        public decimal Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //AddProductToShoppingCardCommand
}