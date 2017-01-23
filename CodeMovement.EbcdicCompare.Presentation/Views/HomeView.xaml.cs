using System.Windows.Controls;
using Wcb.Eco.EbcdicCompare.Ui.ViewModel;

namespace Wcb.Eco.EbcdicCompare.Ui.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView(HomeViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public HomeViewModel ViewModel
        {
            get { return DataContext as HomeViewModel; }
            set { DataContext = value; }
        }
    }
}
