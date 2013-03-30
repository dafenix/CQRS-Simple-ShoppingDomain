using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CQRS.Domain.Commands;

namespace CQRS.WPFUI.ViewModel
{
    public class CreateCustomerViewModel : INotifyPropertyChanged
    {
        private RelayCommand _CmdCreateCustomer;

        public ICommand CreateCustomerCommand
        {
            get
            {
                if (_CmdCreateCustomer == null)
                {
                    _CmdCreateCustomer = new RelayCommand(p => CreateCustomer());
                }
                return _CmdCreateCustomer;
            }
        }

        private void CreateCustomer()
        {
            var cmd = new CreateCustomerCommand(Firstname, Lastname);
            BusFacade.MessageBus.Send(cmd);
        }

        private string _Firstname;

        public string Firstname
        {
            get { return _Firstname; }
            set
            {
                if (value != null)
                {
                    _Firstname = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _Lastname;

        public string Lastname
        {
            get { return _Lastname; }
            set
            {
                if (value != null)
                {
                    _Lastname = value;
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