using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using CQRS.ReadModel.Dto;
using CQRS.WPFUI.ViewModel;

namespace CQRS.WPFUI.Views
{
    /// <summary>
    /// Interaktionslogik für ShoppingCard.xaml
    /// </summary>
    public partial class ShoppingCard : UserControl
    {
        private ObservableCollection<ShoppingCardViewModel> ShoppingCardViewModels =
            new ObservableCollection<ShoppingCardViewModel>();

        public ShoppingCard()
        {
            InitializeComponent();
            this.Loaded += ShoppingCard_Loaded;

            this.DataContext = ShoppingCardViewModels;
        }

        private void ShoppingCard_Loaded(object sender, RoutedEventArgs e)
        {
            ShoppingCardViewModels.Clear();
            var reportingRepo = BusFacade.ReportingRepository;
            var cardItems = reportingRepo.GetByExample<ShoppingCardDto>(new {IdVisitor = Session.VisitorId});
            foreach (var cardItem in cardItems)
            {
                var cardItemVm = new ShoppingCardViewModel();
                cardItemVm.ProductId = cardItem.IdProduct;
                cardItemVm.Productname = cardItem.Name;
                cardItemVm.Price = cardItem.Price.ToString(CultureInfo.InvariantCulture);
                cardItemVm.Quantity = cardItem.Quantity.ToString(CultureInfo.InvariantCulture);
                cardItemVm.PriceOverall = cardItem.PriceOverAll.ToString(CultureInfo.InvariantCulture);
                ShoppingCardViewModels.Add(cardItemVm);
            }
        }
    }
}