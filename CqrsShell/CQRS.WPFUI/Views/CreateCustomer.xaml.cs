using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CQRS.WPFUI.ViewModel;

namespace CQRS.WPFUI.Views
{
    /// <summary>
    /// Interaktionslogik für CreateCustomer.xaml
    /// </summary>
    public partial class CreateCustomer : UserControl
    {
        public CreateCustomer()
        {
            InitializeComponent();
            this.DataContext = new CreateCustomerViewModel();
        }
    }
}